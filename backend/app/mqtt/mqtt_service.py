import json
import paho.mqtt.client as mqtt


class MQTTService:

    def __init__(self, broker="192.168.1.95", port=1883):
        self.broker = broker
        self.port = port
        self.latest_data = {}
        self.sensor_service = None

        self.client = mqtt.Client(
            mqtt.CallbackAPIVersion.VERSION2,
            client_id="acquaceg-backend"
        )

        self.client.on_connect = self.on_connect
        self.client.on_message = self.on_message

    def set_sensor_service(self, sensor_service):
        self.sensor_service = sensor_service

    def connect(self):
        self.client.connect(self.broker, self.port, keepalive=60)
        self.client.loop_start()

    def on_connect(self, client, userdata, flags, reason_code, properties):
        print(f"Reason code: {reason_code} ({reason_code.getName()})")

        if reason_code == 0:
            print("Conectado ao broker MQTT!")
            client.subscribe(
                "acquaceg/devices/arduino-01/sensors/raw",
                qos=1
            )
        else:
            print(f"Erro ao conectar: {reason_code} ({reason_code.getName()})")

    def on_message(self, client, userdata, msg):
        try:
            data = json.loads(msg.payload.decode())
            self.latest_data = data
            print(f"[{msg.topic}] Nova leitura: {data}")

            if self.sensor_service:
                self.sensor_service.save_reading(data)

        except (UnicodeDecodeError, json.JSONDecodeError) as e:
            print(f"Erro ao interpretar mensagem MQTT: {e}")

    def disconnect(self):
        self.client.loop_stop()
        self.client.disconnect()
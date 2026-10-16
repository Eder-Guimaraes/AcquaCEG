import json
import paho.mqtt.client as mqtt


class MQTTService:

    def __init__(self, broker="broker.emqx.io", port=1883):
        self.broker = broker
        self.port = port
        self.latest_data = {}

        self.client = mqtt.Client(
            mqtt.CallbackAPIVersion.VERSION2,
            client_id="acquaceg-backend"
        )

        self.client.on_connect = self.on_connect
        self.client.on_message = self.on_message

    def connect(self):
        self.client.connect(self.broker, self.port, keepalive=60)
        self.client.loop_start()

    def on_connect(self, client, userdata, flags, reason_code, properties):
        if reason_code == 0:
            print("Conectado ao broker MQTT!")
            client.subscribe(
                "acquaceg/devices/arduino-01/sensors/raw",
                qos=1
            )
        else:
            print(f"Erro ao conectar: {reason_code}")

    def on_message(self, client, userdata, msg):
        try:
            self.latest_data = json.loads(msg.payload.decode())
            print(f"[{msg.topic}] Nova leitura: {self.latest_data}")

        except (UnicodeDecodeError, json.JSONDecodeError) as e:
            print(f"Erro ao interpretar mensagem MQTT: {e}")

    def get_latest(self):
        return self.latest_data

    def disconnect(self):
        self.client.loop_stop()
        self.client.disconnect()
using System;
using System.Windows.Forms;

namespace app
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Inicia o app abrindo o PainelPrincipal
            Application.Run(new PainelPrincipal());
        }
    }
}
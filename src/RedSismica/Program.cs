using System;
using System.Windows.Forms;

namespace RedSismica
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var analista = new AnalistaEnSismos("Ana Gómez", "agomez@redsismica.gob.ar");
            var usuario = new Usuario("agomez", analista);
            var sesion = new Sesion(DateTime.Now, usuario);
            var gestor = new GestorRegistrarResultado(sesion);

            Application.Run(new PantallaRegistrarResultado(gestor));
        }
    }
}

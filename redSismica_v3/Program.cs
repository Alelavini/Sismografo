using System;
using System.Windows.Forms;

namespace RedSismicaWinForms
{
    static class Program
    {
        [STAThread]
        static void Main()
        {

            // Configuración de la aplicación
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Cambiá aquí para crear un usuario y pasar el gestor:
            Usuario analistaEnSismos = new Usuario("Analista Sismos");
            Sesion sesion = new Sesion(DateTime.Now, analistaEnSismos);
            GestorRegistrarResultado gestor = new GestorRegistrarResultado(sesion);

            Application.Run(new PantRegistrarResultado(gestor));
        }
    }
}
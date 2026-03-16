namespace RedSismicaWinForms
{
    public class ClasificacionSismo
    {
        private string nombre;

        public ClasificacionSismo(string nombre)
        {
            this.nombre = nombre;
        }

        public string getNombre() => nombre;
    }
}

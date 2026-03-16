namespace RedSismicaWinForms
{
    public class OrigenDeGeneracion
    {
        private string descripcion;
        private string nombre;

        public OrigenDeGeneracion(string nombre, string descripcion)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
        }

        public string getNombre() => nombre;
        public string getDescripcion() => descripcion;
    }
}

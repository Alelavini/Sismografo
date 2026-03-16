namespace RedSismicaWinForms
{
    public class AlcanceSismo
    {
        private string descripcion;
        private string nombre;

        public AlcanceSismo(string nombre, string descripcion)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
        }

        public string getNombre() => nombre;
        public string getDescripcion() => descripcion;
    }
}

namespace RedSismica
{
    public class OrigenDeGeneracion
    {
        public static readonly string[] Nombres = { "Interplaca", "Intraplaca", "Volcánico", "Explosión" };

        private string descripcion;
        private string nombre;

        public OrigenDeGeneracion(string nombre, string descripcion)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
        }

        public string getNombre() => nombre;
        public string getDescripcion() => descripcion;

        public override string ToString() => nombre;
    }
}

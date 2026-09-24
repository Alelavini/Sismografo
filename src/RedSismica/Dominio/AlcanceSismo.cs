namespace RedSismica
{
    public class AlcanceSismo
    {
        public static readonly string[] Nombres = { "Sismo local", "Sismo regional", "Telesismo" };

        private string descripcion;
        private string nombre;

        public AlcanceSismo(string nombre, string descripcion)
        {
            this.nombre = nombre;
            this.descripcion = descripcion;
        }

        public string getNombre() => nombre;
        public string getDescripcion() => descripcion;

        public override string ToString() => nombre;
    }
}

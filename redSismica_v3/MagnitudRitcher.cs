namespace RedSismicaWinForms
{
    public class MagnitudRitcher
    {
        private double numero;
        private string descripcion;
        private string unidad;

        public MagnitudRitcher(double numero, string descripcion, string unidad)
        {
            this.numero = numero;
            this.descripcion = descripcion;
            this.unidad = unidad;
        }

        public double getNumero() => numero;
        public string getDescripcion() => descripcion;
        public string getUnidad() => unidad;
    }
}

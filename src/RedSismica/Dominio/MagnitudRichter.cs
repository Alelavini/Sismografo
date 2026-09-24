namespace RedSismica
{
    public class MagnitudRichter
    {
        private double numero;
        private string descripcion;

        public MagnitudRichter(double numero, string descripcion)
        {
            this.numero = numero;
            this.descripcion = descripcion;
        }

        public double getNumero() => numero;
        public string getDescripcion() => descripcion;

        // Descripción según la escala de Richter
        public static MagnitudRichter desdeValor(double numero)
        {
            string descripcion = numero switch
            {
                < 2.0 => "Micro",
                < 4.0 => "Menor",
                < 5.0 => "Ligero",
                < 6.0 => "Moderado",
                < 7.0 => "Fuerte",
                < 8.0 => "Mayor",
                _ => "Gran terremoto"
            };
            return new MagnitudRichter(numero, descripcion);
        }
    }
}

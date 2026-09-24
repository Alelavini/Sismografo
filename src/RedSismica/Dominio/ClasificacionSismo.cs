namespace RedSismica
{
    public class ClasificacionSismo
    {
        private string nombre;
        private double kmProfundidadDesde;
        private double kmProfundidadHasta;

        public ClasificacionSismo(string nombre, double desde, double hasta)
        {
            this.nombre = nombre;
            this.kmProfundidadDesde = desde;
            this.kmProfundidadHasta = hasta;
        }

        public string getNombre() => nombre;

        public bool esDeProfundidad(double km) => km >= kmProfundidadDesde && km < kmProfundidadHasta;

        // Clasificación según la profundidad del hipocentro
        public static ClasificacionSismo desdeProfundidad(double km)
        {
            if (km < 70) return new ClasificacionSismo("Superficial", 0, 70);
            if (km < 300) return new ClasificacionSismo("Intermedio", 70, 300);
            return new ClasificacionSismo("Profundo", 300, 700);
        }
    }
}

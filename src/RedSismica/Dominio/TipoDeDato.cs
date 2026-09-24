namespace RedSismica
{
    public class TipoDeDato
    {
        private string denominacion;
        private string nombreUnidadMedida;
        private double valorUmbral;

        public TipoDeDato(string denominacion, string unidad, double umbral)
        {
            this.denominacion = denominacion;
            this.nombreUnidadMedida = unidad;
            this.valorUmbral = umbral;
        }

        public string getDenominacion() => denominacion;
        public string getNombreUnidadMedida() => nombreUnidadMedida;
        public double getValorUmbral() => valorUmbral;
    }
}

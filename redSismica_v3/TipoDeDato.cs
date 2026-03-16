namespace RedSismicaWinForms
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

        public string getDenominacion()
        {
            return denominacion;
        }
    }
}

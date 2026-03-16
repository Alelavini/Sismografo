namespace RedSismicaWinForms
{
    public class DetalleMuestraSismica
    {
        private double valor;
        private TipoDeDato tipoDato;

        public DetalleMuestraSismica(double valor, TipoDeDato tipoDato)
        {
            this.valor = valor;
            this.tipoDato = tipoDato;
        }

        public double getDatos()
        {
            return valor;
        }

        public TipoDeDato getTipoDato()
        {
            return tipoDato;
        }
    }
}

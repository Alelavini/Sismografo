namespace RedSismica
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

        public double getDatos() => valor;
        public TipoDeDato getTipoDato() => tipoDato;
        public bool superaUmbral() => valor > tipoDato.getValorUmbral();
    }
}

namespace RedSismica
{
    public class Estado
    {
        public const string AmbitoEventoSismico = "Evento Sismico";

        public const string AutoDetectado = "Auto Detectado";
        public const string BloqueadoEnRevision = "Bloqueado en revisión";
        public const string Confirmado = "Confirmado";
        public const string Rechazado = "Rechazado";
        public const string DerivadoAExperto = "Derivado a Experto";

        private string descripcion;
        private string ambito;

        public Estado(string desc)
        {
            this.descripcion = desc;
            ambito = AmbitoEventoSismico;
        }

        public string getDescripcion() => descripcion;
        public bool esAutoDetectado() => descripcion == AutoDetectado;
        public bool esRechazado() => descripcion == Rechazado;
        public bool esBloqueadoEnRevision() => descripcion == BloqueadoEnRevision;
        public bool esAmbitoEventoSismico() => ambito == AmbitoEventoSismico;
    }
}

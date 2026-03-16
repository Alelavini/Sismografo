namespace RedSismicaWinForms
{
    public class Estado
    {
        private string descripcion;
        private string ambito;

        public Estado(string desc) { 
            this.descripcion = desc;
            ambito = "Evento Sismico";
        }
        public string getDescripcion() => descripcion;
        public bool esAutoDetectado() => descripcion == "Auto Detectado";
        public bool esRechazado() => descripcion == "Rechazado";
        public bool esBloqueadoEnRevision() => descripcion == "Bloqueado en revisión";
        public bool esAmbitoEventoSismico() => ambito == "Evento Sismico";
    }
}

namespace RedSismica
{
    // Estados finales de la revisión manual: no admiten ninguna transición

    public class Confirmado : Estado
    {
        public const string Nombre = "Confirmado";

        public Confirmado() : base(Nombre) { }

        public override bool esFinal() => true;
    }

    public class Rechazado : Estado
    {
        public const string Nombre = "Rechazado";

        public Rechazado() : base(Nombre) { }

        public override bool esRechazado() => true;
        public override bool esFinal() => true;
    }

    public class DerivadoAExperto : Estado
    {
        public const string Nombre = "Derivado a experto";

        public DerivadoAExperto() : base(Nombre) { }

        public override bool esFinal() => true;
    }
}

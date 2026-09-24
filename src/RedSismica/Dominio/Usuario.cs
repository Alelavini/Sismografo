namespace RedSismica
{
    public class Usuario
    {
        private string nombre;
        private AnalistaEnSismos? analistaEnSismos;

        public Usuario(string nombre, AnalistaEnSismos? analista = null)
        {
            this.nombre = nombre;
            this.analistaEnSismos = analista;
        }

        public string getUsuario() => nombre;
        public string getAnalistaEnSismos() => analistaEnSismos?.getNombre() ?? nombre;
    }
}

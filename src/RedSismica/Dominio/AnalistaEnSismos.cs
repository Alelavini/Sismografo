using System;

namespace RedSismica
{
    public class AnalistaEnSismos
    {
        private string nombre;
        private string mail;

        public AnalistaEnSismos(string nombre, string mail)
        {
            this.nombre = nombre;
            this.mail = mail;
        }
        
        public string getNombre() => nombre;
        public string getMail() => mail;
    }
}

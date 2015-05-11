using System;

namespace PNT_2.Data
{
    public class Utilizador
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Nif { get; set; }
        public string Login { get; set; }
        public Registos LastEntry { get; set; }
        public Boolean IsActive { get; set; }



    }
}

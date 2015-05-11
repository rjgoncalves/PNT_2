using System;

namespace PNT_2.Data
{
    public class Registos
    {

        public long Id { get; set; }

        public long IdColaborador { get; set; }

        public DateTime Data { get; set; }

        public Enums.Tipo Tipo { get; set; }

        public string Observações { get; set; }




    }
}

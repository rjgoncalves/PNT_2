using System;

namespace PNT_2.Data
{
   public class Picagens
    {
       public long IdColaborador { get; set; }
       public DateTime Data{get;set;}
       public DateTime Inicio { get; set; }
       public DateTime? Fim { get; set; }
       public String Duracao { get; set; }
       public String Tipo { get; set; }
       public long? RegistoInicio { get; set; }
       public long? RegistoFim { get; set; }
       
    }
}

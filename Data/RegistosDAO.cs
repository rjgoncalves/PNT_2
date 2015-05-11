using System;
using PNT_2.Data;
namespace PNT_2.Data
{

    public interface IRegistosDao
    {
        
    }


    public class RegistosDao
    {
        public Boolean RegisterEntry(Registos registos)
        {
            try
            {
                using (var m = new ModelPnt())
                {
                    m.Registos.Add(registos);
                    m.SaveChanges();

                    return true;
                }
            }
            catch (Exception)
            {
                
                return false;
            }



        }
    }
}


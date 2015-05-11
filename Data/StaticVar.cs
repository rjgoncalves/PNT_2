using System;
using System.Linq;

namespace PNT_2.Data
{
    public static class StaticVar
    {
        private static Utilizador _utilizador;
        public static Utilizador CurrentUtilizador
        {
            get
            {

                try
                {
                    if (_utilizador == null)
                    {
                        using (var mod = new ModelPnt())
                        {
                            _utilizador = mod.Utilizadores.ToList().Find(u => u.Login == Environment.UserName);
                            return _utilizador;


                        }
                    }
                    return _utilizador;
                }
                catch (Exception ex)
                {

                    return null;
                }



            }


        }



    }
}

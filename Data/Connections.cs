using System.Data.OleDb;

namespace PNT_2.Data
{
    public class Connections
    {
        private static OleDbConnection _cnn;
        public static OleDbConnection Conn
        {
            get
            {

                const string src = @"C:\Users\Ricardo\Documents\Visual Studio 2013\Projects\PNT_2\Files\Pnto.accdb";
                const string pass = "tmn.2012";
                _cnn = new OleDbConnection
                {
                    ConnectionString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password={1};", src, pass)
                };


                return _cnn;

            }


        }
    }
}
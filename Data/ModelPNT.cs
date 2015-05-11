using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace PNT_2.Data
{
    public class ModelPnt : IDisposable
    {
        private List<Registos> _regInsertionList = new List<Registos>();
        private List<Registos> _regDeleteList = new List<Registos>();

        private readonly OleDbConnection _cnn = Connections.Conn;

        private ObservableCollection<Registos> _registo;
        public ObservableCollection<Registos> Registos
        {
            get
            {
                
                return _registo ?? (_registo = LoadRegistos());
            }
        }

        private ObservableCollection<Utilizador> _utilizadores;
        public ObservableCollection<Utilizador> Utilizadores
        {

            get
            {

                return _utilizadores ?? (_utilizadores = LoadUtilizadores());
            }

        }

        private List<Picagens> _picagens;
        public List<Picagens> Picagens
        {
            get { return _picagens ?? (_picagens = LoadPicagens()); }
        }

        public ModelPnt()
        {
            try
            {
                if (_cnn.State != ConnectionState.Open)
                {

                    _cnn.Open();



                }


                Registos.CollectionChanged += Registos_CollectionChanged;



            }
            catch (Exception ex)
            {

                throw ex;
            }





        }

        void Registos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var i in e.NewItems)
                {
                    _regInsertionList.Add((Registos)i);

                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {

                foreach (var o in e.OldItems)
                {


                    _regDeleteList.Add((Registos)o);

                }
            }


        }

        public void Refresh()
        {
            _registo = null;
            _picagens = null;
            _utilizadores = null;
            _regDeleteList = null;
            _regInsertionList = null;
        }

        private ObservableCollection<Registos> LoadRegistos()
        {

            try
            {


                OleDbCommand cmd = new OleDbCommand("select * from tbl_registos", _cnn);
                var dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    var lst = new ObservableCollection<Registos>();
                    while (dr.Read())
                    {

                        lst.Add(new Registos
                        {
                            Id = dr[0] == DBNull.Value ? 0 : Convert.ToInt64(dr[0].ToString()),
                            IdColaborador = dr[1] == DBNull.Value ? 0 : Convert.ToInt64(dr[1].ToString()),
                            Data = dr[2] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr[2].ToString()),
                            Tipo = dr[3] == DBNull.Value ? 0 : (Enums.Tipo)Convert.ToInt16(dr[3].ToString()),
                            Observações = dr[4] == DBNull.Value ? null : dr[4].ToString()

                        });



                    }



                    dr.Dispose();
                    cmd.Dispose();
                    return lst;


                }

                return null;



            }

            catch (Exception ex)
            {

                throw ex;
            }
        }

        private ObservableCollection<Utilizador> LoadUtilizadores()
        {
            try
            {

                var strSql = @"SELECT tblDadosPessoais.ID_FUNCIONARIO
	                                    ,tblDadosPessoais.NOME
                                        ,tblDadosPessoais.NIF
	                                    ,tblLogins.LOGIN_GERAL As Login
                                    ,tblDAdosContratuais.Estado
                                    FROM (
	                                    tblDadosContratuais INNER JOIN tblDadosPessoais ON tblDadosContratuais.ID_FUNCIONARIO = tblDadosPessoais.ID_FUNCIONARIO
	                                    )
                                    INNER JOIN tblLogins ON tblDadosPessoais.ID_FUNCIONARIO = tblLogins.ID_FUNCIONARIOS;
                                    ";

                var reg = _registo;
                OleDbCommand cmd = new OleDbCommand(strSql, _cnn);
                var dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    var lst = new ObservableCollection<Utilizador>();
                    while (dr.Read())
                    {

                        lst.Add(new Utilizador
                        {
                            Id = dr[0] == DBNull.Value ? 0 : Convert.ToInt64(dr[0].ToString()),
                            Nome = dr[1] == DBNull.Value ? "" : dr[1].ToString(),
                            Nif = dr[2] == DBNull.Value ? "" : dr[2].ToString(),
                            Login = dr[3] == DBNull.Value ? null : dr[3].ToString(),
                            IsActive = dr[4] == DBNull.Value ? false : (dr[4].ToString() == "1" ? true : false)

                        });



                    }

                    dr.Dispose();
                    cmd.Dispose();
                    return lst;


                }








                return null;



            }

            catch (Exception ex)
            {

                throw ex;
            }

        }

        private List<Picagens> LoadPicagens()
        {

            var lstPicagens = new List<Picagens>();
            var lstUtils = Registos.Select(u => u.IdColaborador).Distinct().ToList();

            lstUtils.ForEach(u =>
            {
                lstPicagens.AddRange(GetPicagens(u, Enums.Tipo.LogIn, Enums.Tipo.LogOut, Enums.PicagensDesignacao.Logins));
                lstPicagens.AddRange(GetPicagens(u, Enums.Tipo.PausaIn, Enums.Tipo.PausaOut, Enums.PicagensDesignacao.Pausa));
                lstPicagens.AddRange(GetPicagens(u, Enums.Tipo.RefeicaoIn, Enums.Tipo.RefeicaoOut, Enums.PicagensDesignacao.Refeição));
                lstPicagens.AddRange(GetPicagens(u, Enums.Tipo.IndisponiblidadeIn, Enums.Tipo.IndisponibilidadeOut, Enums.PicagensDesignacao.Indisponibilidade));
                lstPicagens.AddRange(GetPicagens(u, Enums.Tipo.FormacaoIn, Enums.Tipo.FormacaoOut, Enums.PicagensDesignacao.Formação));
            });



            return lstPicagens;
        }

        private List<Picagens> GetPicagens(long id, Enums.Tipo tipoInicio, Enums.Tipo tipoFim, Enums.PicagensDesignacao picagem)
        {
            try
            {

           var lstPausaIn = _registo.Where(r => r.IdColaborador == id && r.Tipo == tipoInicio).ToList();
            var lstPausaOut = _registo.Where(r => r.IdColaborador == id && r.Tipo == tipoFim).ToList();
            var lstPausas = new List<Picagens>();
            DateTime? dtfim;
            lstPausaIn.ForEach(pi =>
            {
                var pf = lstPausaOut.Find(pfim => pfim.Data >= pi.Data);
                var p = new Picagens();
                p.Data = pi.Data.Date;
                p.Inicio = pi.Data;
                dtfim = pf == null ? DateTime.MinValue : pf.Data;
                p.Fim = dtfim == DateTime.MinValue ? null : dtfim;
                p.IdColaborador = id;
                p.Duracao = dtfim == DateTime.MinValue ? "" : ((TimeSpan)(p.Fim - p.Inicio)).ToString();
                p.Tipo = picagem.ToString();
                p.RegistoInicio = pi.Id;
                p.RegistoFim = pf == null ? 0 : pf.Id;
                lstPausas.Add(p);
            });


            return lstPausas;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Dispose()
        {
            _cnn.Close();
            _cnn.Dispose();

        }

        private void SaveRegisto(Registos reg)
        {

            try
            {
                var da = new OleDbDataAdapter("select * from tbl_registos", _cnn);
                var cmdb = new OleDbCommandBuilder(da);
                OleDbCommand cmd = cmdb.GetInsertCommand();
                cmd.Parameters[0].Value = reg.IdColaborador;
                cmd.Parameters[1].Value = reg.Data;
                cmd.Parameters[2].Value = reg.Tipo;
                cmd.Parameters[3].Value = reg.Observações ?? "";

                cmd.ExecuteNonQuery();
                cmd.Dispose();
                cmdb.Dispose();
                da.Dispose();
            }

            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void DeleteRegisto(Registos reg)
        {

            try
            {

                OleDbCommand cmd = new OleDbCommand("Delete from tbl_registos Where Id = @Id", _cnn); //cmdb.GetDeleteCommand();

                cmd.Parameters.AddWithValue("@Id", reg.Id);


                cmd.ExecuteNonQuery();
                cmd.Dispose();

            }

            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void SaveChanges()
        {
            if (_regInsertionList.Count != 0)
            {
                _regInsertionList.ForEach(SaveRegisto);

            }

            if (_regDeleteList.Count != 0)
            {
                _regDeleteList.ForEach(DeleteRegisto);

            }

            Refresh();
        }
    }
}

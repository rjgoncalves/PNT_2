using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PNT_2.Data;

namespace PNT_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        List<Picagens> _picagens = new List<Picagens>();
        List<Registos> _registos;
        List<Button> buttons = new List<Button>();

        readonly BackgroundWorker bgw = new BackgroundWorker();
        public MainWindow()
        {

            if (StaticVar.CurrentUtilizador == null)
            {
                MessageBox.Show("Sem permissões de acesso.", "Sem permissões.");
                Close();
                return;
            }


            bgw.DoWork += BgwOnDoWork;
            bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
            InitializeComponent();
            using (var m = new ModelPnt())
            {
                _registos = m.Registos.ToList().Where(r => r.IdColaborador == StaticVar.CurrentUtilizador.Id).ToList();

            }



        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var reg = _registos.OrderBy(r => r.Data).Last();
            txtbEstado.Text = reg.Tipo.ToString();
            txtbData.Text = reg.Data.ToString(CultureInfo.CurrentCulture);
        }

        void BgwOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            using (var m = new ModelPnt())
            {
                _registos = m.Registos.ToList().Where(r => r.IdColaborador == StaticVar.CurrentUtilizador.Id).ToList();
                _picagens = m.Picagens.Where(p => p.IdColaborador == StaticVar.CurrentUtilizador.Id).ToList();
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (!bgw.IsBusy) { bgw.RunWorkerAsync(); }
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - ActualHeight;

            buttons.Add(btnLogin);
            buttons.Add(btnOut);
            buttons.Add(btnPausa);
            buttons.Add(btnRef);
            buttons.Add(btnForm);
            buttons.Add(btnIndis);
            buttons.Add(btnAdmin);

            var reg = _registos.OrderBy(r => r.Data).Last();

            if (reg != null)
            {
                txtbEstado.Text = reg.Tipo.ToString();
                txtbData.Text = reg.Data.ToString(CultureInfo.CurrentCulture);
                EnableButtons(reg.Tipo);
            }
            else
            {
                txtbEstado.Text = "";
                txtbData.Text = "";
                EnableButtons(Enums.Tipo.LogIn);
            }


        }

        private void EnableButtons(Enums.Tipo tipo)
        {

            switch (tipo)
            {
                case Enums.Tipo.LogIn:
                    buttons.ForEach(b =>
                    {
                        b.IsEnabled = true;

                    });
                    buttons.Find(b => b.Name == btnOut.Name).IsEnabled = false;
                    break;
                case Enums.Tipo.LogOut:
                    buttons.ForEach(b =>
                    {
                        b.IsEnabled = false;

                    });
                    buttons.Find(b => b.Name == btnOut.Name).IsEnabled = true;
                    buttons.Find(b => b.Name == btnLogin.Name).IsEnabled = true;
                    break;
                case Enums.Tipo.PausaIn:
                    buttons.ForEach(b =>
                    {
                        b.IsEnabled = false;

                    });
                    buttons.Find(b => b.Name == btnPausa.Name).IsEnabled = true;
                    break;
                case Enums.Tipo.PausaOut:
                    buttons.ForEach(b =>
                    {
                        b.IsEnabled = true;

                    });
                    buttons.Find(b => b.Name == btnOut.Name).IsEnabled = false;
                    break;
                case Enums.Tipo.RefeicaoIn:
                    buttons.ForEach(b =>
                   {
                       b.IsEnabled = false;

                   });
                    buttons.Find(b => b.Name == btnRef.Name).IsEnabled = true;
                    break;
                case Enums.Tipo.RefeicaoOut:
                    buttons.ForEach(b =>
                                    {
                                        b.IsEnabled = true;

                                    });
                    buttons.Find(b => b.Name == btnOut.Name).IsEnabled = false;
                    break;
                case Enums.Tipo.IndisponiblidadeIn:
                    buttons.ForEach(b =>
                   {
                       b.IsEnabled = false;

                   });
                    buttons.Find(b => b.Name == btnIndis.Name).IsEnabled = true;
                    break;
                case Enums.Tipo.IndisponibilidadeOut:
                    buttons.ForEach(b =>
          {
              b.IsEnabled = true;

          });
                    buttons.Find(b => b.Name == btnOut.Name).IsEnabled = false;
                    break;
                case Enums.Tipo.FormacaoIn:
                    buttons.ForEach(b =>
                   {
                       b.IsEnabled = false;

                   });
                    buttons.Find(b => b.Name == btnForm.Name).IsEnabled = true;
                    break;
                case Enums.Tipo.FormacaoOut:
                    buttons.ForEach(b =>
                        {
                            b.IsEnabled = true;

                        });
                    buttons.Find(b => b.Name == btnOut.Name).IsEnabled = false;
                    break;

            }


        }

        private void AddRegisto(object sender, RoutedEventArgs e)
        {
            using (var m = new ModelPnt())
            {
                _registos = m.Registos.ToList().OrderBy(r=> r.Data).Where(r => r.IdColaborador == StaticVar.CurrentUtilizador.Id).ToList();
                var reg = _registos.Last();

                var newRegisto = new Registos();
                newRegisto.IdColaborador = StaticVar.CurrentUtilizador.Id;
                newRegisto.Data = DateTime.Now;
                

            var btn = (Button)sender;
                if (btn == btnLogin && reg.Tipo == Enums.Tipo.LogOut)
                {
                    newRegisto.Tipo = Enums.Tipo.LogIn;
                }
                else
                {
                    newRegisto.Tipo = Enums.Tipo.LogOut;
                }
                if (btn == btnLogin && reg.Tipo == Enums.Tipo.PausaOut)
                {
                    newRegisto.Tipo = Enums.Tipo.PausaIn;
                }
                else
                {
                    newRegisto.Tipo = Enums.Tipo.PausaOut;
                }
                if (btn == btnLogin && reg.Tipo == Enums.Tipo.FormacaoOut)
                {
                    newRegisto.Tipo = Enums.Tipo.FormacaoIn;
                }
                else
                {
                    newRegisto.Tipo = Enums.Tipo.FormacaoOut;
                }
                if (btn == btnLogin && reg.Tipo == Enums.Tipo.IndisponibilidadeOut)
                {
                    newRegisto.Tipo = Enums.Tipo.IndisponiblidadeIn;
                }
                else
                {
                    newRegisto.Tipo = Enums.Tipo.IndisponibilidadeOut;
                }

                m.Registos.Add(newRegisto);
                m.SaveChanges();
                EnableButtons(newRegisto.Tipo);
            }

          
        }

        private void BtnAdmin_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }


    }
}

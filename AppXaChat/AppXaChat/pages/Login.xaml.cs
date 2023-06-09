using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySqlConnector;
//using MySql.Data.MySqlClient;
//using System.Data;

namespace AppXaChat.pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            lbClick();
            NavigationPage.SetHasBackButton(this, false);

            if (Application.Current.Properties.ContainsKey("usuario"))
            {
                entrUsuario.Text = Application.Current.Properties["usuario"] as string;
            }
            if (Application.Current.Properties.ContainsKey("contrasena"))
            {
                entrContra.Text = Application.Current.Properties["contrasena"] as string;
            }
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            string connection = "Server=127.0.0.1;Port=3306;User ID=root;Password=12345;Database=chat";
            MySqlConnection sqlconnection = new MySqlConnection(connection);
            sqlconnection.Close();
            //sqlconnection.Close();

            try
            {
                sqlconnection.Close();
                sqlconnection.Open();
                MySqlCommand query1 = new MySqlCommand("select id from CLIENTES where id = '" + entrUsuario.Text + "'", sqlconnection);
                var leerUsuario = query1.ExecuteReader();
                if (leerUsuario.Read())
                {
                    sqlconnection.Close();
                    sqlconnection.Open();
                    MySqlCommand query2 = new MySqlCommand("select * from CLIENTES where id = '" + entrUsuario.Text + "' AND password = '" + entrContra.Text + "'", sqlconnection);
                    var leerContra = query2.ExecuteReader();
                    if (leerContra.Read())
                    {
                        //DisplayAlert("ALERTA", "El usuario ingresado ya existe!", "Aceptar");
                        sqlconnection.Close();
                        string user = entrUsuario.Text;
                        //permanencia de datos
                        string usuario = entrUsuario.Text;
                        string contrastena = entrContra.Text;
                        Application.Current.Properties["usuario"] = usuario;
                        Application.Current.Properties["contrasena"] = contrastena;
                        await Application.Current.SavePropertiesAsync();
                        //navegar a la página chat
                        await Application.Current.MainPage.Navigation.PushAsync(new MainPage(user));
                    }
                    else
                    {
                        _ = DisplayAlert("ALERTA", "Contraseña incorrecta!", "Aceptar");
                        sqlconnection.Close();
                        entrContra.Text = null;
                    }

                }
                else
                {
                    _ = DisplayAlert("ALERTA", "Usuario no encontrado!", "Aceptar");
                    sqlconnection.Close();
                    entrUsuario.Text = null;
                    entrContra.Text = null;
                }
            }
            catch (Exception ex)
            {
                 _ = DisplayAlert("ERROR", ex.ToString(), "OK");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void btnRegistro_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Registrar());
        }

        public void lbClick()
        {
            lbUpdate.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    await Navigation.PushAsync(new Update());
                })
            });
        }
    }
}
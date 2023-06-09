using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppXaChat.pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Registrar : ContentPage
    {
        public MySqlConnection sqlconnection = new MySqlConnection("Server=127.0.0.1;Port=3306;User ID=root;Password=12345;Database=chat");
        public MySqlCommand queri1 = new MySqlCommand();

        public Registrar()
        {
            InitializeComponent();
        }

        private void btnRegistrar_Clicked(object sender, EventArgs e)
        {
            //MySqlConnection sqlconnection = new MySqlConnection(connection);
            sqlconnection.Close();
            sqlconnection.Open();
            MySqlCommand queri1 = new MySqlCommand("select id from CLIENTES where id = '" + entrUsuarioreg.Text + "'", sqlconnection);
            var rd = queri1.ExecuteReader();

            try
            {
                if (rd.Read())
                {
                    _ = DisplayAlert("ALERTA", "El usuario ingresado ya existe!", "Aceptar");
                    entrUsuarioreg.Text = null;
                }
                else
                {
                    Connection();
                }
            }
            catch(Exception ex)
            {
                _ = DisplayAlert("ALERTA", ex.ToString(), "OK");
            }
        }

        private async void Connection()
        {
            //MySqlConnection sqlconnection = new MySqlConnection(connection);
            

            try
            {
                sqlconnection.Close();
                sqlconnection.Open();

                if (!string.IsNullOrEmpty(entrUsuarioreg.Text))
                {
                    if(!string.IsNullOrEmpty(entrContrareg.Text) && !string.IsNullOrEmpty(entrConfirmContr.Text))
                    {
                        if (entrContrareg.Text == entrConfirmContr.Text)
                        {

                            MySqlCommand query = new MySqlCommand("insert into CLIENTES(id,password) values('" + entrUsuarioreg.Text + "', '" + entrContrareg.Text + "')", sqlconnection);
                            var rd = query.ExecuteReaderAsync();
                            _ = DisplayAlert("ALERTA", "Registrado correctamente, inicie sesión", "Aceptar");
                            entrUsuarioreg.Text = null;
                            entrContrareg.Text = null;
                            entrConfirmContr.Text = null;
                            sqlconnection.Close();
                            await Navigation.PushAsync(new Login());

                        }
                        else
                        {
                            _ = DisplayAlert("ALERTA", "La contraseña no coindice!", "Aceptar");
                            entrContrareg.Text = null;
                            entrConfirmContr.Text = null;
                        }
                    }
                    else
                    {
                        _ = DisplayAlert("ALERTA", "Escriba su contraseña!", "Aceptar");
                    }

                }
                else
                {
                    _ = DisplayAlert("ALERTA", "Escriba su nombre de usuario", "Aceptar");
                }

            }
            catch (Exception ex)
            {
                _ = DisplayAlert("ALERTA", ex.ToString(), "OK");
            }
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }
    }
}
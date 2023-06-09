using AppXaChat.pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace AppXaChat
{

    public partial class MainPage : ContentPage
    {
        public TcpClient tcpClient;
        static string name;
        private static List<string> MessageList = new List<string>();
        public static ObservableCollection<string> Mensajes = new ObservableCollection<string>();
        public MainPage(string user)
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);

            name = user;
            tcpClient = ConfigurarCliente();
            StreamWriter writer = new StreamWriter(tcpClient.GetStream());
            //name;
            writer.WriteLine(name + " " + "se ha conectado");
            writer.Flush();

            Thread thread = new Thread(LeerDatos);
            thread.Start(tcpClient);
            
        }

        public async void Prueba_Clicked(object sender, EventArgs e)
        {
                try
                {
                    if (tcpClient.Connected && !string.IsNullOrEmpty(entrMsj.Text))
                    {
                        StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());

                        string input = entrMsj.Text;
                        sWriter.WriteLine(name + ": " + input);
                        Mensajes.Add("Tú: " + input);
                        lvMsj.ItemsSource = Mensajes;
                        entrMsj.Text = "";
                        sWriter.Flush();
                    }
                    else if (string.IsNullOrEmpty(entrMsj.Text))
                    {
                        _ = DisplayAlert("ALERTA", "El mensaje no debe de estar vacío!", "OK");
                    }
                }
                catch
                {
                    _ =DisplayAlert("ALERTA", "No hay conexión con el servidor!", "OK");
                    await Navigation.PushAsync(new Login());
            }
        }

        public void LeerDatos(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            NetworkStream stream = tcpClient.GetStream();
            string message = string.Empty;
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            data = new byte[256];
            String responseData = String.Empty;
            do
            {
                try
                {
                    //Leer los datos e imprimirlos en el otro cliente
                    Int32 bytes = stream.Read(data, 0, data.Length);
                    responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                    if (!string.IsNullOrEmpty(responseData))
                    {
                        Mensajes.Add(responseData);
                        lvMsj.ItemsSource = Mensajes;
                    }
                    else
                    {
                        break;
                    }


                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                    
                }
            } while (true);
        }

        //Configurando TcpClient
        public TcpClient ConfigurarCliente()
        {
            TcpClient Client = new TcpClient("127.0.0.1", 2611);
            return Client;
        }
        private async void btnSalir_Clicked(object sender, EventArgs e)
        {
            bool op = await DisplayAlert("ALERTA", "¿Está seguro que desea cerrar sesión?", "Cerrar sesión", "Cancelar");
            if(op == true)
            {
                StreamWriter writer = new StreamWriter(tcpClient.GetStream());
                writer.WriteLine(name + " " + "se ha desconectado");
                writer.Flush();
                tcpClient.Close();
                await Navigation.PushAsync(new Login());
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
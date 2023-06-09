using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;
namespace Server
{
    class Program
    {
        private static TcpListener Listener;
        //list de clientes
        private static List<TcpClient> tcpClientsList = new List<TcpClient>();
        //lista de mensajes
        private static List<string> MessageList = new List<string>();
        //ruta para guardar los mensajes de los clientes
        private static string savePath = @"/home/ubuntu/chat-server/mensajes.txt";

        //Iniciando servidor
        public static void SetupServer()
        {
            //creando un nuevo TcpListener
            Listener = new TcpListener(IPAddress.Any, 2611);
            Listener.Start();
            Console.WriteLine("Servidor iniciado, esperando conexiones...");
        }

        //Esperando la conexión del cliente
        public static void ClientListener(object obj)
        {
            TcpClient client = (TcpClient)obj;
            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter Writer = new StreamWriter(client.GetStream());

            //Console.WriteLine("Client connected");
            try
            {
                while (true)
                {
                    //Leer los datos del cliente
                    string message = reader.ReadLine();
                    if(!string.IsNullOrEmpty(message))
                    {
                        SaveMessage(message);
                        BroadcastMessage(message, client);
                        //Mostrar los datos recibidos del cliente en la consola del servidor
                        Console.WriteLine(message);
                    }
                    else
                    {
                        Console.WriteLine("Cliente Desconectado...");
                        break;
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Cliente Desconectado...");
            }
            tcpClientsList.Remove(client);
        }

        public static void BroadcastMessage(string message, TcpClient excludeClient)
        {
            foreach (TcpClient client in tcpClientsList)
            {
                if (client != excludeClient)
                {
                    StreamWriter Writer = new StreamWriter(client.GetStream());
                    Writer.WriteLine(message);
                    Writer.Flush();
                }
            }
        }
        //Guardar los mensajes de los clientes en un archivo .txt
        public static void SaveMessage(string newMessage)
        {
            try
            {
                //Leer los mensajes y agregarlos a la lista
                MessageList = File.ReadAllLines(savePath).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error al guardar el archivo : " + e);
            }

            // Agregar mensaje a la lista
            MessageList.Add(newMessage);

            // Guardar los mensajes en el archivo .txt
            File.WriteAllLines(savePath, MessageList);
        }
        static void Main()
        {
            SetupServer();
            while (true)
            {
                //Esperando y aceptando nuevo cliente
                //Console.WriteLine("Esperando Clientes...");
                TcpClient Client = Listener.AcceptTcpClient();
                tcpClientsList.Add(Client);

                //Crear un nuevo hilo después de aceptar el cliente
                Thread thread = new Thread(ClientListener);
                thread.Start(Client);

            }
        }
    }

}

using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 13000;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                string message = userName;
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                Console.WriteLine("#BOT: Добро пожаловать, {0}", userName);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Метод SendMessage преобразует сообщение пользователя в массив байтов и отправляет на сервер.
        /// </summary>
        static void SendMessage()
        {
            Console.Write("Введите сообщение: ");

            while (true)
            {
                string message = Console.ReadLine();

                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Метод RecriveMessage получает с сервера массив байтов и преобразует его в строку.
        /// </summary>
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    // Отключаем клиент если получаем пустое сообщение.
                    if(message != "")
                    {
                        Console.WriteLine(message);//вывод сообщения
                    }
                    else
                    {
                        Console.WriteLine("Завершение чата...");
                        Disconnect();
                        Environment.Exit(0); //завершение процесса
                    }
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        /// <summary>
        /// Метод Disconnect завершает работу клиента и уведомляет об этом сервер.
        /// </summary>
        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}

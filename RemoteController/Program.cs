using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemoteController
{
    static class Program
    {

        // Порт
        static int port = 10000;
        // Адрес
        static IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
        // Оправить сообщение
        const byte codeMsg = 1;
        // Повернуть экран
        const byte codeRotate = 2;
        // Выключить компьютер
        const byte codePoff = 3;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
           


            // Создаем локальную конечную точку
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            // Создаем основной сокет
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // Связываем сокет с конечной точкой
                socket.Bind(ipEndPoint);
                // Переходим в режим "прослушивания"
                socket.Listen(1);
                while (true)
                {
                    // Ждем соединение. При удачном соединение создается новый экземпляр Socket
                    Socket handler = socket.Accept();
                    // Массив, где сохраняем принятые данные.
                    byte[] recBytes = new byte[1024];
                    int nBytes = handler.Receive(recBytes);
                    switch (recBytes[0])    // Определяемся с командами клиента
                    {
                        case codeMsg:   // Сообщение                         
                            nBytes = handler.Receive(recBytes); // Читаем данные сообщения
                            if (nBytes != 0)
                            {
                                // Преобразуем полученный набор байт в строку
                                String msg = Encoding.UTF8.GetString(recBytes, 0, nBytes);
                                MessageBox.Show(msg, "Привет Пупсик!");
                            }
                            break;
                        case codePoff: // Выключаем
                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            p.StartInfo.FileName = "shutdown.exe";
                            p.StartInfo.Arguments = "-s -t 00";
                            p.Start();
                            socket.Close();
                            break;
                    }
                    // Освобождаем сокеты
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}

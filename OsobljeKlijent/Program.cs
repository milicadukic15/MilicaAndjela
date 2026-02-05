using Domen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OsobljeKlijent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== OSOBLJE KLIJENT (TCP) ===");

            Socket osobljeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                osobljeSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));

                osobljeSocket.Blocking = false;
                Console.WriteLine("Povezan na server. Čekam zadatke...");

                while (true)
                {
                    byte[] buffer = new byte[1024];
                    try
                    {
                        int primljeno = osobljeSocket.Receive(buffer);

                        if (primljeno > 0)
                        {
                            string poruka = Encoding.UTF8.GetString(buffer, 0, primljeno);
                            Console.WriteLine($"\n[STIGAO ZADATAK]: {poruka}");

                            string odgovor = $"POTVRDA_IZVRSENJA|Primljeno: {poruka}";
                            byte[] odgovorBajtovi = Encoding.UTF8.GetBytes(odgovor);
                            osobljeSocket.Send(odgovorBajtovi);
                            Console.WriteLine("[INFO]: Potvrda o izvršenju poslata serveru.");
                        }
                    }
                    catch (SocketException ex)
                    {
                        if (ex.NativeErrorCode != 10035)
                        {
                            throw ex; 
                        }
                    }

                    Thread.Sleep(1000);

                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                        break;
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Greška ili diskonekcija: " + ex.Message);
            }
            finally 
            {
                osobljeSocket.Close();
                Console.WriteLine("Konekcija zatvorena. Pritisnite bilo koji taster...");
                Console.ReadKey();
            }
        }
    }
}

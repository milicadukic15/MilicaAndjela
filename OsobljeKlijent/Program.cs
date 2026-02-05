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
            Console.WriteLine("=== OSOBLJE KLIJENT ===");

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

                            string odgovorServeru = IzvrsiZadatak(poruka);

                            byte[] odgovorBajtovi = Encoding.UTF8.GetBytes(odgovorServeru);
                            osobljeSocket.Send(odgovorBajtovi);
                            Console.WriteLine("[INFO]: Status apartmana ažuriran. Potvrda poslata.");
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

        static string IzvrsiZadatak(string poruka)
        {
            string statusPoruka = "";

            if (poruka.Contains("ZADATAK"))
            {
                Console.WriteLine("Osoblje vrši pripremu sobe i obnovu minibara...");
                Thread.Sleep(2000); 
                statusPoruka = "Soba je očišćena i minibar je dopunjen.";
            }
            else if (poruka.Contains("HITNO") || poruka.Contains("ALARM"))
            {
                Console.WriteLine("Osoblje hitno izlazi na teren radi sanacije alarma...");
                Thread.Sleep(3000);
                statusPoruka = "Alarm saniran, stanje sobe vraćeno u normalu.";
            }
            else if (poruka.Contains("NARUDZBINA"))
            {
                Console.WriteLine("-> Dostava narudžbine u toku...");
                Thread.Sleep(1500);
                statusPoruka = "Narudžbina uspešno dostavljena.";
            }
            else
            {
                statusPoruka = "Zadatak obavljen.";
            }

            return $"POTVRDA_IZVRSENJA|{statusPoruka}";
        }
    }
}

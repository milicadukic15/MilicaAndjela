using Domen;
using Domen.Entiteti;
using Domen.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public class Program
    {

        static Socket udpGosti;
        static Socket tcpOsoblje;
        static List<Socket> povezanoOsoblje = new List<Socket>();
        static List<Apartman> listaApartmana;
        static bool kraj = false;

        static void Main(string[] args)
        {
            Console.WriteLine("=== HOTELSKI SERVER ===");

            // 1. Inicijalizacija podataka
            listaApartmana = InicijalizujHotel();

            // 2. UDP Setup (Gosti)
            udpGosti = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpGosti.Bind(new IPEndPoint(IPAddress.Any, 5000));
            udpGosti.Blocking = false;

            // 3. TCP Setup (Osoblje)
            tcpOsoblje = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpOsoblje.Bind(new IPEndPoint(IPAddress.Any, 6000));
            tcpOsoblje.Listen(5);
            tcpOsoblje.Blocking = false;

            Console.WriteLine("Server sluša: UDP(5000) i TCP(6000). Press ESC to stop.");

            byte[] buffer = new byte[1024];
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                while (!kraj)
                {
                    // Formiranje listi za Select (baš kao u primeru)
                    List<Socket> checkRead = new List<Socket> { udpGosti, tcpOsoblje };
                    checkRead.AddRange(povezanoOsoblje); // Dodajemo sve radnike koji su online

                    List<Socket> checkError = new List<Socket> { udpGosti, tcpOsoblje };
                    checkError.AddRange(povezanoOsoblje);

                    // Čekamo na događaj (timeout 1 sekunda)
                    Socket.Select(checkRead, null, checkError, 1000000);

                    if (checkRead.Count > 0)
                    {
                        foreach (Socket s in checkRead)
                        {
                            if (s == udpGosti) // STIGAO UDP PAKET (GOST)
                            {
                                int brBajta = s.ReceiveFrom(buffer, ref remoteEP);
                                byte[] podaci = new byte[brBajta];
                                Array.Copy(buffer, podaci, brBajta);

                                Gost g = Gost.Deserialize(podaci);
                                Console.WriteLine($"Rezervacija: {g.Ime} {g.Prezime} (Sa: {remoteEP})");
                            }
                            else if (s == tcpOsoblje) // NOVA TCP KONEKCIJA (RADNIK)
                            {
                                Socket noviRadnik = s.Accept();
                                noviRadnik.Blocking = false;
                                povezanoOsoblje.Add(noviRadnik);
                                Console.WriteLine($"Osoblje povezano: {noviRadnik.RemoteEndPoint}");
                            }
                            else // STIGLA PORUKA OD VEĆ POVEZANOG RADNIKA
                            {
                                int brBajta = s.Receive(buffer);
                                if (brBajta == 0) // Radnik se diskonektovao
                                {
                                    Console.WriteLine($"Osoblje otislo: {s.RemoteEndPoint}");
                                    s.Close();
                                    povezanoOsoblje.Remove(s);
                                    break;
                                }
                                else
                                {
                                    // Ovde bi išla deserijalizacija poruke od osoblja
                                    string poruka = Encoding.UTF8.GetString(buffer, 0, brBajta);
                                    Console.WriteLine($"[TCP Poruka]: {poruka}");
                                }
                            }
                        }
                    }

                    // Provera tastature za izlaz (Esc)
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                        kraj = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greska: {ex.Message}");
            }

            ZatvoriSve();
        }

        static List<Apartman> InicijalizujHotel()
        {
            return new List<Apartman>
            {
                new Apartman(101, 3, KlasaApartmana.PRVA, 2),
                new Apartman(102, 2, KlasaApartmana.DRUGA, 4),
                new Apartman(201, 1, KlasaApartmana.TRECA, 5)
            };
        }

        static void ZatvoriSve()
        {
            foreach (Socket s in povezanoOsoblje) s.Close();
            udpGosti.Close();
            tcpOsoblje.Close();
            Console.WriteLine("Server zatvoren.");
        }

    }
}

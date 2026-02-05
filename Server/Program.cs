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
        static Socket udpSocket;
        static Socket tcpSocket;
        static List<Apartman> listaApartmana;

        static void Main(string[] args)
        {
            Console.WriteLine("=== HOTELSKI SISTEM: SERVER ===");


            try 
            { 
                listaApartmana = InicijalizujHotel();
                Console.WriteLine($"[INFO] Hotel inicijalizovan sa {listaApartmana.Count} apartmana.");

                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udpSocket.Bind(new IPEndPoint(IPAddress.Any, 5000)); //udp 5000 gosti

                tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpSocket.Bind(new IPEndPoint(IPAddress.Any, 6000)); //tcp 6000 osoblje
                tcpSocket.Listen(5);

                Console.WriteLine("Server je spreman. Sluša na portovima 5000 (Gosti) i 6000 (Osoblje).");
                Console.WriteLine("---------------------------------");



                // 4. Glavna petlja za prijem poruka (Zadatak 2)
                byte[] buffer = new byte[1024];
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    // ReceiveFrom blokira izvršavanje dok ne stigne UDP paket
                    int primljenoBajtova = udpSocket.ReceiveFrom(buffer, ref remoteEP);

                    if (primljenoBajtova > 0)
                    {
                        // Izdvajamo tačan broj bajtova koji je stigao
                        byte[] primljeniPodaci = new byte[primljenoBajtova];
                        Array.Copy(buffer, primljeniPodaci, primljenoBajtova);

                        try
                        {
                            // DESERIJALIZACIJA (Zadatak 3)
                            // Pretvaramo bajtove nazad u objekat Gost
                            Gost primljeniGost = Gost.Deserialize(primljeniPodaci);

                            Console.WriteLine($"\n[UDP PRIMLJENO] Lokacija: {remoteEP}");
                            Console.WriteLine($"> Gost: {primljeniGost.Ime} {primljeniGost.Prezime}");
                            Console.WriteLine($"> Pasoš: {primljeniGost.BrojPasosa}");
                            Console.WriteLine($"> Datum rođenja: {primljeniGost.DatumRodjenja.ToShortDateString()}");
                            Console.WriteLine("----------------------------------------");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[GREŠKA] Neuspešna deserijalizacija: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FATALNA GREŠKA] Server je stao: {ex.Message}");
                Console.ReadLine();
            }
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

    }
}

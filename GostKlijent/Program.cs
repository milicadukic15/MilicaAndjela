using Domen;
using Domen.Entiteti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GostKlijent
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== GOST KLIJENT ===");

            Socket gostSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5005);
            byte[] buffer = new byte[2048];
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                Console.WriteLine("Tražim spisak slobodnih soba...");
                gostSocket.SendTo(Encoding.UTF8.GetBytes("DOSTUPNO"), serverEP);

                int primljeno = gostSocket.ReceiveFrom(buffer, ref remoteEP);
                string listaSoba = Encoding.UTF8.GetString(buffer, 0, primljeno);
                Console.WriteLine($"[SERVER]:\n{listaSoba}");


                Console.WriteLine("\n--- UNESITE KRITERIJUME ZA REZERVACIJU ---");
                Console.Write("Izaberite klasu (1, 2 ili 3): ");
                string klasa = Console.ReadLine();
                Console.Write("Broj gostiju: ");
                string brGostiju = Console.ReadLine();
                Console.Write("Broj noćenja: ");
                string brNoci = Console.ReadLine();

                string komanda = $"REZERVACIJA_UPIT|{klasa}|{brGostiju}|{brNoci}";
                gostSocket.SendTo(Encoding.UTF8.GetBytes(komanda), serverEP);

                primljeno = gostSocket.ReceiveFrom(buffer, ref remoteEP);
                string odgovor = Encoding.UTF8.GetString(buffer, 0, primljeno);
                Console.WriteLine($"[SERVER ODGOVOR]: {odgovor}");

                if (odgovor.Contains("POTVRDA"))
                {
                    string brSobe = new string(odgovor.Where(char.IsDigit).ToArray());

                    int brojGostiju = int.Parse(brGostiju);
                    for (int i = 0; i < brojGostiju; i++)
                    {
                        Console.WriteLine($"\nUnos podataka za gosta {i + 1}:");
                        Console.Write("Ime: "); string ime = Console.ReadLine();
                        Console.Write("Prezime: "); string prezime = Console.ReadLine();
                        Console.Write("Pol: "); string pol = Console.ReadLine();
                        Console.Write("Broj pasoša: "); string pasos = Console.ReadLine();


                        Gost g = new Gost(ime, prezime, pol, DateTime.Now, pasos);
                        byte[] gostBajtovi = g.Serialize();
                        gostSocket.SendTo(gostBajtovi, serverEP);
                    }

                    Console.WriteLine("\n==============================================");
                    Console.WriteLine("USPEŠNO STE PRIJAVLJENI U SOBU " + brSobe);
                    Console.WriteLine("Komande: 1 (NARUDŽBINA), 2 (ALARM)");
                    Console.WriteLine("==============================================");

                    bool boravakAktivno = true;


                    Thread slusaoc = new Thread(() => {
                        byte[] tempBuffer = new byte[2048];
                        EndPoint rEP = new IPEndPoint(IPAddress.Any, 0);
                        while (boravakAktivno)
                        {
                            try
                            {
                                if (gostSocket.Available > 0)
                                {
                                    int n = gostSocket.ReceiveFrom(tempBuffer, ref rEP);
                                    string notifikacija = Encoding.UTF8.GetString(tempBuffer, 0, n);
                                    Console.WriteLine($"\n[NOTIFIKACIJA]: {notifikacija}");

                                    if (notifikacija.Contains("KRAJ"))
                                    {
                                        Console.WriteLine("\n--- SISTEM: Boravak je istekao. Molimo završite započete radnje. ---");
                                    }
                                }
                            }
                            catch { }
                            Thread.Sleep(500);
                        }
                    });
                    slusaoc.Start();


                    while (boravakAktivno)
                    {
                        if (Console.KeyAvailable)
                        {
                            string izbor = Console.ReadLine();

                            if (izbor == "1")
                            {
                                Console.Write("Šta želite iz minibara? ");
                                string stavka = Console.ReadLine();
                                string poruka = $"NARUDZBINA|{brSobe}|{stavka}";
                                gostSocket.SendTo(Encoding.UTF8.GetBytes(poruka), serverEP);
                                Console.WriteLine(">> Narudžbina poslata.");
                            }
                            else if (izbor == "2")
                            {
                                string poruka = $"ALARM|{brSobe}";
                                gostSocket.SendTo(Encoding.UTF8.GetBytes(poruka), serverEP);
                                Console.WriteLine(">> ALARM AKTIVIRAN! Osoblje je obavešteno.");
                            }
                        }

                        // Provera da li je stigla neka poruka od servera (INFO o danima ili KRAJ - račun)
                        try
                        {
                            if (gostSocket.Available > 0)
                            {
                                primljeno = gostSocket.ReceiveFrom(buffer, ref remoteEP);
                                string notifikacija = Encoding.UTF8.GetString(buffer, 0, primljeno);
                                Console.WriteLine($"\n[NOTIFIKACIJA OD SERVERA]:\n{notifikacija}");

                                if (notifikacija.Contains("KRAJ"))
                                {
                                    Console.WriteLine("\n--- NAPLATA ---");
                                    Console.Write("Unesite broj kartice za potvrdu uplate: ");
                                    Console.ReadLine();
                                    Console.WriteLine("Uplata uspešna. Prijatan put!");
                                    boravakAktivno = false; 
                                }
                            }
                        }
                        catch (SocketException) { }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Greška: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Pritisni bilo koji taster za izlaz...");
                gostSocket.Close();
                Console.ReadKey();
            }

        }
    }
}

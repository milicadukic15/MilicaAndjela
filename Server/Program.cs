using Domen;
using Domen.Entiteti;
using Domen.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Server
{
    internal class Program
    {

        static Hotel mojHotel = new Hotel();
        static List<Socket> klijenti = new List<Socket>();
        static void Main(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            listenSocket.Listen(10);
            listenSocket.Blocking = false;

            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(new IPEndPoint(IPAddress.Any, 5005));
            udpSocket.Blocking = false;

            Console.WriteLine("SERVER POKRENUT...");

            while (true)
            {
                List<Socket> checkRead = new List<Socket>();
                checkRead.Add(listenSocket);
                checkRead.Add(udpSocket);
                foreach (Socket s in klijenti) checkRead.Add(s);

                Socket.Select(checkRead, null, null, 1000000);

                foreach (Socket s in checkRead)
                {
                    if (s == listenSocket) 
                    {
                        Socket noviKlijent = listenSocket.Accept();
                        noviKlijent.Blocking = false;
                        klijenti.Add(noviKlijent);
                        Console.WriteLine($" Osoblje se povezalo.");
                    }
                    else if (s == udpSocket) 
                    {
                        byte[] buffer = new byte[1024];
                        EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        int primljeno = udpSocket.ReceiveFrom(buffer, ref remoteEP);

                        string poruka = Encoding.UTF8.GetString(buffer, 0, primljeno);
                        ObradiGosta(poruka, remoteEP, udpSocket);
                    }
                    else 
                    {
                        try
                        {
                            byte[] buffer = new byte[1024];
                            int primljeno = s.Receive(buffer);
                            if (primljeno <= 0) 
                            {
                                s.Close();
                                klijenti.Remove(s);
                                Console.WriteLine("Osoblje se odjavilo.");
                                break;
                            }
                            string odgovorOsoblja = Encoding.UTF8.GetString(buffer, 0, primljeno);
                            Console.WriteLine($"[POTVRDA OSOBLJA]: {odgovorOsoblja}");
                        }
                        catch
                        {
                            s.Close();
                            klijenti.Remove(s);
                            break;
                        }
                    }
                }
            }
        }

        static void ObradiGosta(string poruka, EndPoint ep, Socket server)
        {
            string[] delovi = poruka.Split('|');
            string komanda = delovi[0];

            if (komanda == "REZERVACIJA_UPIT" && delovi.Length >= 3)
            {
                if (int.TryParse(delovi[1], out int kl) && int.TryParse(delovi[2], out int br))
                {
                    string odgovor = mojHotel.ObradiRezervaciju((KlasaApartmana)kl, br);
                    server.SendTo(Encoding.UTF8.GetBytes(odgovor), ep);
                    Console.WriteLine($"[UDP -> GOST]: {odgovor}");

                    if (odgovor.Contains("POTVRDA"))
                    {
                        string zadatak = "ZADATAK|Pripremiti sobu (ciscenje, minibar)";
                        byte[] zadatakBajtovi = Encoding.UTF8.GetBytes(zadatak);

                        foreach (var radnik in klijenti)
                        {
                            if (radnik.Connected) radnik.Send(zadatakBajtovi);
                        }
                        Console.WriteLine("[OSOBLJE]: Zadatak poslat.");

                        byte[] gostBuffer = new byte[1024];
                        server.ReceiveFrom(gostBuffer, ref ep);
                        Console.WriteLine("[ZADATAK 3]: Primljeni binarni podaci o gostu.");
                    }
                }
            }
            else if (komanda == "ALARM" && delovi.Length >= 2)
            {
                if (int.TryParse(delovi[1], out int brojSobe))
                {
                    mojHotel.AktivirajAlarm(brojSobe);
                    byte[] data = Encoding.UTF8.GetBytes($"HITNO: Alarm u sobi {brojSobe}");
                    foreach (var k in klijenti) k.Send(data);
                }
            }
            else if (komanda == "NARUDZBINA" && delovi.Length >= 3)
            {
                if (int.TryParse(delovi[1], out int brojSobe))
                {
                    mojHotel.RegistrujNarudzbinu(brojSobe, delovi[2]);
                    byte[] data = Encoding.UTF8.GetBytes($"NARUDZBINA: {delovi[2]} za sobu {brojSobe}");
                    foreach (var k in klijenti) k.Send(data);
                }
            }
        }

    }
}

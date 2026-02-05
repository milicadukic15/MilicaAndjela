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
        static List<TcpClient> povezanoOsoblje = new List<TcpClient>();

        static void Main(string[] args)
        {
            TcpListener tcpServer = new TcpListener(IPAddress.Any, 8080);
            tcpServer.Start();
            UdpClient udpServer = new UdpClient(5005);

            Console.WriteLine("SERVER POKRENUT...");

            while (true)
            {
                if (tcpServer.Pending())
                {
                    TcpClient klijent = tcpServer.AcceptTcpClient();
                    povezanoOsoblje.Add(klijent);
                    Console.WriteLine("[TCP] Osoblje povezano.");
                }

                foreach (TcpClient o in povezanoOsoblje.ToList()) 
                {
                    if (o.Connected && o.GetStream().DataAvailable)
                    {
                        byte[] buffer = new byte[1024];
                        int read = o.GetStream().Read(buffer, 0, buffer.Length);
                        string odgovorOsoblja = Encoding.UTF8.GetString(buffer, 0, read);

                        Console.WriteLine($"[OSOBLJE KLIJENT]: {odgovorOsoblja}");

                    }
                }

                if (udpServer.Available > 0)
                {
                    IPEndPoint gostEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpServer.Receive(ref gostEP);
                    string poruka = Encoding.UTF8.GetString(data);

                    ObradiGosta(poruka, gostEP, udpServer);
                }

                System.Threading.Thread.Sleep(10); 
            }
        }

        static void ObradiGosta(string poruka, IPEndPoint ep, UdpClient server)
        {
            if (string.IsNullOrEmpty(poruka)) return;

            string[] delovi = poruka.Split('|');
            string komanda = delovi[0];

            if (delovi.Length < 2)
            {
                Console.WriteLine($"[GREŠKA] Primljena nevalidna poruka: {poruka}");
                return;
            }

            if (komanda == "REZERVACIJA_UPIT" && delovi.Length >= 3)
            {
                if (int.TryParse(delovi[1], out int klasaBroj) && int.TryParse(delovi[2], out int brojGostiju))
                {
                    KlasaApartmana trazenaKlasa = (KlasaApartmana)klasaBroj;
                    string odgovor = mojHotel.ObradiRezervaciju(trazenaKlasa, brojGostiju);
                    byte[] data = Encoding.UTF8.GetBytes(odgovor);
                    server.Send(data, data.Length, ep);
                }
            }
            else if (komanda == "ALARM")
            {
                if (int.TryParse(delovi[1], out int brojSobe))
                {
                    mojHotel.AktivirajAlarm(brojSobe);
                    Console.WriteLine($"!!! ALARM SOBA {brojSobe} !!!");

                    byte[] alert = Encoding.UTF8.GetBytes($"HITNO: Alarm u sobi {brojSobe}");
                    foreach (var o in povezanoOsoblje)
                    {
                        if (o.Connected) o.GetStream().Write(alert, 0, alert.Length);
                    }
                }
            }
            else if (komanda == "NARUDZBINA" && delovi.Length >= 3)
            {
                if (int.TryParse(delovi[1], out int brojSobe))
                {
                    string stavka = delovi[2];
                    mojHotel.RegistrujNarudzbinu(brojSobe, stavka);

                    byte[] potvrdaGostu = Encoding.UTF8.GetBytes($"INFO|Narudžbina '{stavka}' primljena.");
                    server.Send(potvrdaGostu, potvrdaGostu.Length, ep);

                    byte[] zadatakOsoblju = Encoding.UTF8.GetBytes($"ZADATAK: Dostava {stavka} u sobu {brojSobe}");
                    foreach (var o in povezanoOsoblje)
                    {
                        if (o.Connected) o.GetStream().Write(zadatakOsoblju, 0, zadatakOsoblju.Length);
                    }
                }
            }
        }

    }
}

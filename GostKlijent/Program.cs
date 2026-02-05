using Domen;
using Domen.Entiteti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

            try
            {
                Gost noviGost = new Gost("Marko", "Markovic", "Muski", new DateTime(1990, 5, 20), "123456789");

                string komanda = "REZERVACIJA_UPIT|1|1";
                byte[] komandaBajtovi = Encoding.UTF8.GetBytes(komanda);
                gostSocket.SendTo(komandaBajtovi, serverEP);

                Console.WriteLine($"Zahtev za rezervaciju poslat za: {noviGost.Ime} {noviGost.Prezime}");

                byte[] buffer = new byte[1024];
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

                int primljeno = gostSocket.ReceiveFrom(buffer, ref remoteEP);
                string odgovor = Encoding.UTF8.GetString(buffer, 0, primljeno);

                Console.WriteLine($"[SERVER ODGOVOR]: {odgovor}");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Greška: " + ex.Message);
            }

            Console.WriteLine("Pritisni bilo koji taster za izlaz...");
            gostSocket.Close();
            Console.ReadKey();

        }
    }
}

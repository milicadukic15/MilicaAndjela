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

            // 1. Kreiranje UDP utičnice
            Socket klijentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // 2. Definisanje adrese servera (IP adresa tvog kompjutera i port 5000)
            EndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

            try
            {
                // 3. Kreiranje testnog gosta koristeći konstruktor iz Domena
                Gost noviGost = new Gost("Marko", "Markovic", "Muski", new DateTime(1990, 5, 20), "123456789");

                // 4. Serijalizacija (Zadatak 3 u praksi!)
                byte[] podaci = noviGost.Serialize();

                // 5. Slanje podataka serveru
                klijentSocket.SendTo(podaci, 0, podaci.Length, SocketFlags.None, serverEP);

                Console.WriteLine($"Zahtev za rezervaciju poslat za: {noviGost.Ime} {noviGost.Prezime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greška: " + ex.Message);
            }

            Console.WriteLine("Pritisni bilo koji taster za izlaz...");
            klijentSocket.Close();
            Console.ReadKey();

        }
    }
}

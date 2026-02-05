using Domen.Entiteti;
using Domen.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Hotel
    {
        public List<Apartman> Apartmani { get; set; } = new List<Apartman>();
        public List<string> ListaZadatakaZaOsoblje { get; set; } = new List<string>();

        public Hotel()
        {
            Apartmani.Add(new Apartman { BrojApartmana = 101, Sprat = 1, Klasa = KlasaApartmana.PRVA, MaxBrojGostiju = 2 });
            Apartmani.Add(new Apartman { BrojApartmana = 202, Sprat = 2, Klasa = KlasaApartmana.DRUGA, MaxBrojGostiju = 4 });
            Apartmani.Add(new Apartman { BrojApartmana = 303, Sprat = 3, Klasa = KlasaApartmana.TRECA, MaxBrojGostiju = 1});

            Apartmani[2].Stanje = StanjeApartmana.POTREBNO_CISCENJE;
            ListaZadatakaZaOsoblje.Add("Soba 303: Potrebno generalno čišćenje.");
        }

        public string ProveriDostupnost()
        {
            List<Apartman> slobodne = Apartmani.Where(a => a.Stanje == StanjeApartmana.PRAZAN).ToList();
            if (slobodne.Count == 0) return "Trenutno nema slobodnih soba.";

            string odgovor = "Slobodne sobe:\n";
            foreach (Apartman ap in slobodne)
            {
                odgovor += $"- Soba {ap.BrojApartmana}, Klasa {ap.Klasa}, Max osoba: {ap.MaxBrojGostiju}\n";
            }
            return odgovor;
        }

        public string ObradiRezervaciju(KlasaApartmana klasa, int brojGostiju)
        {
            Apartman ap = Apartmani.FirstOrDefault(a => a.Stanje == StanjeApartmana.PRAZAN
                                              && a.Klasa == klasa
                                              && a.MaxBrojGostiju >= brojGostiju);
            if (ap != null)
            {
                ap.Stanje = StanjeApartmana.ZAUZET;
                return $"POTVRDA - Rezervisana soba {ap.BrojApartmana}";
            }
            return "ODBIJENO - Nema slobodnih soba tražene klase.";
        }

        public void AktivirajAlarm(int brojSobe)
        {
            Apartman ap = Apartmani.FirstOrDefault(a => a.BrojApartmana == brojSobe);
            if (ap != null)
            {
                ap.Alarm = StanjeAlarma.AKTIVIRAN;
                ListaZadatakaZaOsoblje.Add($"HITNO: Alarm u sobi {brojSobe}!");
                Console.WriteLine($"Alarm aktiviran u sobi {brojSobe}. Zadatak prosleđen osoblju.");
            }
        }

        public void RegistrujNarudzbinu(int brojSobe, string stavka)
        {
            Apartman ap = Apartmani.FirstOrDefault(a => a.BrojApartmana == brojSobe);
            if (ap != null)
            {
                ListaZadatakaZaOsoblje.Add($"NARUDŽBINA: Soba {brojSobe} traži {stavka}");
                Console.WriteLine($"Narudžbina '{stavka}' zabeležena za sobu {brojSobe}.");
            }
        }
    }
}

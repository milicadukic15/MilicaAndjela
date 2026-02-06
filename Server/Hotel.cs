using Domen.Entiteti;
using Domen.Enumeracije;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class AktivniBoravak
    {
        public int BrojSobe { get; set; }
        public int PreostaloDana { get; set; }
        public double Racun { get; set; }
        public System.Net.EndPoint GostEP { get; set; }
    }
    public class Hotel
    {
        public List<Apartman> Apartmani { get; set; } = new List<Apartman>();
        public List<string> ListaZadatakaZaOsoblje { get; set; } = new List<string>();
        public List<AktivniBoravak> Boravci { get; set; } = new List<AktivniBoravak>();

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

        public string ObradiRezervaciju(int klasa, int brojGostiju, int brojNoci, System.Net.EndPoint ep)
        {
            Apartman ap = Apartmani.FirstOrDefault(a => a.Stanje == StanjeApartmana.PRAZAN
                                              && (int)a.Klasa == klasa
                                              && a.MaxBrojGostiju >= brojGostiju);
            if (ap != null)
            {
                ap.Stanje = StanjeApartmana.ZAUZET;
                Boravci.Add(new AktivniBoravak
                {
                    BrojSobe = ap.BrojApartmana,
                    PreostaloDana = brojNoci,
                    Racun = brojNoci * (50 * (int)ap.Klasa),
                    GostEP = ep
                });
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

                AktivniBoravak boravak = Boravci.FirstOrDefault(b => b.BrojSobe == brojSobe);
                if (boravak != null)
                {
                    boravak.Racun += 50.0; 
                    Console.WriteLine($"[RAČUN]: Sobi {brojSobe} dodato 50 EUR za alarm. Ukupno: {boravak.Racun}");
                }

                ListaZadatakaZaOsoblje.Add($"HITNO: Alarm u sobi {brojSobe}!");
                Console.WriteLine($"Alarm aktiviran u sobi {brojSobe}. Zadatak prosleđen osoblju.");
            }
        }

        public void RegistrujNarudzbinu(int brojSobe, string stavka)
        {
            Apartman ap = Apartmani.FirstOrDefault(a => a.BrojApartmana == brojSobe);
            if (ap != null)
            {
                AktivniBoravak boravak = Boravci.FirstOrDefault(b => b.BrojSobe == brojSobe);
                if (boravak != null)
                {
                    boravak.Racun += 15.0; 
                    Console.WriteLine($"[RAČUN]: Sobi {brojSobe} dodato 15 EUR za {stavka}. Ukupno: {boravak.Racun}");
                }

                ListaZadatakaZaOsoblje.Add($"NARUDŽBINA: Soba {brojSobe} traži {stavka}");
                Console.WriteLine($"Narudžbina '{stavka}' zabeležena za sobu {brojSobe}.");
            }
        }
    }
}

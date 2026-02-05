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

        public Hotel()
        {
            Apartmani.Add(new Apartman { BrojApartmana = 101, Klasa = KlasaApartmana.PRVA, MaxBrojGostiju = 2, Stanje = StanjeApartmana.PRAZAN });
            Apartmani.Add(new Apartman { BrojApartmana = 202, Klasa = KlasaApartmana.DRUGA, MaxBrojGostiju = 4, Stanje = StanjeApartmana.POTREBNO_CISCENJE });
            Apartmani.Add(new Apartman { BrojApartmana = 303, Klasa = KlasaApartmana.TRECA, MaxBrojGostiju = 1, Stanje = StanjeApartmana.ZAUZET });
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
            if (ap != null) ap.Alarm = StanjeAlarma.AKTIVIRAN;
        }

        public void RegistrujNarudzbinu(int brojSobe, string stavka)
        {
            Apartman ap = Apartmani.FirstOrDefault(a => a.BrojApartmana == brojSobe);
            if (ap != null)
            {
                Console.WriteLine($"Narudžbina '{stavka}' zabeležena za sobu {brojSobe}.");
            }
        }
    }
}

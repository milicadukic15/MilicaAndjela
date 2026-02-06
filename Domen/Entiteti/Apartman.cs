using Domen.Enumeracije;
using Domen.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entiteti
{
    [Serializable]
    public class Apartman : IMreznaPoruka
    {
        public int BrojApartmana { get; set; }
        public int Sprat { get; set; }
        public KlasaApartmana Klasa { get; set; } 
        public int MaxBrojGostiju { get; set; }
        public int TrenutniBrojGostiju { get; set; }
        public StanjeApartmana Stanje { get; set; }
        public StanjeAlarma Alarm { get; set; }
        public List<Gost> ListaGostiju { get; set; } = new List<Gost>();

        public double TrenutniRacun { get; set; } = 0;
        public int PreostaloNoci { get; set; }
        public int UkupnoNocenja { get; set; }

        public Apartman()
        {
            ListaGostiju = new List<Gost>();
        }
        public Apartman(int broj, int sprat, KlasaApartmana klasa, int maxGostiju)
        {
            BrojApartmana = broj;
            Sprat = sprat;
            Klasa = klasa;
            MaxBrojGostiju = maxGostiju;
            TrenutniBrojGostiju = 0;
            Stanje = StanjeApartmana.PRAZAN;
            Alarm = StanjeAlarma.NORMALNO;
            ListaGostiju = new List<Gost>();
        }

        public double CenaPoNoci()
        {
            if (Klasa == KlasaApartmana.PRVA) return 150;
            if (Klasa == KlasaApartmana.DRUGA) return 100;
            return 60;
        }
        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public static Apartman Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (Apartman)bf.Deserialize(ms);
            }
        }
    }
}

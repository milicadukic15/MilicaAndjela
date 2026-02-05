using Domen.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entiteti
{
    [Serializable]
    public class Osoblje : IMreznaPoruka
    {
        public string ID { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Pol { get; set; }
        public string Funkcija { get; set; }

        public Osoblje() { }
        public Osoblje(string id, string ime, string prezime, string pol, string funkcija)
        {
            ID = id;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Funkcija = funkcija;
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
        public static Osoblje Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (Osoblje)bf.Deserialize(ms);
            }
        }
    }
}

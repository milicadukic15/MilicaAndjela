using Domen.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entiteti
{
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
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(ID);
                    writer.Write(Ime);
                    writer.Write(Prezime);
                    writer.Write(Pol);
                    writer.Write(Funkcija);
                    return ms.ToArray();
                }
            }
        }
        public static Osoblje Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    return new Osoblje
                    {
                        ID = reader.ReadString(),
                        Ime = reader.ReadString(),
                        Prezime = reader.ReadString(),
                        Pol = reader.ReadString(),
                        Funkcija = reader.ReadString()
                    };
                }
            }
        }
    }
}

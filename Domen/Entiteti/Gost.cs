using Domen.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entiteti
{
    public class Gost : IMreznaPoruka
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Pol { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string BrojPasosa { get; set; }

        public Gost() { }
        public Gost(string ime, string prezime, string pol, DateTime datumRodjenja, string brojPasosa)
        {
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            DatumRodjenja = datumRodjenja;
            BrojPasosa = brojPasosa;
        }

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(Ime);
                    writer.Write(Prezime);
                    writer.Write(Pol);
                    writer.Write(DatumRodjenja.ToBinary());
                    writer.Write(BrojPasosa);
                    return ms.ToArray();
                }
            }
        }
        public static Gost Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    return new Gost
                    {
                        Ime = reader.ReadString(),
                        Prezime = reader.ReadString(),
                        Pol = reader.ReadString(),
                        DatumRodjenja = DateTime.FromBinary(reader.ReadInt64()),
                        BrojPasosa = reader.ReadString()
                    };
                }
            }
        }
    }
}

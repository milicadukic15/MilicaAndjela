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
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, this); 
                return ms.ToArray();
            }
        }
        public static Gost Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (Gost)bf.Deserialize(ms);
            }
        }
    }
}

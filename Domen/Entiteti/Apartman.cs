using Domen.Enumeracije;
using Domen.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entiteti
{
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

        public Apartman()
        {
            ListaGostiju = new List<Gost>();
            Stanje = StanjeApartmana.PRAZAN;
            Alarm = StanjeAlarma.NORMALNO;
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

        public byte[] Serialize()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(BrojApartmana);
                    writer.Write(Sprat);
                    writer.Write((int)Klasa);
                    writer.Write(MaxBrojGostiju);
                    writer.Write(TrenutniBrojGostiju);
                    writer.Write((int)Stanje);
                    writer.Write((int)Alarm);

                    writer.Write(ListaGostiju.Count);
                    foreach (Gost gost in ListaGostiju)
                    {
                        byte[] gostBytes = gost.Serialize();
                        writer.Write(gostBytes.Length);
                        writer.Write(gostBytes);
                    }
                    return ms.ToArray();
                }
            }
        }

        public static Apartman Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    Apartman apartman = new Apartman
                    {
                        BrojApartmana = reader.ReadInt32(),
                        Sprat = reader.ReadInt32(),
                        Klasa = (KlasaApartmana)reader.ReadInt32(),
                        MaxBrojGostiju = reader.ReadInt32(),
                        TrenutniBrojGostiju = reader.ReadInt32(),
                        Stanje = (StanjeApartmana)reader.ReadInt32(),
                        Alarm = (StanjeAlarma)reader.ReadInt32()
                    };

                    int brojGostiju = reader.ReadInt32();
                    for (int i = 0; i < brojGostiju; i++)
                    {
                        int duzinaGosta = reader.ReadInt32();
                        byte[] gostData = reader.ReadBytes(duzinaGosta);
                        apartman.ListaGostiju.Add(Gost.Deserialize(gostData));
                    }
                    return apartman;
                }
            }
        }
    }
}

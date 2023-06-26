using Newtonsoft.Json;
using ReversiRestApi.Models.Database;
using ReversiRestApi.Models.Enumerations;
using ReversiRestApi.Models.Interfaces;

namespace ReversiRestApi.Models
{
    public class Spel : ISpel
    {
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public string Speler1Token { get; set; }
        public string Speler2Token { get; set; }
        public Kleur[,] Bord { get; set; }
        public Kleur AandeBeurt { get; set; }
        public Kleur Winnaar { get; set; }

        public Spel()
        {
            Token = Guid.NewGuid().ToString();
            Bord = new Kleur[8, 8];
            Bord[3, 3] = Kleur.Wit;
            Bord[3, 4] = Kleur.Zwart;
            Bord[4, 4] = Kleur.Wit;
            Bord[4, 3] = Kleur.Zwart;
            AandeBeurt = Kleur.Wit;
            Winnaar = Kleur.Geen;
        }

        public Spel(SpelDatabase spel)
        {
            ID = spel.ID;
            Omschrijving = spel.Omschrijving;
            Token = spel.Token;
            Speler1Token = spel.Speler1Token;
            Speler2Token = spel.Speler2Token;
            Bord = JsonConvert.DeserializeObject<Kleur[,]>(spel.Bord);
            AandeBeurt = spel.AandeBeurt;
            Winnaar = spel.Winnaar;
        }

        public bool Afgelopen()
        {
            if (!Pas())
                return false;

            Kleur tegenstander = AandeBeurt == Kleur.Wit ? Kleur.Zwart : Kleur.Wit;
            AandeBeurt = tegenstander;

            if (!Pas())
                return false;

            return true;
        }

        public bool DoeZet(int rijZet, int kolomZet)
        {
            if (!ZetMogelijk(rijZet, kolomZet))
                return false;

            Bord[rijZet, kolomZet] = AandeBeurt;

            foreach ((int, int) piece in PiecesToFlipOver(rijZet, kolomZet))
                Bord[piece.Item1, piece.Item2] = AandeBeurt;

            Kleur tegenstander = AandeBeurt == Kleur.Wit ? Kleur.Zwart : Kleur.Wit;
            AandeBeurt = tegenstander;

            if (Afgelopen())
                Winnaar = OverwegendeKleur();

            return true;
        }

        public Kleur OverwegendeKleur()
        {
            int countZwart = 0;
            int countWit = 0;

            for (int y = 0; y < Bord.GetLength(0); y++)
            {
                for (int x = 0; x < Bord.GetLength(1); x++)
                {
                    if (Bord[y, x] == Kleur.Wit)
                        countWit += 1;
                    else if (Bord[y, x] == Kleur.Zwart)
                        countZwart += 1;
                }
            }

            if (countZwart == countWit)
                return Kleur.Geen;

            return countZwart > countWit ? Kleur.Zwart : Kleur.Wit;
        }

        public bool Pas()
        {
            for (int i = 0; i < Bord.GetLength(0); i++)
            {
                for (int j = 0; j < Bord.GetLength(1); j++)
                {
                    if (ZetMogelijk(i, j))
                        return false;
                }
            }

            Kleur tegenstander = AandeBeurt == Kleur.Wit ? Kleur.Zwart : Kleur.Wit;
            AandeBeurt = tegenstander;
            return true;
        }

        public bool ZetMogelijk(int rijZet, int kolomZet)
        {
            if (rijZet > 7 || rijZet < 0 || kolomZet > 7 || kolomZet < 0)
                return false;

            if (Bord[rijZet, kolomZet] != 0)
                return false;

            if (PiecesToFlipOver(rijZet, kolomZet).Count == 0)
                return false;

            return true;
        }

        public string GetSpelerTokenDieAanDeBeurtIs()
        {
            return AandeBeurt == Kleur.Wit ? Speler1Token : Speler2Token;
        }

        public void Opgeven(string spelerToken)
        {
            Winnaar = spelerToken == Speler1Token ? Kleur.Zwart : Kleur.Wit;
        }

        private List<(int, int)> PiecesToFlipOver(int rijZet, int kolomZet)
        {
            List<(int, int)> piecesOfEnemyToFlipOver = new();
            Kleur tegenstander = AandeBeurt == Kleur.Wit ? Kleur.Zwart : Kleur.Wit;
            List<int> possibleSides = new();

            try
            {
                if (Bord[rijZet - 1, kolomZet] == tegenstander)
                    possibleSides.Add(0);
            }
            catch (Exception) { }

            try
            {
                if (Bord[rijZet - 1, kolomZet + 1] == tegenstander)
                    possibleSides.Add(1);
            }
            catch (Exception) { }

            try
            {
                if (Bord[rijZet, kolomZet + 1] == tegenstander)
                    possibleSides.Add(2);
            }
            catch (Exception) { }

            try
            {
                if (Bord[rijZet + 1, kolomZet + 1] == tegenstander)
                    possibleSides.Add(3);
            }
            catch (Exception) { }

            try
            {
                if (Bord[rijZet + 1, kolomZet] == tegenstander)
                    possibleSides.Add(4);
            }
            catch (Exception) { }

            try
            {
                if (Bord[rijZet + 1, kolomZet - 1] == tegenstander)
                    possibleSides.Add(5);
            }
            catch (Exception) { }

            try
            {
                if (Bord[rijZet, kolomZet - 1] == tegenstander)
                    possibleSides.Add(6);
            }
            catch (Exception) { }

            try
            {
                if (Bord[rijZet - 1, kolomZet - 1] == tegenstander)
                    possibleSides.Add(7);
            }
            catch (Exception) { }

            if (possibleSides.Count == 0)
                return piecesOfEnemyToFlipOver;

            foreach (int side in possibleSides)
            {
                List<(int, int)> list = new();
                switch (side)
                {
                    case 0:
                        list = CheckRow(rijZet, kolomZet, -1, 0);
                        break;

                    case 1:
                        list = CheckRow(rijZet, kolomZet, -1, 1);
                        break;

                    case 2:
                        list = CheckRow(rijZet, kolomZet, 0, 1);
                        break;

                    case 3:
                        list = CheckRow(rijZet, kolomZet, 1, 1);
                        break;

                    case 4:
                        list = CheckRow(rijZet, kolomZet, 1, 0);
                        break;

                    case 5:
                        list = CheckRow(rijZet, kolomZet, 1, -1);
                        break;

                    case 6:
                        list = CheckRow(rijZet, kolomZet, 0, -1);
                        break;

                    case 7:
                        list = CheckRow(rijZet, kolomZet, -1, -1);
                        break;

                    default:
                        break;
                }
                piecesOfEnemyToFlipOver.AddRange(list);
            }

            return piecesOfEnemyToFlipOver;
        }

        private List<(int, int)> CheckRow(int rijZet, int kolomZet, int funcRij, int funcKolom)
        {
            int count = 0;
            List<(int, int)> tegenstanderPieces = new List<(int, int)>();
            int tempRij = rijZet + funcRij;
            int tempKolom = kolomZet + funcKolom;

            while (tempRij > -1 && tempRij < 8 && tempKolom > -1 && tempKolom < 8)
            {
                if (Bord[tempRij, tempKolom] == AandeBeurt)
                {
                    count = 1;
                    break;
                }
                else if (Bord[tempRij, tempKolom] == Kleur.Geen)
                    break;
                else
                    tegenstanderPieces.Add((tempRij, tempKolom));

                tempRij += funcRij;
                tempKolom += funcKolom;
            }

            if (count == 1)
                return tegenstanderPieces;
            else
                return new List<(int, int)>();
        }

        public override bool Equals(object? obj)
        {
            // If the passed object is null, return False
            if (obj == null)
            {
                return false;
            }
            // If the passed object is not Customer Type, return False
            if (!(obj is Spel))
            {
                return false;
            }

            return Token.Equals(((Spel)obj).Token);
        }
    }
}
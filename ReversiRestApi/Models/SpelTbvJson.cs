using Newtonsoft.Json;
using ReversiRestApi.Models.Enumerations;

namespace ReversiRestApi.Models
{
    public class SpelTbvJson
    {
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public string Speler1Token { get; set; }
        public string Speler2Token { get; set; }
        public string Bord { get; set; }
        public Kleur AandeBeurt { get; set; }
        public Kleur Winnaar { get; set; }

        public SpelTbvJson(Spel spel)
        {
            ID = spel.ID;
            Omschrijving = spel.Omschrijving;
            Token = spel.Token;
            Speler1Token = spel.Speler1Token;
            Speler2Token = spel.Speler2Token;
            Bord = JsonConvert.SerializeObject(spel.Bord);
            AandeBeurt = spel.AandeBeurt;
            Winnaar = spel.Winnaar;
        }

        public SpelTbvJson() { }
    }
}

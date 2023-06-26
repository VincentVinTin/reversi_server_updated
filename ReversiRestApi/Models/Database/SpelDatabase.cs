using ReversiRestApi.Models.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ReversiRestApi.Models.Database
{
    public class SpelDatabase
    {
        [Key]
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public string Speler1Token { get; set; }
        public string Speler2Token { get; set; }
        public string Bord { get; set; }
        public Kleur AandeBeurt { get; set; }
        public Kleur Winnaar { get; set; }
    }
}

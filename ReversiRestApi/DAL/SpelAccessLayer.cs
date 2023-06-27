using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReversiRestApi.Models;
using ReversiRestApi.Models.Database;
using ReversiRestApi.Models.Interfaces;

namespace ReversiRestApi.DAL
{
    public class SpelAccessLayer : ISpelRepository
    {
        public DatabaseContext Context { get; }
        private DbSet<SpelDatabase> _spellen { get; }

        public SpelAccessLayer(DatabaseContext context)
        {
            Context = context;
            _spellen = context.Spellen;
        }

        public async Task AddSpel(Spel spel)
        {
            Context.Add(new SpelDatabase()
            {
                ID = spel.ID,
                Omschrijving = spel.Omschrijving,
                Token = spel.Token,
                Speler1Token = spel.Speler1Token,
                Speler2Token = spel.Speler2Token,
                Bord = JsonConvert.SerializeObject(spel.Bord),
                AandeBeurt = spel.AandeBeurt,
                Winnaar = spel.Winnaar
            });
            await Context.SaveChangesAsync();
        }

        public async Task<Spel?> GetSpel(string spelToken)
        {
            try
            {
                return new Spel(await _spellen.FirstAsync(x => x.Token == spelToken));
            }
            catch
            {
                return null;
            }
        }

        public List<Spel> GetSpellen()
        {
            return _spellen.Select(s => new Spel(s)).ToList();
        }

        public async Task UpdateSpel(Spel spel)
        {
            SpelDatabase spelDatabase = _spellen.Where(s => s.Token == spel.Token).First();

            spelDatabase.Speler2Token = spel.Speler2Token;
            spelDatabase.Bord = JsonConvert.SerializeObject(spel.Bord);
            spelDatabase.AandeBeurt = spel.AandeBeurt;
            spelDatabase.Winnaar = spel.Winnaar;

            Context.Update(spelDatabase);
            await Context.SaveChangesAsync();
        }
    }
}
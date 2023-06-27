using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ReversiMvcApp.Data;
using ReversiMvcApp.Models;

namespace ReversiMvcApp.Controllers
{
    public class SpelerController : Controller
    {
        private readonly ReversiDbContext _context;

        public SpelerController(ReversiDbContext context)
        {
            _context = context;
        }

        public Speler GetSpelerIngelogd(Controller controller)
        {
            ClaimsPrincipal currentUser = controller.User;
            var currentUserID = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            if (SpelerExists(currentUserID))
                return GetSpeler(currentUserID)!;
            else
                return CreateSpeler(currentUserID, currentUser.FindFirst(ClaimTypes.Name)?.Value ?? "");
        }

        public Speler CreateSpeler(string guid, string naam)
        {
            Speler speler = new()
            {
                Guid = guid,
                Naam = naam,
                AantalGewonnen = 0,
                AantalVerloren = 0,
                AantalGelijk = 0
            };
            _context.Spelers.Add(speler);
            _context.SaveChangesAsync();
            return speler;
        }

        private bool SpelerExists(string guid)
        {
            return _context.Spelers.Any(e => e.Guid == guid);
        }

        public Speler? GetSpeler(string guid)
        {
            return _context.Spelers.FirstOrDefault(s => s.Guid == guid);
        }
    }
}
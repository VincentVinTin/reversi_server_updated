using Microsoft.AspNetCore.Mvc;
using ReversiRestApi.Models;
using ReversiRestApi.Models.Interfaces;

namespace ReversiRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpelController : ControllerBase
    {
        private readonly ISpelRepository iRepository;

        public SpelController(ISpelRepository repository)
        {
            iRepository = repository;
        }

        // GET api/spel
        [HttpGet]
        public ActionResult<IEnumerable<SpelTbvJson>> GetSpellen()
        {
            var spellen = iRepository.GetSpellen();
            return spellen.Select(s => new SpelTbvJson(s)).ToList();
        }

        // GET api/spel/wachtend
        [HttpGet("wachtend")]
        public ActionResult<IEnumerable<SpelTbvJson>> GetSpellenMetWachtendeSpelerAsync()
        {
            var spellen = iRepository.GetSpellen();
            return spellen.Where(s => s.Speler2Token == null).Select(s => new SpelTbvJson(s)).ToList();
        }

        // POST api/spel
        [HttpPost]
        public async Task<SpelTbvJson> PostSpelAsync([FromBody] SpelInfoTbvApi spelInfo)
        {
            Spel spel = new()
            {
                Speler1Token = spelInfo.Speler1Token,
                Omschrijving = spelInfo.Omschrijving
            };

            await iRepository.AddSpel(spel);
            return new SpelTbvJson(spel);
        }

        // GET api/spel/{token}
        [HttpGet("{token}")]
        public async Task<ActionResult<SpelTbvJson>> GetSpelByTokenAsync(string token)
        {
            Spel? spel = await iRepository.GetSpel(token);

            if (spel is null)
                return NotFound("Kan spel niet vinden");

            return new SpelTbvJson(spel);
        }

        // PUT api/spel/{token}/join
        [HttpPut("{token}/join")]
        public async Task<ActionResult<SpelTbvJson>> PutJoinSpelAsync(string token, [FromBody] JoinSpeler body)
        {
            Spel? spel = await iRepository.GetSpel(token);

            if (spel is null)
                return NotFound("Kan spel niet vinden");

            if (spel.Speler1Token == body.SpelerToken || spel.Speler2Token != null)
                return Unauthorized();

            spel.Speler2Token = body.SpelerToken;
            await iRepository.UpdateSpel(spel);

            return new SpelTbvJson(spel);
        }

        // PUT api/spel/{token}/beurt
        [HttpGet("{token}/beurt")]
        public async Task<ActionResult<string>> GetBeurtOfSpelAsync(string token)
        {
            Spel? spel = await iRepository.GetSpel(token);

            if (spel is null)
                return NotFound("Kan spel niet vinden");

            return spel.GetSpelerTokenDieAanDeBeurtIs();
        }

        // PUT api/spel/{token}/zet/{spelerToken}
        [HttpPut("{token}/zet/{spelerToken}")]
        public async Task<ActionResult<SpelTbvJson>> PutDoeZetAsync(string token, string spelerToken, [FromBody] SpelZet body)
        {
            Spel? spel = await iRepository.GetSpel(token);

            if (spel is null)
                return NotFound("Kan spel niet vinden");

            if (spel.GetSpelerTokenDieAanDeBeurtIs() != spelerToken)
                return Unauthorized("Speler is niet aan de beurt");

            bool isAllowed = spel.DoeZet(body.Rij, body.Kolom);
            if (!isAllowed)
                return Unauthorized("Geen geldige zet");

            await iRepository.UpdateSpel(spel);
            return new SpelTbvJson(spel);
        }

        // PUT api/spel/{token}/pas/{spelerToken}
        [HttpPut("{token}/pas/{spelerToken}")]
        public async Task<ActionResult<SpelTbvJson>> PutPasAsync(string token, string spelerToken)
        {
            Spel? spel = await iRepository.GetSpel(token);

            if (spel is null)
                return NotFound("Kan spel niet vinden");

            if (spel.GetSpelerTokenDieAanDeBeurtIs() != spelerToken)
                return Unauthorized("Speler is niet aan de beurt");

            bool isAllowed = spel.Pas();
            if (!isAllowed)
                return Unauthorized("Er is een zet mogelijk");

            await iRepository.UpdateSpel(spel);
            return new SpelTbvJson(spel);
        }

        // PUT api/spel/{token}/opgeven/{spelerToken}
        [HttpPut("{token}/opgeven/{spelerToken}")]
        public async Task<ActionResult<SpelTbvJson>> PutOpgevenAsync(string token, string spelerToken)
        {
            Spel? spel = await iRepository.GetSpel(token);

            if (spel is null)
                return NotFound("Kan spel niet vinden");

            spel.Opgeven(spelerToken);

            await iRepository.UpdateSpel(spel);
            return new SpelTbvJson(spel);
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReversiMvcApp.Data;
using ReversiRestApi.Models;

namespace ReversiMvcApp.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class SpelController : Controller
    {
        private readonly ILogger<SpelController> _logger;
        private readonly SpelerController _spelerController;
        private readonly ApiController _apiController;

        public SpelController(ILogger<SpelController> logger, SpelerController spelerController, ApiController apiController)
        {
            _logger = logger;
            _spelerController = spelerController;
            _apiController = apiController;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var spellen = await _apiController.GetListAsync<SpelTbvJson>("spel/wachtend");

            return View(spellen);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(string omschrijving)
        {
            var spelerToken = _spelerController.GetSpelerIngelogd(this).Guid;
            var response = await _apiController.PostAsync("spel", new SpelTbvJson()
            {
                Speler1Token = spelerToken,
                Omschrijving = omschrijving,
            });

            return RedirectToAction(nameof(Spel), new { token = response.Token });
        }

        [HttpGet("Join")]
        public async Task<IActionResult> Join(string spelToken)
        {
            var spelerToken = _spelerController.GetSpelerIngelogd(this).Guid;
            var response = await _apiController.PutAsync($"spel/{spelToken}/join",
                    new JoinSpeler() { SpelerToken = spelerToken });

            return RedirectToAction(nameof(Spel), new { token = spelToken });
        }

        [HttpGet("Get/{token}")]
        public async Task<IActionResult> Spel(string token)
        {
            var response = await _apiController.GetAsync<SpelTbvJson>($"spel/{token}/");

            return View(response);
        }
    }
}
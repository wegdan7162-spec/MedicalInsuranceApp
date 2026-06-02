using MedicalInsuranceApp1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInsuranceApp1.Controllers
{
    [AllowAnonymous]
    public class MigrationController : Controller
    {
        private readonly UserMigrationService _migrationService;

        public MigrationController(UserMigrationService migrationService)
            => _migrationService = migrationService;

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> RunMigration()
        {
            var result = await _migrationService.MigrateUsersAsync();
            return View("Result", result);
        }
    }
}
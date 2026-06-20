//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace MedicalInsuranceApp1.Controllers
//{
//    [Authorize]
//    public class CourtCasesController : Controller
//    {
//        public IActionResult Index(string? search, int? branchId, string? status, int? year, int page = 1)
//            => RedirectToAction("Index", "OutterClaims", new { search, branchId, status, year, page });

//        public IActionResult Details(int id)
//            => RedirectToAction("Details", "OutterClaims", new { id });

//        public IActionResult Create()
//            => RedirectToAction("Create", "OutterClaims");

//        public IActionResult Edit(int id)
//            => RedirectToAction("Edit", "OutterClaims", new { id });
//    }
//}

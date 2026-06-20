using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.InsuredEmployees)]
    public class InsuredEmployeesController : Controller
    {
        private readonly AppDbContext _db;
        public InsuredEmployeesController(AppDbContext db) => _db = db;

        // ─────────────────────────────────────────────────────────
        // قائمة الموظفين المؤمن عليهم — اختيارياً مفلترة بعقد معين
        // ─────────────────────────────────────────────────────────
        public async Task<IActionResult> Index(int? contractId, string? search)
        {
            var query = _db.InsuredEmployees
                .Include(x => x.Contract)
                .Where(x => x.Del != true);

            if (contractId.HasValue)
                query = query.Where(x => x.ContractId == contractId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.FullName.Contains(search) ||
                    (x.Specialization != null && x.Specialization.Contains(search)));

            var list = await query.OrderBy(x => x.ContractId).ThenBy(x => x.FullName).ToListAsync();

            ViewBag.ContractId = contractId;
            ViewBag.Search     = search;
            ViewBag.Contracts  = await _db.InsuranceContracts
                                          .Where(x => x.Del != true)
                                          .OrderBy(x => x.FacilityName).ToListAsync();

            // إحصائيات الكشف
            ViewBag.TotalEmployeeShare = list.Sum(x => x.EmployeeShare);
            ViewBag.TotalEntityShare   = list.Sum(x => x.EntityShare);
            ViewBag.TotalPremium       = list.Sum(x => x.TotalPremium);
            ViewBag.Count              = list.Count;

            return View(list);
        }

        // ─────────────────────────────────────────────────────────
        // إضافة موظف واحد
        // ─────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Create(int? contractId)
        {
            await LoadViewBags();
            var model = new InsuredEmployee
            {
                ContractId   = contractId ?? 0,
                EntityRate   = 0.03m,
                EmployeeRate = 0.02m
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InsuredEmployee model)
        {
            CalculateShares(model);
            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;
            _db.InsuredEmployees.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة الموظف المؤمن عليه";
            return RedirectToAction("Index", new { contractId = model.ContractId });
        }

        // ─────────────────────────────────────────────────────────
        // تعديل
        // ─────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.InsuredEmployees.FindAsync(id);
            if (item == null) return NotFound();
            await LoadViewBags();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InsuredEmployee model)
        {
            var item = await _db.InsuredEmployees.FindAsync(id);
            if (item == null) return NotFound();

            item.FullName       = model.FullName;
            item.Specialization = model.Specialization;
            item.Phone          = model.Phone;
            item.MonthlySalary  = model.MonthlySalary;
            item.EntityRate     = model.EntityRate;
            item.EmployeeRate   = model.EmployeeRate;
            CalculateShares(item);

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث بيانات الموظف";
            return RedirectToAction("Index", new { contractId = item.ContractId });
        }

        // ─────────────────────────────────────────────────────────
        // حذف ناعم
        // ─────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.InsuredEmployees.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف السجل";
            return RedirectToAction("Index");
        }

        // ─────────────────────────────────────────────────────────
        // API: إعادة حساب الحصص عند تغيير الراتب أو النسب (AJAX)
        // ─────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult CalcShares(decimal salary, decimal entityRate = 0.03m, decimal empRate = 0.02m)
        {
            var entityShare   = Math.Round(salary * entityRate, 3);
            var employeeShare = Math.Round(salary * empRate, 3);
            return Json(new
            {
                entityShare,
                employeeShare,
                total = entityShare + employeeShare
            });
        }

        // ─────────────────────────────────────────────────────────
        private static void CalculateShares(InsuredEmployee e)
        {
            e.EntityShare   = Math.Round(e.MonthlySalary * e.EntityRate, 3);
            e.EmployeeShare = Math.Round(e.MonthlySalary * e.EmployeeRate, 3);
            e.TotalPremium  = e.EntityShare + e.EmployeeShare;
        }

        private async Task LoadViewBags()
        {
            ViewBag.Contracts = await _db.InsuranceContracts
                .Where(x => x.Del != true)
                .OrderBy(x => x.FacilityName)
                .ToListAsync();
        }
    }
}

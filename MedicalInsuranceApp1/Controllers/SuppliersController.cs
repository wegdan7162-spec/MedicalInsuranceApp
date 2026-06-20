using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Suppliers)]
    public class SuppliersController : Controller
    {
        private readonly AppDbContext _db;
        public SuppliersController(AppDbContext db) => _db = db;

        // ===== قائمة الموردين =====
        public async Task<IActionResult> Index(string? search, string? type, string? status)
        {
            var query = _db.Suppliers.Where(x => x.Del != true);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x =>
                    x.Name.Contains(search) ||
                    (x.Phone          != null && x.Phone.Contains(search)) ||
                    (x.ContractNumber != null && x.ContractNumber.Contains(search)) ||
                    (x.Email          != null && x.Email.Contains(search)));

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(x => x.EntityType == type);

            if (status == "active")
                query = query.Where(x => x.IsActive);
            else if (status == "inactive")
                query = query.Where(x => !x.IsActive);

            var list = await query.OrderBy(x => x.Name).ToListAsync();

            ViewBag.Search = search;
            ViewBag.Type   = type;
            ViewBag.Status = status;
            ViewBag.Total        = list.Count;
            ViewBag.ActiveCount  = list.Count(x => x.IsActive);
            ViewBag.InactiveCount= list.Count(x => !x.IsActive);

            return View(list);
        }

        // ===== إضافة مورد =====
        [HttpGet]
        public IActionResult Create() => View(new Supplier { IsActive = true });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier model)
        {
            if (!ModelState.IsValid) return View(model);

            model.UserId    = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            model.CreatedAt = DateTime.Now;
            model.Del       = false;

            _db.Suppliers.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة المورد بنجاح";
            return RedirectToAction("Index");
        }

        // ===== تعديل مورد =====
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Suppliers.FindAsync(id);
            if (item == null || item.Del == true) return NotFound();
            return View(item);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Supplier model)
        {
            var item = await _db.Suppliers.FindAsync(id);
            if (item == null || item.Del == true) return NotFound();
            if (!ModelState.IsValid) return View(model);

            item.Name           = model.Name;
            item.Phone          = model.Phone;
            item.Email          = model.Email;
            item.Address        = model.Address;
            item.ContractNumber = model.ContractNumber;
            item.EntityType     = model.EntityType;
            item.Notes          = model.Notes;
            item.IsActive       = model.IsActive;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث بيانات المورد";
            return RedirectToAction("Index");
        }

        // ===== حذف مورد =====
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.Suppliers.FindAsync(id);
            if (item == null) return NotFound();

            item.Del          = true;
            item.DeletedAt    = DateTime.Now;
            item.DeletedBy    = User.Identity?.Name;
            item.DeleteReason = reason;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم حذف المورد";
            return RedirectToAction("Index");
        }

        // ===== API: قائمة الموردين للـ dropdown =====
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var list = await _db.Suppliers
                .Where(x => x.Del != true && x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new { x.Id, x.Name, x.ContractNumber })
                .ToListAsync();
            return Json(list);
        }
    }
}

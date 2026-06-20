using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.ViewModels;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Plaintiffs)]
    public class PlaintiffsController : Controller
    {
        private readonly AppDbContext _db;
        public PlaintiffsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.PlaintiffNames
                .Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.PlainitiffName)
                .ToListAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new PlaintiffName());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlaintiffName model)
        {
            if (string.IsNullOrWhiteSpace(model.PlainitiffName))
            {
                TempData["Error"] = "اسم المدعي مطلوب";
                return View(model);
            }

            model.Del = false;
            _db.PlaintiffNames.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"تم إضافة المدعي '{model.PlainitiffName}' بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.PlaintiffNames.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PlaintiffName model)
        {
            if (string.IsNullOrWhiteSpace(model.PlainitiffName))
            {
                TempData["Error"] = "اسم المدعي مطلوب";
                return View(model);
            }

            var p = await _db.PlaintiffNames.FindAsync(model.Id);
            if (p == null) return NotFound();

            p.PlainitiffName = model.PlainitiffName.Trim();
            p.Job     = model.Job;
            p.Phone   = model.Phone;
            p.Email   = model.Email;
            p.Address = model.Address;
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث بيانات المدعي بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var hasClaims = await _db.OutterClaims.AnyAsync(x => x.PlaintiffNameId == id && (x.Del == null || x.Del == false))
                         || await _db.FriendlyClaims.AnyAsync(x => x.PlaintiffNameId == id && (x.Del == null || x.Del == false));

            if (hasClaims)
            {
                TempData["Error"] = "لا يمكن الحذف — هذا المدعي لديه مطالبات مرتبطة به";
                return RedirectToAction("Index");
            }

            var p = await _db.PlaintiffNames.FindAsync(id);
            if (p != null)
            {
                try
                {
                    p.Del = true;
                    p.DeletedAt = DateTime.Now;
                    p.DeletedBy = User.Identity?.Name;
                    p.DeleteReason = reason;
                    await _db.SaveChangesAsync();
                    TempData["Success"] = "تم حذف المدعي بنجاح";
                }
                catch (Exception ex)
                {
                    try
                    {
                        _db.Entry(p).Reload();
                        p.Del = true;
                        await _db.SaveChangesAsync();
                        TempData["Success"] = "تم حذف المدعي بنجاح";
                    }
                    catch
                    {
                        TempData["Error"] = $"خطأ: {ex.Message}";
                    }
                }
            }
            else
            {
                TempData["Error"] = "المدعي غير موجود";
            }
            return RedirectToAction("Index");
        }
    }
}
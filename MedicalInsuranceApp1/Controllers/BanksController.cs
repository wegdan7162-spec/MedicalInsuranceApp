using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Banks)]
    public class BanksController : Controller
    {
        private readonly AppDbContext _db;
        public BanksController(AppDbContext db) => _db = db;

        // ===== المصارف =====

        public async Task<IActionResult> Index()
        {
            var banks = await _db.Banks
                .Where(x => x.Del != true)
                .Include(x => x.Branches.Where(b => b.Del != true))
                .OrderBy(x => x.BankName)
                .ToListAsync();

            return View(banks);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.Banks, "Add")]
        public async Task<IActionResult> Create(Bank model)
        {
            if (!ModelState.IsValid) return View(model);
            model.Del = false;
            _db.Banks.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة المصرف بنجاح — يمكنك الآن إضافة فروعه";
            return RedirectToAction("CreateBranch", new { id = model.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.Banks.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.Banks, "Edit")]
        public async Task<IActionResult> Edit(int id, Bank model)
        {
            if (!ModelState.IsValid) return View(model);
            var item = await _db.Banks.FindAsync(id);
            if (item == null) return NotFound();

            item.BankCode = model.BankCode;
            item.BankName = model.BankName;
            item.Notes    = model.Notes;
            item.IsActive = model.IsActive;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث المصرف بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.Banks, "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Banks.FindAsync(id);
            if (item != null)
            {
                item.Del       = true;
                item.DeletedAt = DateTime.Now;
                item.DeletedBy = User.Identity?.Name;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف المصرف";
            return RedirectToAction("Index");
        }

        // ===== فروع المصرف =====

        [HttpGet]
        public async Task<IActionResult> CreateBranch(int id)
        {
            var bank = await _db.Banks.FindAsync(id);
            if (bank == null) return NotFound();
            ViewBag.BankId   = id;
            ViewBag.BankName = bank.BankName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.Banks, "Add")]
        public async Task<IActionResult> CreateBranch(BankBranch model)
        {
            if (!ModelState.IsValid)
            {
                var bank = await _db.Banks.FindAsync(model.BankId);
                ViewBag.BankId   = model.BankId;
                ViewBag.BankName = bank?.BankName;
                return View(model);
            }
            model.Del = false;
            _db.BankBranches.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة الفرع بنجاح — يمكنك إضافة فرع آخر";
            return RedirectToAction("CreateBranch", new { id = model.BankId });
        }

        [HttpGet]
        public async Task<IActionResult> EditBranch(int id)
        {
            var branch = await _db.BankBranches.Include(x => x.Bank).FirstOrDefaultAsync(x => x.Id == id);
            if (branch == null) return NotFound();
            ViewBag.BankName = branch.Bank?.BankName;
            return View(branch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.Banks, "Edit")]
        public async Task<IActionResult> EditBranch(int id, BankBranch model)
        {
            var branch = await _db.BankBranches.FindAsync(id);
            if (branch == null) return NotFound();

            branch.BranchCode = model.BranchCode;
            branch.BranchName = model.BranchName;
            branch.Address    = model.Address;
            branch.Phone      = model.Phone;
            branch.IsActive   = model.IsActive;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث الفرع بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.Banks, "Delete")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _db.BankBranches.FindAsync(id);
            if (branch != null)
            {
                branch.Del       = true;
                branch.DeletedAt = DateTime.Now;
                branch.DeletedBy = User.Identity?.Name;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف الفرع";
            return RedirectToAction("Index");
        }
    }
}

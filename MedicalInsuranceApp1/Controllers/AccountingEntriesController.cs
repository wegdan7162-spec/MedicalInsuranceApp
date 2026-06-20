using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.AccountingEntries)]
    public class AccountingEntriesController : Controller
    {
        private readonly AppDbContext _db;
        public AccountingEntriesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? status, int? month, int? year)
        {
            int selMonth = month ?? DateTime.Now.Month;
            int selYear  = year  ?? DateTime.Now.Year;

            var query = _db.AccountingEntries
                .Include(x => x.Lines)
                .Include(x => x.SupplyOrder).ThenInclude(s => s!.Contract)
                .Include(x => x.BankDepositSlip).ThenInclude(b => b!.BankAccount)
                .Where(x => x.Del != true
                          && x.EntryDate.Month == selMonth
                          && x.EntryDate.Year  == selYear);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            var list = await query.OrderByDescending(x => x.EntryDate).ToListAsync();

            ViewBag.Month          = selMonth;
            ViewBag.Year           = selYear;
            ViewBag.Status         = status;
            ViewBag.TotalDebit     = list.Sum(x => x.DebitAmount);
            ViewBag.TotalBankDebit = list.Sum(x => x.BankDebitAmount);
            ViewBag.CountPending   = list.Count(x => x.Status == "مبدئي");
            ViewBag.CountComplete  = list.Count(x => x.Status == "مكتمل");
            ViewBag.CountApproved  = list.Count(x => x.Status == "معتمد");

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _db.AccountingEntries
                .Include(x => x.Lines)
                .Include(x => x.SupplyOrder).ThenInclude(s => s!.Contract)
                .Include(x => x.BankDepositSlip).ThenInclude(b => b!.BankAccount)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);

            if (item == null) return NotFound();
            return View(item);
        }

        /// <summary>اعتماد القيد من قِبَل قسم المراجعة الداخلية</summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(AppModules.InternalReview)]
        public async Task<IActionResult> Approve(int id)
        {
            var item = await _db.AccountingEntries.FindAsync(id);
            if (item == null) return NotFound();

            if (item.Status != "مكتمل")
            {
                TempData["Error"] = "لا يمكن اعتماد قيد غير مكتمل — يجب ربطه بقسيمة إيداع أولاً.";
                return RedirectToAction("Details", new { id });
            }

            item.Status = "معتمد";
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم اعتماد القيد المحاسبي";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Print(int id)
        {
            var item = await _db.AccountingEntries
                .Include(x => x.Lines)
                .Include(x => x.SupplyOrder).ThenInclude(s => s!.Contract)
                .Include(x => x.BankDepositSlip).ThenInclude(b => b!.BankAccount)
                .FirstOrDefaultAsync(x => x.Id == id && x.Del != true);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var item = await _db.AccountingEntries.FindAsync(id);
            if (item != null)
            {
                item.Del          = true;
                item.DeletedAt    = DateTime.Now;
                item.DeletedBy    = User.Identity?.Name;
                item.DeleteReason = reason;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف القيد";
            return RedirectToAction("Index");
        }
    }
}

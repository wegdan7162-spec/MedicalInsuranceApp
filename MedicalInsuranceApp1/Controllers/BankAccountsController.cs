using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Infrastrcture;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.BankAccounts)]
    public class BankAccountsController : Controller
    {
        private readonly AppDbContext _db;
        public BankAccountsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(string? search)
        {
            var query = _db.BankAccounts.Where(x => x.Del != true);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.BankName == search);

            var list = await query.OrderBy(x => x.BankName).ToListAsync();

            ViewBag.Search       = search;
            ViewBag.TotalBalance = list.Where(x => x.IsActive).Sum(x => x.CurrentBalance);
            ViewBag.ActiveCount  = list.Count(x => x.IsActive);

            // قائمة المصارف الفريدة للقائمة المنسدلة
            ViewBag.BankNames = await _db.BankAccounts
                .Where(x => x.Del != true)
                .Select(x => x.BankName)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BankAccount model)
        {
            model.Del            = false;
            model.CurrentBalance = model.OpeningBalance;

            _db.BankAccounts.Add(model);
            await _db.SaveChangesAsync();
            TempData["Success"] = "تم إضافة الحساب المصرفي بنجاح";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.BankAccounts.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BankAccount model)
        {
            var item = await _db.BankAccounts.FindAsync(id);
            if (item == null) return NotFound();

            item.BankName      = model.BankName;
            item.AccountNumber = model.AccountNumber;
            item.BankBranch    = model.BankBranch;
            item.IsActive      = model.IsActive;
            item.Notes         = model.Notes;

            await _db.SaveChangesAsync();
            TempData["Success"] = "تم تحديث الحساب المصرفي بنجاح";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.BankAccounts.FindAsync(id);
            if (item != null)
            {
                item.Del       = true;
                item.DeletedAt = DateTime.Now;
                item.DeletedBy = User.Identity?.Name;
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "تم حذف الحساب المصرفي";
            return RedirectToAction("Index");
        }

        /// <summary>تحديث الرصيد الحالي بناءً على كل الحركات</summary>
        public async Task<IActionResult> RecalcBalance(int id)
        {
            var account = await _db.BankAccounts.FindAsync(id);
            if (account == null) return NotFound();

            var txs = await _db.BankTransactions
                .Where(x => x.BankAccountId == id && x.Del != true)
                .OrderBy(x => x.TransactionDate)
                .ToListAsync();

            decimal balance = account.OpeningBalance;
            foreach (var tx in txs)
            {
                balance += tx.CreditAmount - tx.DebitAmount;
                tx.RunningBalance = balance;
            }
            account.CurrentBalance = balance;
            await _db.SaveChangesAsync();

            TempData["Success"] = "تم إعادة حساب الرصيد بنجاح";
            return RedirectToAction("Index");
        }
    }
}

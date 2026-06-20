using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.Trash)]
    public class TrashController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrashController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> OutterClaims()
        {
            var items = await _db.OutterClaims
                .Include(x => x.Plaintiff)
                .Include(x => x.Branch)
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> FriendlyClaims()
        {
            var items = await _db.FriendlyClaims
                .Include(x => x.Plaintiff)
                .Include(x => x.Branch)
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Branches()
        {
            var items = await _db.Branches
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Courts()
        {
            var items = await _db.Courts
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Plaintiffs()
        {
            var items = await _db.PlaintiffNames
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        // ===== Restore Actions =====
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreOutterClaim(int id)
        {
            var x = await _db.OutterClaims.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع المطالبة بنجاح";
            return RedirectToAction("OutterClaims");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreFriendlyClaim(int id)
        {
            var x = await _db.FriendlyClaims.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع المطالبة بنجاح";
            return RedirectToAction("FriendlyClaims");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreBranch(int id)
        {
            var x = await _db.Branches.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع الفرع بنجاح";
            return RedirectToAction("Branches");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCourt(int id)
        {
            var x = await _db.Courts.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع المحكمة بنجاح";
            return RedirectToAction("Courts");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestorePlaintiff(int id)
        {
            var x = await _db.PlaintiffNames.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع المدعي بنجاح";
            return RedirectToAction("Plaintiffs");
        }

        // ===== Permanent Delete Actions =====
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOutterClaim(int id)
        {
            var x = await _db.OutterClaims.FindAsync(id);
            if (x != null) { _db.OutterClaims.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للمطالبة";
            return RedirectToAction("OutterClaims");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFriendlyClaim(int id)
        {
            var x = await _db.FriendlyClaims.FindAsync(id);
            if (x != null) { _db.FriendlyClaims.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للمطالبة";
            return RedirectToAction("FriendlyClaims");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var x = await _db.Branches.FindAsync(id);
            if (x != null) { _db.Branches.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للفرع";
            return RedirectToAction("Branches");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourt(int id)
        {
            var x = await _db.Courts.FindAsync(id);
            if (x != null) { _db.Courts.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للمحكمة";
            return RedirectToAction("Courts");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlaintiff(int id)
        {
            var x = await _db.PlaintiffNames.FindAsync(id);
            if (x != null) { _db.PlaintiffNames.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للمدعي";
            return RedirectToAction("Plaintiffs");
        }

        // ═══════════════════════════════════════════════
        //  الجزء المالي + الاكتتاب
        // ═══════════════════════════════════════════════

        // ── القيود المحاسبية ──
        public async Task<IActionResult> AccountingEntries()
        {
            var items = await _db.AccountingEntries
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreAccountingEntry(int id)
        {
            var x = await _db.AccountingEntries.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع القيد المحاسبي";
            return RedirectToAction("AccountingEntries");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountingEntry(int id)
        {
            var x = await _db.AccountingEntries.FindAsync(id);
            if (x != null) { _db.AccountingEntries.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للقيد";
            return RedirectToAction("AccountingEntries");
        }

        // ── إيصالات القبض ──
        public async Task<IActionResult> ReceiptVouchers()
        {
            var items = await _db.ReceiptVouchers
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreReceiptVoucher(int id)
        {
            var x = await _db.ReceiptVouchers.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع إيصال القبض";
            return RedirectToAction("ReceiptVouchers");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReceiptVoucher(int id)
        {
            var x = await _db.ReceiptVouchers.FindAsync(id);
            if (x != null) { _db.ReceiptVouchers.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للإيصال";
            return RedirectToAction("ReceiptVouchers");
        }

        // ── أوامر التوريد ──
        public async Task<IActionResult> SupplyOrders()
        {
            var items = await _db.SupplyOrders
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreSupplyOrder(int id)
        {
            var x = await _db.SupplyOrders.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع أمر التوريد";
            return RedirectToAction("SupplyOrders");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplyOrder(int id)
        {
            var x = await _db.SupplyOrders.FindAsync(id);
            if (x != null) { _db.SupplyOrders.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي لأمر التوريد";
            return RedirectToAction("SupplyOrders");
        }

        // ── عقود التأمين ──
        public async Task<IActionResult> InsuranceContracts()
        {
            var items = await _db.InsuranceContracts
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreInsuranceContract(int id)
        {
            var x = await _db.InsuranceContracts.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع عقد التأمين";
            return RedirectToAction("InsuranceContracts");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInsuranceContract(int id)
        {
            var x = await _db.InsuranceContracts.FindAsync(id);
            if (x != null) { _db.InsuranceContracts.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للعقد";
            return RedirectToAction("InsuranceContracts");
        }

        // ── قسائم الإيداع ──
        public async Task<IActionResult> BankDepositSlips()
        {
            var items = await _db.BankDepositSlips
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreBankDepositSlip(int id)
        {
            var x = await _db.BankDepositSlips.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع قسيمة الإيداع";
            return RedirectToAction("BankDepositSlips");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBankDepositSlip(int id)
        {
            var x = await _db.BankDepositSlips.FindAsync(id);
            if (x != null) { _db.BankDepositSlips.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي لقسيمة الإيداع";
            return RedirectToAction("BankDepositSlips");
        }

        // ── العمولات ──
        public async Task<IActionResult> Commissions()
        {
            var items = await _db.Commissions
                .Where(x => x.Del == true)
                .OrderByDescending(x => x.DeletedAt)
                .ToListAsync();
            return View(items);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCommission(int id)
        {
            var x = await _db.Commissions.FindAsync(id);
            if (x != null) { x.Del = false; x.DeletedAt = null; x.DeletedBy = null; x.DeleteReason = null; await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم استرجاع العمولة";
            return RedirectToAction("Commissions");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommission(int id)
        {
            var x = await _db.Commissions.FindAsync(id);
            if (x != null) { _db.Commissions.Remove(x); await _db.SaveChangesAsync(); }
            TempData["Success"] = "تم الحذف النهائي للعمولة";
            return RedirectToAction("Commissions");
        }
    }
}

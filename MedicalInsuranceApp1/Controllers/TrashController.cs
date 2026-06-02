using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Identity;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
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
    }
}

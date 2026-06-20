using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalInsuranceApp1.Data;
using MedicalInsuranceApp1.Models.Entities;
using MedicalInsuranceApp1.Models.ViewModels;
using MedicalInsuranceApp1.Infrastrcture;
using MedicalInsuranceApp1.Models.ViewModels.Identity;

namespace MedicalInsuranceApp1.Controllers
{
    [Authorize]
    [RequirePermission(AppModules.OutterClaims)]
    public class OutterClaimsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OutterClaimsController(
            AppDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ===== قائمة =====
        public async Task<IActionResult> Index(
            string? search, int? branchId,
            string? status, int? year, string? claimStatus)
        {
            var query = _db.OutterClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .Include(x => x.Court)
                .Where(x => x.Del == null || x.Del == false)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // دعم البحث بصيغة "السنة-الرقم" مثل 2025-71
                long? searchNum = null;
                int?  searchYear = null;
                if (search.Contains('-'))
                {
                    var parts = search.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int sy) && long.TryParse(parts[1], out long sn))
                    { searchYear = sy; searchNum = sn; }
                }

                query = query.Where(x =>
                    (x.Plaintiff != null && x.Plaintiff.PlainitiffName.Contains(search)) ||
                    (x.Place != null && x.Place.Contains(search)) ||
                    (searchNum.HasValue && searchYear.HasValue && x.Num == searchNum && x.Year == searchYear) ||
                    x.Num.ToString().Contains(search));
            }

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId);

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "تحت التسوية")
                    query = query.Where(x => x.FileStatus == null || x.FileStatus == "" || x.FileStatus == "UnderSettlement" || x.FileStatus == "تحت التسوية");
                else if (status == "مسدد")
                    query = query.Where(x => x.FileStatus == "مسددة" || x.FileStatus == "مسدد" || x.FileStatus == "Paid");
                else if (status == "مغلقة")
                    query = query.Where(x => x.FileStatus == "مغلقة" || x.FileStatus == "مغلق" || x.FileStatus == "Closed");
                else if (status == "قيد المراجعة")
                    query = query.Where(x => x.FileStatus == "قيد المراجعة" || x.FileStatus == "UnderReview");
                else if (status == "محالة للقانوني")
                    query = query.Where(x => x.FileStatus == "محالة للقانوني" || x.FileStatus == "Legal");
                else if (status == "جلسات المحكمة")
                    query = query.Where(x => x.FileStatus == "جلسات المحكمة" || x.FileStatus == "CourtSessions");
                else if (status == "حكم لصالح الشركة")
                    query = query.Where(x => x.FileStatus == "حكم لصالح الشركة" || x.FileStatus == "RulingForCompany");
                else if (status == "حكم ضد الشركة")
                    query = query.Where(x => x.FileStatus == "حكم ضد الشركة" || x.FileStatus == "RulingAgainstCompany");
                else if (status == "محالة للمالية")
                    query = query.Where(x => x.FileStatus == "محالة للمالية" || x.FileStatus == "Finance");
                else
                    query = query.Where(x => x.FileStatus == status);
            }

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            if (!string.IsNullOrEmpty(claimStatus))
            {
                if (claimStatus == "م.مغطاء")
                    query = query.Where(x => x.ClaimStatus == "م مغطاء"
                                          || x.ClaimStatus == "م.مغطاء"
                                          || x.ClaimStatus == " م.مغطاء"
                                          || x.ClaimStatus == "مغطى"
                                          || x.ClaimStatus == null
                                          || x.ClaimStatus == "");
                else if (claimStatus == "غير مغطاء")
                    query = query.Where(x => x.ClaimStatus == "غيرمغطاء"
                                          || x.ClaimStatus == "غير مغطاء"
                                          || x.ClaimStatus == "غير مغطى"
                                          || x.ClaimStatus == "غيرمغطى");
                else
                    query = query.Where(x => x.ClaimStatus == claimStatus);
            }

            var items = await query.OrderBy(x => x.Year).ThenBy(x => x.Num).ToListAsync();

            var vm = items.Select(x => new OutterClaimListVM
            {
                Id = x.Id,
                ClaimNumber = $"{x.Year}-{x.Num}",
                Year = x.Year,
                PlaintiffName = x.Plaintiff?.PlainitiffName ?? "-",
                BranchName = x.Branch?.BranchName ?? "-",
                CourtName = x.Court?.CourtName ?? "-",
                Place = x.Place ?? "-",
                Reserve = x.Reserve,
                Setteld = x.Setteld,
                ClaimStatus = string.IsNullOrEmpty(x.ClaimStatus) ? " م.مغطاء" : x.ClaimStatus,
                FileStatus = string.IsNullOrEmpty(x.FileStatus) ? "تحت التسوية" : x.FileStatus,
                RegDate = x.RegDate,
                IncidentDate = x.IncidentDate
            }).ToList();

            ViewBag.Search = search;
            ViewBag.BranchId = branchId;
            ViewBag.Status = status;
            ViewBag.ClaimStatus = claimStatus;
            ViewBag.Year = year;
            ViewBag.Total = vm.Count;
            ViewBag.Branches = await _db.Branches.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.StatusList = GetStatusList();

            return View(vm);
        }

        // ===== جرد أولى =====
        public async Task<IActionResult> Inventory(string? search, int? branchId, int? year)
        {
            var query = _db.OutterClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .Include(x => x.Court)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                // دعم البحث بصيغة "السنة-الرقم" مثل 2025-71
                long? searchNum = null;
                int?  searchYear = null;
                if (search.Contains('-'))
                {
                    var parts = search.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int sy) && long.TryParse(parts[1], out long sn))
                    { searchYear = sy; searchNum = sn; }
                }

                query = query.Where(x =>
                    (x.Plaintiff != null && x.Plaintiff.PlainitiffName.Contains(search)) ||
                    (x.Place != null && x.Place.Contains(search)) ||
                    (searchNum.HasValue && searchYear.HasValue && x.Num == searchNum && x.Year == searchYear) ||
                    x.Num.ToString().Contains(search));
            }

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId);

            if (year.HasValue)
                query = query.Where(x => x.Year == year);

            var items = await query.OrderBy(x => x.Year).ThenBy(x => x.Num).ToListAsync();

            var vm = items.Select(x => new OutterClaimListVM
            {
                Id = x.Id,
                ClaimNumber = $"{x.Year}-{x.Num}",
                Year = x.Year,
                PlaintiffName = x.Plaintiff?.PlainitiffName ?? "-",
                BranchName = x.Branch?.BranchName ?? "-",
                CourtName = x.Court?.CourtName ?? "-",
                Place = x.Place ?? "-",
                Reserve = x.Reserve,
                Setteld = x.Setteld,
                ClaimStatus = x.ClaimStatus ?? "-",
                FileStatus = x.FileStatus ?? "-",
                RegDate = x.RegDate,
                IncidentDate = x.IncidentDate
            }).ToList();

            ViewBag.Search = search;
            ViewBag.BranchId = branchId;
            ViewBag.Year = year;
            ViewBag.Branches = await _db.Branches.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.BranchName).ToListAsync();
            ViewBag.TotalReserve = vm.Sum(x => x.Reserve);
            ViewBag.TotalSettled = vm.Sum(x => x.Setteld);
            ViewBag.Total = vm.Count;

            return View(vm);
        }

        // ===== تفاصيل =====
        public async Task<IActionResult> Details(int id)
        {
            var claim = await _db.OutterClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .Include(x => x.Court)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (claim == null) return NotFound();

            var user = claim.UserId != null
                ? await _userManager.FindByIdAsync(claim.UserId)
                : null;

            var vm = new OutterClaimDetailsVM
            {
                Id = claim.Id,
                Num = claim.Num,
                Year = claim.Year,
                PlaintiffName = claim.Plaintiff?.PlainitiffName ?? "-",
                BranchName = claim.Branch?.BranchName ?? "-",
                CourtName = claim.Court?.CourtName ?? "-",
                Place = claim.Place ?? "-",
                Reserve = claim.Reserve,
                Setteld = claim.Setteld,
                Note = claim.Note,
                ClaimStatus = claim.ClaimStatus ?? "م.مغطاء",
                FileStatus = claim.FileStatus ?? "تحت التسوية",
                RegDate = claim.RegDate,
                IncidentDate = claim.IncidentDate,
                CreatedBy = user?.FullName ?? user?.UserName ?? "-",
                Files = claim.Files?.Select(f => new OutterFileVM
                {
                    Id = f.Id,
                    OutterId = f.OutterId,
                    FileName = $"ملف-{f.Id}"
                }).ToList() ?? new(),

                Expenses = await _db.Expenses
                    .Include(e => e.ExpenseType)
                    .Where(e => e.ClaimType == "OutterClaim"
                             && e.ClaimNumber == (int)claim.Num
                             && e.Year == claim.Year)
                    .OrderByDescending(e => e.ExpenseDate)
                    .Select(e => new ExpenseListVM
                    {
                        Id            = e.Id,
                        ExpenseTypeName = e.ExpenseType != null ? e.ExpenseType.ExpenseName : "-",
                        Amount        = e.Amount,
                        ExpenseDate   = e.ExpenseDate,
                        Notes         = e.Notes,
                        AddedBy       = e.UserId ?? "-"
                    }).ToListAsync()
            };
            ViewBag.ExpenseDebug = $"DEBUG Details: ClaimNumber={(int)claim.Num} | Year={claim.Year} | Count={vm.Expenses.Count}";

            // استبدل UserId بالاسم الكامل للمصروفات
            foreach (var exp in vm.Expenses)
            {
                if (exp.AddedBy != "-")
                {
                    var u = await _userManager.FindByIdAsync(exp.AddedBy);
                    exp.AddedBy = u?.FullName ?? u?.UserName ?? "-";
                }
            }

            return View(vm);
        }

        // ===== إضافة =====
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildCreateVM();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OutterClaimCreateVM model)
        {
            if (!ModelState.IsValid)
                return View(await BuildCreateVM(model));

            // التحقق من التكرار
            var duplicate = await _db.OutterClaims.AnyAsync(x =>
                x.PlaintiffNameId == model.PlaintiffNameId &&
                x.Place == model.Place &&
                x.IncidentDate.HasValue &&
                model.IncidentDate.HasValue &&
                x.IncidentDate.Value.Date == model.IncidentDate.Value.Date);

            if (duplicate)
            {
                ModelState.AddModelError("",
                    "⚠ يوجد مطالبة مسبقة بنفس المدعي والمكان والتاريخ");
                return View(await BuildCreateVM(model));
            }

            // التحقق من تكرار رقم المطالبة في نفس السنة ونفس النوع
            var numExists = await _db.OutterClaims.AnyAsync(x =>
                x.Num == model.Num && x.Year == model.ClaimYear);
            if (numExists)
            {
                return View(await BuildCreateVM(model));
            }

            var user = await _userManager.GetUserAsync(User);

            var claim = new OutterClaim
            {
                Num = model.Num,
                Year = model.ClaimYear,
                PlaintiffNameId = model.PlaintiffNameId,
                BranchId = model.BranchId,
                CourtId = model.CourtId,
                Place = model.Place,
                IncidentDate = model.IncidentDate,
                RegDate = model.RegDate,
                Reserve = model.Reserve,
                Setteld = model.Setteld,
                Note = model.Note,
                ClaimStatus = model.ClaimStatus,
                FileStatus = model.FileStatus,
                UserId = user?.Id
            };

            _db.OutterClaims.Add(claim);
            await _db.SaveChangesAsync();

            // رفع المرفق
            var file = Request.Form.Files.FirstOrDefault();
            if (file != null && file.Length > 0)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                _db.OutterFiles.Add(new OutterFile
                {
                    OutterId = claim.Id,
                    FileData = ms.ToArray()
                });
                await _db.SaveChangesAsync();
            }

            // حفظ المصروف الأولي إذا أُدخل
            bool expenseAttempted = model.ExpenseTypeId.HasValue || (model.ExpenseAmount.HasValue && model.ExpenseAmount > 0);
            bool expenseSaved = false;
            string expenseError = "";

            if (model.ExpenseTypeId.HasValue && model.ExpenseAmount.HasValue && model.ExpenseAmount > 0)
            {
                try
                {
                    _db.Expenses.Add(new Expense
                    {
                        ClaimNumber   = (int)claim.Num,
                        Year          = claim.Year,
                        ExpenseTypeId = model.ExpenseTypeId.Value,
                        Amount        = model.ExpenseAmount.Value,
                        ExpenseDate   = model.ExpenseDate ?? DateTime.Now,
                        Notes         = model.ExpenseNotes,
                        UserId        = user?.Id,
                        BranchId      = claim.BranchId ?? 0
                    });
                    await _db.SaveChangesAsync();
                    expenseSaved = true;
                }
                catch (Exception ex)
                {
                    expenseError = ex.InnerException?.Message ?? ex.Message;
                }
            }
            else if (expenseAttempted)
            {
                expenseError = "يرجى اختيار نوع المصروف وإدخال مبلغ أكبر من صفر.";
            }

            if (expenseSaved)
                TempData["Success"] = $"✔ تم تسجيل المطالبة رقم {claim.Year}-{claim.Num} بنجاح، وتم حفظ المصروف.";
            else if (!string.IsNullOrEmpty(expenseError))
            {
                TempData["Success"] = $"✔ تم تسجيل المطالبة رقم {claim.Year}-{claim.Num} بنجاح";
                TempData["Error"] = $"لم يُحفظ المصروف: {expenseError}";
            }
            else
                TempData["Success"] = $"✔ تم تسجيل مطالبة قضايا رقم {claim.Year}-{claim.Num} بنجاح";

            return RedirectToAction("Index");
        }

        // ===== تعديل =====
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var claim = await _db.OutterClaims.FindAsync(id);
            if (claim == null) return NotFound();

            var baseVm = await BuildCreateVM();
            var vm = new OutterClaimEditVM
            {
                Id = claim.Id,
                Num = claim.Num,
                ClaimYear = claim.Year,
                PlaintiffNameId = claim.PlaintiffNameId ?? 0,
                BranchId = claim.BranchId ?? 0,
                CourtId = claim.CourtId,
                Place = claim.Place ?? "",
                IncidentDate = claim.IncidentDate,
                RegDate = claim.RegDate,
                Reserve = claim.Reserve,
                Setteld = claim.Setteld,
                Note = claim.Note,
                ClaimStatus = claim.ClaimStatus ?? "م.مغطاء",
                FileStatus = claim.FileStatus ?? "تحت التسوية",
                Plaintiffs = baseVm.Plaintiffs,
                Branches = baseVm.Branches,
                Courts = baseVm.Courts,
                StatusList = baseVm.StatusList,
                ClaimStatusList = baseVm.ClaimStatusList,
                ExpenseTypes = baseVm.ExpenseTypes
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OutterClaimEditVM model)
        {
            // DEBUG: أقرأ القيم المرسلة من الفورم مباشرة
            var rawTypeId  = Request.Form["ExpenseTypeId"].ToString();
            var rawAmount  = Request.Form["ExpenseAmount"].ToString();
            var rawDate    = Request.Form["ExpenseDate"].ToString();
            TempData["FormDebug"] = $"Form values → ExpenseTypeId='{rawTypeId}' | ExpenseAmount='{rawAmount}' | ExpenseDate='{rawDate}' | model.ExpenseTypeId={model.ExpenseTypeId} | model.ExpenseAmount={model.ExpenseAmount}";

            if (!ModelState.IsValid)
                return View(await BuildCreateVM(model));

            var claim = await _db.OutterClaims.FindAsync(model.Id);
            if (claim == null) return NotFound();

            claim.PlaintiffNameId = model.PlaintiffNameId;
            claim.BranchId = model.BranchId;
            claim.CourtId = model.CourtId;
            claim.Place = model.Place;
            claim.IncidentDate = model.IncidentDate;
            claim.RegDate = model.RegDate;
            claim.Reserve = model.Reserve;
            claim.Setteld = model.Setteld;
            claim.Note = model.Note;
            claim.ClaimStatus = model.ClaimStatus;
            claim.FileStatus = model.FileStatus;

            await _db.SaveChangesAsync();

            // حفظ المصروف الجديد إذا أُدخل
            bool expenseAttempted = model.ExpenseTypeId.HasValue || (model.ExpenseAmount.HasValue && model.ExpenseAmount > 0);
            bool expenseSaved = false;
            string expenseError = "";

            if (model.ExpenseTypeId.HasValue && model.ExpenseAmount.HasValue && model.ExpenseAmount > 0)
            {
                try
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    _db.Expenses.Add(new Expense
                    {
                        ClaimNumber   = (int)claim.Num,
                        Year          = claim.Year,
                        ExpenseTypeId = model.ExpenseTypeId.Value,
                        Amount        = model.ExpenseAmount.Value,
                        ExpenseDate   = model.ExpenseDate ?? DateTime.Now,
                        Notes         = model.ExpenseNotes,
                        UserId        = currentUser?.Id,
                        BranchId      = claim.BranchId ?? 0
                    });
                    await _db.SaveChangesAsync();
                    expenseSaved = true;

                    // تحقق فوري: هل المصروف موجود في DB الآن؟
                    int claimNumInt = (int)claim.Num;
                    int claimYear   = claim.Year;
                    var savedExpenses = await _db.Expenses
                        .Where(e => e.ClaimType == "OutterClaim"
                                 && e.ClaimNumber == claimNumInt
                                 && e.Year == claimYear)
                        .ToListAsync();
                    TempData["Debug"] = $"بعد الحفظ مباشرة: عدد المصروفات في DB = {savedExpenses.Count} | ClaimNumber={claimNumInt} | Year={claimYear}";
                }
                catch (Exception ex)
                {
                    expenseError = ex.InnerException?.Message ?? ex.Message;
                }
            }
            else if (expenseAttempted)
            {
                expenseError = $"ExpenseTypeId={model.ExpenseTypeId} | Amount={model.ExpenseAmount}";
            }

            if (expenseSaved)
                TempData["Success"] = $"✔ تم تحديث المطالبة بنجاح، وتم حفظ المصروف. [ClaimNumber={(int)claim.Num} | Year={claim.Year}]";
            else if (!string.IsNullOrEmpty(expenseError))
                TempData["Error"] = $"لم يُحفظ المصروف: {expenseError}";
            else
                TempData["Success"] = "✔ تم تحديث مطالبة قضايا بنجاح";

            return RedirectToAction("Index");
        }

        // ===== تحديث الحالة =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var claim = await _db.OutterClaims.FindAsync(id);
            if (claim == null) return NotFound();

            claim.ClaimStatus = status;
            await _db.SaveChangesAsync();

            TempData["Success"] = $"✔ تم تحديث الحالة إلى: {status}";
            return RedirectToAction("Details", new { id });
        }

        // ===== تحميل مرفق =====
        public async Task<IActionResult> DownloadFile(int id)
        {
            var file = await _db.OutterFiles.FindAsync(id);
            if (file == null) return NotFound();
            return File(file.FileData, "application/octet-stream", $"file-{id}.pdf");
        }

        // ===== حذف =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, string? reason)
        {
            var claim = await _db.OutterClaims.FirstOrDefaultAsync(x => x.Id == id);
            if (claim == null) return NotFound();

            try
            {
                claim.Del = true;
                claim.DeletedAt = DateTime.Now;
                claim.DeletedBy = User.Identity?.Name;
                claim.DeleteReason = reason;
                await _db.SaveChangesAsync();

                TempData["Success"] = "✔ تم حذف المطالبة بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"✖ فشل الحذف: {ex.InnerException?.Message ?? ex.Message}";
            }
            return RedirectToAction("Index");
        }

        // ===== دوال مساعدة =====
        private async Task<OutterClaimCreateVM> BuildCreateVM(OutterClaimCreateVM? model = null)
        {
            var branches   = await _db.Branches.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.BranchName).ToListAsync();
            var courts     = await _db.Courts.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.CourtName).ToListAsync();
            var plaintiffs = await _db.PlaintiffNames.Where(x => (x.Del == null || x.Del == false)).OrderBy(x => x.PlainitiffName).ToListAsync();

            var vm = model ?? new OutterClaimCreateVM { RegDate = DateTime.Now };

            vm.Branches = branches.Select(b => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = b.Id.ToString(), Text = b.BranchName
            }).ToList();

            vm.Courts = courts.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(), Text = c.CourtName
            }).ToList();

            vm.Plaintiffs = plaintiffs.Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = p.Id.ToString(), Text = p.PlainitiffName
            }).ToList();

            vm.StatusList    = GetStatusList();
            vm.ClaimStatusList = new() { "غير مغطاء", "م.مغطاء" };
            vm.ExpenseTypes  = await GetExpenseTypesAsync();

            return vm;
        }

        private List<string> GetStatusList() => new()
        {
            "تحت التسوية",
            "قيد المراجعة",
            "محالة للقانوني",
            "جلسات المحكمة",
            "حكم لصالح الشركة",
            "حكم ضد الشركة",
            "محالة للمالية",
            "مسدد",
            "مغلقة"
        };

        // ===== إضافة مصروف =====
        [HttpGet]
        public async Task<IActionResult> AddExpense(int id)
        {
            var claim = await _db.OutterClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (claim == null) return NotFound();

            var vm = new AddExpenseVM
            {
                ClaimId       = claim.Id,
                ClaimNum      = claim.Num,
                ClaimYear     = claim.Year,
                PlaintiffName = claim.Plaintiff?.PlainitiffName ?? "-",
                BranchName    = claim.Branch?.BranchName ?? "-",
                BranchId      = claim.BranchId ?? 0,
                ExpenseDate   = DateTime.Now,
                ExpenseTypes  = await GetExpenseTypesAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense(AddExpenseVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ExpenseTypes = await GetExpenseTypesAsync();
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var expense = new Expense
            {
                ClaimNumber   = (int)model.ClaimNum,
                Year          = model.ClaimYear,
                ExpenseTypeId = model.ExpenseTypeId,
                Amount        = model.Amount,
                ExpenseDate   = model.ExpenseDate,
                Notes         = model.Notes,
                UserId        = currentUser?.Id,
                BranchId      = model.BranchId
            };

            _db.Expenses.Add(expense);
            await _db.SaveChangesAsync();

            TempData["Success"] = "✔ تم إضافة المصروف بنجاح";
            return RedirectToAction("Details", new { id = model.ClaimId });
        }

        // ===== تعديل مصروف =====
        [HttpGet]
        public async Task<IActionResult> EditExpense(int id)
        {
            var expense = await _db.Expenses
                .Include(e => e.ExpenseType)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null) return NotFound();

            // نجلب المطالبة المرتبطة
            var claim = await _db.OutterClaims
                .Include(x => x.Branch)
                .Include(x => x.Plaintiff)
                .FirstOrDefaultAsync(x =>
                    x.Num  == expense.ClaimNumber &&
                    x.Year == expense.Year);

            if (claim == null) return NotFound();

            var vm = new EditExpenseVM
            {
                ExpenseId     = expense.Id,
                ClaimId       = claim.Id,
                ClaimNum      = claim.Num,
                ClaimYear     = claim.Year,
                PlaintiffName = claim.Plaintiff?.PlainitiffName ?? "-",
                BranchName    = claim.Branch?.BranchName ?? "-",
                BranchId      = claim.BranchId ?? 0,
                ExpenseTypeId = expense.ExpenseTypeId,
                Amount        = expense.Amount,
                ExpenseDate   = expense.ExpenseDate,
                Notes         = expense.Notes,
                ExpenseTypes  = await GetExpenseTypesAsync()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExpense(EditExpenseVM model)
        {
            if (!ModelState.IsValid)
            {
                model.ExpenseTypes = await GetExpenseTypesAsync();
                return View(model);
            }

            var expense = await _db.Expenses.FindAsync(model.ExpenseId);
            if (expense == null) return NotFound();

            try
            {
                expense.ExpenseTypeId = model.ExpenseTypeId;
                expense.Amount        = model.Amount;
                expense.ExpenseDate   = model.ExpenseDate;
                expense.Notes         = model.Notes;

                await _db.SaveChangesAsync();

                TempData["Success"] = "✔ تم تحديث المصروف بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"✖ فشل التحديث: {ex.InnerException?.Message ?? ex.Message}";
            }

            return RedirectToAction("Details", new { id = model.ClaimId });
        }

        // ===== حذف مصروف =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExpense(int expenseId, int claimId)
        {
            var expense = await _db.Expenses.FindAsync(expenseId);
            if (expense == null) return NotFound();

            try
            {
                _db.Expenses.Remove(expense);
                await _db.SaveChangesAsync();
                TempData["Success"] = "✔ تم حذف المصروف بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"✖ فشل الحذف: {ex.InnerException?.Message ?? ex.Message}";
            }

            return RedirectToAction("Details", new { id = claimId });
        }

        // ===== دالة مساعدة: أنواع المصروفات =====
        private async Task<List<SelectListItem>> GetExpenseTypesAsync()
        {
            var types = await _db.ExpenseTypes.OrderBy(e => e.ExpenseName).ToListAsync();
            return types.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text  = e.ExpenseName
            }).ToList();
        }
    }
}
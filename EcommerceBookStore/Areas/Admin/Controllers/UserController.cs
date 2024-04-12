using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using EcommerceBookStore.Models.ViewModels;
using EcommerceBookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBookStore.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
                IUnitOfWork unitOfWork) {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult RoleManagement(string userId) {
            RoleManagementViewModel RoleVM = new RoleManagementViewModel() {
                ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId),
                RoleList = _roleManager.Roles.Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(x => new SelectListItem {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(x => x.Id == userId)).GetAwaiter().GetResult().FirstOrDefault();
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManageMent(RoleManagementViewModel roleVM) {
            string oldRoleName = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(x => x.Id == roleVM.ApplicationUser.Id)).GetAwaiter().GetResult().FirstOrDefault();

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == roleVM.ApplicationUser.Id);

            if (!(roleVM.ApplicationUser.Role == oldRoleName)) {
                // a role was updated
                if (roleVM.ApplicationUser.Role == SD.Role_Company) {
                    applicationUser.CompanyId = roleVM.ApplicationUser.CompanyId;
                }
                if (oldRoleName == SD.Role_Company) {
                    applicationUser.CompanyId = null;
                }
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();

                // remove old role
                _userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();
                // add new role
                _userManager.AddToRoleAsync(applicationUser, roleVM.ApplicationUser.Role).GetAwaiter().GetResult();
            } else {
                if (oldRoleName == SD.Role_Company && applicationUser.CompanyId != roleVM.ApplicationUser.CompanyId) {
                    applicationUser.CompanyId = roleVM.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }
            return RedirectToAction("Index");
        }


        #region API CALL

        // Get All user
        [HttpGet]
        public IActionResult GetAll() {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            foreach (var user in userList) {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                if (user.Company == null) {
                    user.Company = new() { Name = "" };
                }
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string userId) {
            var objFromDb = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
            if (objFromDb == null) {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now) {
                // user is currently locked and we need unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            } else {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "User Lock/Unlock successful." });
        }

        #endregion
    }
}

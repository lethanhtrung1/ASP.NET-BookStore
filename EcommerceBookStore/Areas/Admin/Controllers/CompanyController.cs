using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using EcommerceBookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBookStore.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CompanyController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		public CompanyController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
			return View(companyList);
		}

		public IActionResult Upsert(int? id) {
			if (id == null || id == 0) {
				return View(new Company());
			} else {
				Company companyObj = _unitOfWork.Company.Get(x => x.Id == id);
				return View(companyObj);
			}
		}

		[HttpPost]
		public IActionResult Upsert(Company company) {
			if (ModelState.IsValid) {
				if (company.Id == 0) {
					_unitOfWork.Company.Add(company);
					TempData["success"] = "Company created successful";
				} else {
					_unitOfWork.Company.Update(company);
					TempData["success"] = "Company updated successful";
				}
				_unitOfWork.Save();
				return RedirectToAction("Index");
			} else {
				return View(company);
			}
		}

		#region API CALLs

		[HttpGet]
		public IActionResult GetAll() {
			List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
			return Json(new { data = companyList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id) {
			var CompanyToDeleted = _unitOfWork.Company.Get(x => x.Id == id);
			if (CompanyToDeleted == null) {
				return Json(new { success = false, message = "Error while deleting" });
			}
			_unitOfWork.Company.Remove(CompanyToDeleted);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Deleted Successful" });
		}

		#endregion
	}
}

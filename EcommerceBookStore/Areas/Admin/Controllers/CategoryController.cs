using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using EcommerceBookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceBookStore.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CategoryController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		public CategoryController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			List<Category> categories = _unitOfWork.Category.GetAll().ToList();
			return View(categories);
		}

		public IActionResult Create() {
			return View();
		}

		[HttpPost]
		public IActionResult Create(Category category) {
			if (category.Name == category.DisplayOrder.ToString()) {
				ModelState.AddModelError("name", "The Display Order cannot exactly match th Name.");
			}
			if (ModelState.IsValid) {
				_unitOfWork.Category.Add(category);
				_unitOfWork.Save();
				TempData["success"] = "Category created successful.";
				return RedirectToAction("Index", "Category");
			}
			return View();
		}

		public IActionResult Edit(int? categoryId) {
			if (categoryId == null || categoryId == 0) {
				return NotFound();
			}
			//Category? category = _dbContext.Categories.Find(categoryId);
			//Category? category = _dbContext.Categories.FirstOrDefault(x => x.Id == categoryId);
			Category? category = _unitOfWork.Category.Get(x => x.Id == categoryId);
			if (category == null) {
				return NotFound();
			}
			return View(category);
		}

		[HttpPost]
		public IActionResult Edit(Category category) {
			if (ModelState.IsValid) {
				_unitOfWork.Category.Update(category);
				_unitOfWork.Save();
				TempData["success"] = "Category updated successful.";
				return RedirectToAction("Index");
			}
			return View(category);
		}

		public IActionResult Delete(int? categoryId) {
			if (categoryId == null || categoryId == 0) {
				return NotFound();
			}
			Category? category = _unitOfWork.Category.Get(x => x.Id == categoryId);
			if (category == null) {
				return NotFound();
			}
			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? categoryId) {
			Category? category = _unitOfWork.Category.Get(x => x.Id == categoryId);
			if (category == null) {
				return NotFound();
			}
			_unitOfWork.Category.Remove(category);
			_unitOfWork.Save();
			TempData["success"] = "Category deleted successful.";
			return RedirectToAction("Index");
		}

		#region API CALLS

		public IActionResult GetAll() {
			IEnumerable<Category> categories = _unitOfWork.Category.GetAll();
			return Json(new { data = categories });
		}

		#endregion
	}
}

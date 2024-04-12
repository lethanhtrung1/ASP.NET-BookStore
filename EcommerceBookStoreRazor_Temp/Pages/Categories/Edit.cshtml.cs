using EcommerceBookStoreRazor_Temp.Data;
using EcommerceBookStoreRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceBookStoreRazor_Temp.Pages.Categories {
	public class EditModel : PageModel {
		private readonly ApplicationDbContext _dbContext;

		[BindProperty]
		public Category Category { get; set; }

		public EditModel(ApplicationDbContext dbContext) {
			_dbContext = dbContext;
		}

		public void OnGet(int? categoryId) {
			if (categoryId != null && categoryId != 0) {
				Category = _dbContext.Categories.FirstOrDefault(x => x.Id == categoryId);
			}
		}

		public IActionResult OnPost() {
			if (ModelState.IsValid) {
				_dbContext.Categories.Update(Category);
				_dbContext.SaveChanges();
				TempData ["success"] = "Category updated successfully";
				return RedirectToAction("Index");
			}
			return Page();
		}
	}
}

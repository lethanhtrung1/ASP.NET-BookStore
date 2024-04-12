using EcommerceBookStoreRazor_Temp.Data;
using EcommerceBookStoreRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceBookStoreRazor_Temp.Pages.Categories {
	public class DeleteModel : PageModel {
		private readonly ApplicationDbContext _dbContext;

		[BindProperty]
		public Category Category { get; set; }

		public DeleteModel(ApplicationDbContext dbContext) {
			_dbContext = dbContext;
		}

		public void OnGet(int? categoryId) {
			if (categoryId != null && categoryId != 0) {
				Category = _dbContext.Categories.FirstOrDefault(x => x.Id == categoryId);
			}
		}

		public IActionResult OnPost() {
			Category? obj = _dbContext.Categories.Find(Category.Id);
			if(obj == null) {
				return NotFound();
			}
			_dbContext.Categories.Remove(obj);
			_dbContext.SaveChanges();
			TempData ["success"] = "Category deleted successfuly";
			return RedirectToAction("Index");
		}
	}
}

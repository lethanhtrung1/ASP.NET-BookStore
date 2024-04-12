using EcommerceBookStoreRazor_Temp.Data;
using EcommerceBookStoreRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceBookStoreRazor_Temp.Pages.Categories {
	[BindProperties]
	public class CreateModel : PageModel {
		private readonly ApplicationDbContext _dbContext;

		//[BindProperty]
		public Category Category { get; set; }

		public CreateModel(ApplicationDbContext dbContext) {
			_dbContext = dbContext;
		}

		public void OnGet() { }

		public IActionResult OnPost() {
			_dbContext.Categories.Add(Category);
			_dbContext.SaveChanges();
			TempData ["success"] = "Category created successfuly";
			return RedirectToAction("Index");
		}
	}
}

using EcommerceBookStoreRazor_Temp.Data;
using EcommerceBookStoreRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceBookStoreRazor_Temp.Pages.Categories {
	public class IndexModel : PageModel {
		private readonly ApplicationDbContext _dbContext;
		public List<Category> CategoryList { get; set; }

		public IndexModel(ApplicationDbContext dbContext) {
			_dbContext = dbContext;
		}

		public void OnGet() {
			CategoryList = _dbContext.Categories.ToList();
		}
	}
}

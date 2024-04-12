using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;

namespace EcommerceBookStore.DataAccess.Repository {
	public class CategoryRepository : Repository<Category>, ICategoryRepository {
		private readonly ApplicationDbContext _dbContext;

		public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(Category category) {
			_dbContext.Categories.Update(category);
		}
	}
}

using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;

namespace EcommerceBookStore.DataAccess.Repository {
	public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository {
		private readonly ApplicationDbContext _dbContext;

		public ProductImageRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(ProductImage productImage) {
			_dbContext.ProductImages.Update(productImage);
		}
	}
}

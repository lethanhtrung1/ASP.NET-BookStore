using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;

namespace EcommerceBookStore.DataAccess.Repository {
	public class ProductRepository : Repository<Product>, IProductRepository {
		private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext) {
            _dbContext = dbContext;
        }

        public void Update(Product product) {
			var productFromDb = _dbContext.Products.FirstOrDefault(x => x.Id == product.Id);
			if(productFromDb != null) {
				productFromDb.Title = product.Title;
				productFromDb.Description = product.Description;
				productFromDb.ISBN = product.ISBN;
				productFromDb.Author = product.Author;
				productFromDb.ListPrice = product.ListPrice;
				productFromDb.Price = product.Price;
				productFromDb.Price50= product.Price50;
				productFromDb.Price100 = product.Price100;
				productFromDb.CategoryId = product.CategoryId;
				// EF core automatically update ProductImages table
				productFromDb.ProductImages = product.ProductImages;
				//if(product.ImageUrl != null) {
				//	productFromDb.ImageUrl = product.ImageUrl;
				//}
			}
			//_dbContext.Products.Update(product);
		}
	}
}

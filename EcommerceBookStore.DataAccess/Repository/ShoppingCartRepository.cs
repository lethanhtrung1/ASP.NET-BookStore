using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookStore.DataAccess.Repository {
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository {
		private readonly ApplicationDbContext _dbContext;

		public ShoppingCartRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(ShoppingCart shoppingCart) {
			_dbContext.shoppingCarts.Update(shoppingCart);
		}
	}
}

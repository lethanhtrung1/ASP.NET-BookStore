﻿using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;

namespace EcommerceBookStore.DataAccess.Repository {
	public class UnitOfWork : IUnitOfWork {
		private readonly ApplicationDbContext _dbContext;
		public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }
		public ICompanyRepository Company { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IApplicationUserRepository ApplicationUser { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }
		public IOrderDetailRepository OrderDetail { get; private set; }
		public IProductImageRepository ProductImage {  get; private set; }

		public UnitOfWork(ApplicationDbContext dbContext) {
			_dbContext = dbContext;
			Category = new CategoryRepository(_dbContext);
			Product = new ProductRepository(_dbContext);
			Company = new CompanyRepository(_dbContext);
			ShoppingCart = new ShoppingCartRepository(_dbContext);
			ApplicationUser = new ApplicationUserRepository(_dbContext);
			OrderHeader = new OrderHeaderRepository(_dbContext);
			OrderDetail	= new OrderDetailRepository(_dbContext);
			ProductImage = new ProductImageRepository(_dbContext);
		}

		public void Save() {
			_dbContext.SaveChanges();
		}
	}
}

using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookStore.DataAccess.Repository {
	public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository {
		private readonly ApplicationDbContext _dbContext;

		public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(ApplicationUser applicationUser) {
			_dbContext.ApplicationUsers.Update(applicationUser);
		}
	}
}

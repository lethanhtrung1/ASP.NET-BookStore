using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookStore.DataAccess.Repository {
	public class CompanyRepository : Repository<Company>, ICompanyRepository {
		private readonly ApplicationDbContext _dbContext;

		public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(Company company) {
			_dbContext.Companies.Update(company);
		}
	}
}

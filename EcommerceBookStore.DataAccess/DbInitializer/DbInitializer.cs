using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.Models;
using EcommerceBookStore.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceBookStore.DataAccess.DbInitializer {
	public class DbInitializer : IDbInitializer {
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _dbContext;

		public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext) {
			_userManager = userManager;
			_roleManager = roleManager;
			_dbContext = dbContext;
		}

		public void Initialize() {
			// migration if they are not applied
			try {
				if (_dbContext.Database.GetPendingMigrations().Count() > 0) {
					_dbContext.Database.Migrate();
				}
			} catch (Exception ex) { }

			// create roles if the are not created
			if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult()) {
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

				// if roles are not created, then we will create admin user as well
				_userManager.CreateAsync(new ApplicationUser {
					UserName = "admin@gmail.com",
					Email = "admin@gmail.com",
					Name = "Admin",
					PhoneNumber = "0969905002",
					StreetAddress = "575 Orange St.New York, NY 10028",
					City = "New York",
					State = "NY",
					PostalCode = "696969"
				}, "admin123aA@").GetAwaiter().GetResult();

				ApplicationUser user = _dbContext.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
				_userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
			}
			return;
		}
	}
}

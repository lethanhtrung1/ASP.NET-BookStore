using EcommerceBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookStore.DataAccess.Repository.IRepository {
	public interface IProductImageRepository : IRepository<ProductImage> {
		void Update(ProductImage productImage);
	}
}

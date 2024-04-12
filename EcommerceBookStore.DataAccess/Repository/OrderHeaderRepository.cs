using EcommerceBookStore.DataAccess.Data;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookStore.DataAccess.Repository {
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository {
		private readonly ApplicationDbContext _dbContext;

		public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext) {
			_dbContext = dbContext;
		}

		public void Update(OrderHeader orderHeader) {
			_dbContext.OrderHeaders.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null) {
			var orderFromDb = _dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if (orderFromDb != null) {
				orderFromDb.OrderStatus = orderStatus;
				if(!string.IsNullOrEmpty(paymentStatus)) {
					orderFromDb.PaymentStatus = paymentStatus;
				}
			}
		}

		public void UpdateStipePaymentID(int id, string sessionId, string paymentIntentId) {
			var orderFromDb = _dbContext.OrderHeaders.FirstOrDefault(x => x.Id == id);
			if(orderFromDb != null) {
				if(!string.IsNullOrEmpty(sessionId)) {
					orderFromDb.SessionId = sessionId;
				}
				if(!string.IsNullOrEmpty(paymentIntentId)) {
					orderFromDb.PaymentIntentId = paymentIntentId;
					orderFromDb.PaymentDate = DateTime.Now;
				}
			}
		}
	}
}

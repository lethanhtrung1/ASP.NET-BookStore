﻿using EcommerceBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceBookStore.DataAccess.Repository.IRepository {
	public interface IOrderHeaderRepository : IRepository<OrderHeader> {
		void Update(OrderHeader orderHeader);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStipePaymentID(int id, string sessionId, string paymentIntentId);
	}
}

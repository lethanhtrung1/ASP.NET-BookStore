using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using EcommerceBookStore.Models.ViewModels;
using EcommerceBookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace EcommerceBookStore.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller {
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrderViewModel OrderVM { get; set; }

		public OrderController(IUnitOfWork unitOfWork) {
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult Details(int orderId) {
			OrderVM = new() {
				OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderId, includeProperties: "Product")
			};
			return View(OrderVM);
		}


		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult UpdateOrderDetail() {
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);

			orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
			orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = OrderVM.OrderHeader.City;
			orderHeaderFromDb.State = OrderVM.OrderHeader.State;
			orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier)) {
				orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
			}
			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber)) {
				orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			}

			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();
			TempData["success"] = "Order Detail updated successful.";

			return RedirectToAction(nameof(Details), new {
				orderId = orderHeaderFromDb.Id
			});
		}


		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult StartProcessing() {
			_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();
			TempData["success"] = "Order Detail updated successful.";

			return RedirectToAction(nameof(Details), new {
				orderId = OrderVM.OrderHeader.Id
			});
		}


		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult ShipOrder() {
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
			orderHeader.OrderStatus = SD.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;
			if (orderHeader.OrderStatus == SD.PaymentStatusDelayedPayment) {
				orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
			}

			_unitOfWork.OrderHeader.Update(orderHeader);
			//_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusShipped);
			_unitOfWork.Save();
			TempData["success"] = "Order shipped successfully.";

			return RedirectToAction(nameof(Details), new {
				orderId = OrderVM.OrderHeader.Id
			});
		}


		[HttpPost]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult CancelOrder() {
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);

			if(orderHeader.PaymentStatus == SD.PaymentStatusApproved) {
				var options = new RefundCreateOptions {
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId,
				};
				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
			} else {
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
			}
			_unitOfWork.Save();
			TempData["success"] = "Order Cancelled successfully";
			return RedirectToAction(nameof(Details), new {
				orderId = orderHeader.Id
			});
		}


		[HttpPost]
		[ActionName("Details")]
		public IActionResult Details_PAY_NOW() {
			OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
			OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

			// Logic Stripe
			var domain = "https://localhost:7245/";

			var options = new Stripe.Checkout.SessionCreateOptions {
				SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
				CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
				LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
				Mode = "payment",
			};

			foreach (var item in OrderVM.OrderDetail) {
				var sessionLineItem = new SessionLineItemOptions {
					PriceData = new SessionLineItemPriceDataOptions {
						UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions {
							Name = item.Product.Title,
						}
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItem);
			}

			var service = new Stripe.Checkout.SessionService();
			Session session = service.Create(options);
			// response have a Id anh PaymentIntentId
			_unitOfWork.OrderHeader.UpdateStipePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();
			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
		}


		// Payment Confirmation for company delay payment
		public IActionResult PaymentConfirmation(int orderHeaderId) {
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == orderHeaderId);
			if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment) {
				// this is an order by company
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if(session.PaymentStatus.ToLower() == "paid") {
					// payment successful
					_unitOfWork.OrderHeader.UpdateStipePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}
			return View(orderHeaderId);
		}



		#region API CALL

		[HttpGet]
		public IActionResult GetAll(string status) {
			List<OrderHeader> orderHeaders;

			if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)) {
				orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			} else {
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
				orderHeaders = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
			}

			switch (status) {
				case "pending":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusPending).ToList();
					break;
				case "inprocess":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusInProcess).ToList();
					break;
				case "completed":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusShipped).ToList();
					break;
				case "approved":
					orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusApproved).ToList();
					break;
				default:
					break;
			}
			return Json(new { data = orderHeaders });
		}

		#endregion
	}
}

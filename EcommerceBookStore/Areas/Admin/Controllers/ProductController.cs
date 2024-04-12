using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using EcommerceBookStore.Models.ViewModels;
using EcommerceBookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceBookStore.Areas.Admin.Controllers {
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class ProductController : Controller {
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) {
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index() {
			List<Product> listProduct = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return View(listProduct);
		}

		// UpdateInsert
		public IActionResult Upsert(int? productId) {
			//IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem {
			//	Text = x.Name,
			//	Value = x.Id.ToString()
			//});
			//ViewBag.CategoryList = CategoryList;
			//ViewData ["CategoryList"] = CategoryList;
			ProductViewModel productViewModel = new() {
				CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem {
					Text = x.Name,
					Value = x.Id.ToString()
				}),
				Product = new Product()
			};
			if (productId == null || productId == 0) {
				// Create
				return View(productViewModel);
			} else {
				// Update
				productViewModel.Product = _unitOfWork.Product.Get(x => x.Id == productId, includeProperties: "ProductImages");
				return View(productViewModel);
			}
		}

		[HttpPost]
		public IActionResult Upsert(ProductViewModel productViewModel, List<IFormFile> files) {
			if (ModelState.IsValid) {
				if (productViewModel.Product.Id != 0) {
					_unitOfWork.Product.Update(productViewModel.Product);
					TempData["success"] = "Product updated successful";
				} else {
					_unitOfWork.Product.Add(productViewModel.Product);
					TempData["success"] = "Product created successful";
				}
				_unitOfWork.Save();

				// logic images
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (files != null) {
					foreach (IFormFile file in files) {
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string productPath = @"images\products\product-" + productViewModel.Product.Id;
						string finalPath = Path.Combine(wwwRootPath, productPath);
						if (!Directory.Exists(finalPath)) {
							Directory.CreateDirectory(finalPath);
						}
						using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create)) {
							file.CopyTo(fileStream);
						}
						ProductImage productImage = new() {
							ImageUrl = @"\" + productPath + @"\" + fileName,
							ProductId = productViewModel.Product.Id,
						};
						if (productViewModel.Product.ProductImages == null) {
							productViewModel.Product.ProductImages = new List<ProductImage>();
						}
						productViewModel.Product.ProductImages.Add(productImage);
					}
					_unitOfWork.Product.Update(productViewModel.Product);
					_unitOfWork.Save();

					// update
					//if (!string.IsNullOrEmpty(productViewModel.Product.ImageUrl)) {
					//	// Delete the old image
					//	var oldImagePath = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
					//	if (System.IO.File.Exists(oldImagePath)) {
					//		System.IO.File.Delete(oldImagePath);
					//	}
					//}

					//using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create)) {
					//	file.CopyTo(fileStream);
					//}

					//productViewModel.Product.ImageUrl = @"\images\product\" + fileName;
				}


				return RedirectToAction("Index");
			} else {
				productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem {
					Text = x.Name,
					Value = x.Id.ToString()
				});

				return View(productViewModel);
			}
		}

		public IActionResult DeleteImage(int imageId) {
			var imageToBeDeleted = _unitOfWork.ProductImage.Get(x => x.Id == imageId);
			int productId = imageToBeDeleted.ProductId;
			if (imageToBeDeleted != null) {
				if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl)) {
					string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));
					if (System.IO.File.Exists(oldImagePath)) {
						System.IO.File.Delete(oldImagePath);
					}
				}
				_unitOfWork.ProductImage.Remove(imageToBeDeleted);
				_unitOfWork.Save();
				TempData["success"] = "Image Deleted Successful.";
			}
			return RedirectToAction(nameof(Upsert), new { productId = productId });
		}

		#region Update
		//public IActionResult Edit(int? productId) {
		//	if (productId == null || productId == 0) {
		//		return NotFound();
		//	}
		//	Product product = _unitOfWork.Product.Get(x => x.Id == productId);
		//	if (product == null) {
		//		return NotFound();
		//	}
		//	return View(product);
		//}

		//[HttpPost]
		//public IActionResult Edit(Product product) {
		//	if (ModelState.IsValid) {
		//		_unitOfWork.Product.Update(product);
		//		_unitOfWork.Save();
		//		TempData["success"] = "Product updated successful";
		//		return RedirectToAction("Index");
		//	}
		//	return View(product);
		//}
		#endregion

		#region Delete
		//public IActionResult Delete(int? productId) {
		//	if (productId == null || productId == 0) {
		//		return NotFound();
		//	}
		//	Product product = _unitOfWork.Product.Get(x => x.Id == productId);
		//	if (product == null) {
		//		return NotFound();
		//	}
		//	return View(product);
		//}

		//[HttpPost, ActionName("Delete")]
		//public IActionResult DeletePOST(int? productId) {
		//	Product? product = _unitOfWork.Product.Get(x => x.Id == productId);
		//	if (product == null) {
		//		return NotFound();
		//	}
		//	_unitOfWork.Product.Remove(product);
		//	_unitOfWork.Save();
		//	TempData["success"] = "Product deleted successful";
		//	return RedirectToAction("Index");
		//}
		#endregion


		#region API CALLS

		[HttpGet]
		public IActionResult GetAll() {
			List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages").ToList();
			return Json(new { data = productList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id) {
			var productToBeDeleted = _unitOfWork.Product.Get(x => x.Id == id);
			if (productToBeDeleted == null) {
				return Json(new { success = false, message = "Error while deleting" });
			}
			//var oldImageUrl = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
			//if (System.IO.File.Exists(oldImageUrl)) {
			//	System.IO.File.Delete(oldImageUrl);
			//}
			string productPath = @"images/products/product-" + id;
			string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);
			if(Directory.Exists(finalPath)) {
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (var filePath in filePaths) {
					System.IO.File.Delete(filePath);
				}
				// Delete path product-id
				Directory.Delete(finalPath);
			}

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Deleted successful" });
		}

		#endregion
	}
}

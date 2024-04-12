﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EcommerceBookStore.DataAccess.Repository.IRepository;
using EcommerceBookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceBookStore.Areas.Identity.Pages.Account.Manage {
	public class IndexModel : PageModel {
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly IUnitOfWork _unitOfWork;

		public IndexModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUnitOfWork unitOfWork) {
			_userManager = userManager;
			_signInManager = signInManager;
			_unitOfWork	= unitOfWork;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string StatusMessage { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel {
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }


			[Display(Name = "street address")]
			public string? StreetAddress { get; set; }
			public string? City { get; set; }
			public string? State { get; set; }
			public string? Name {  get; set; }

			[Display(Name = "postal code")]
			public string? PostalCode { get; set; }
		}

		private async Task LoadAsync(ApplicationUser user) {
			var userName = await _userManager.GetUserNameAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

			Username = userName;

			Input = new InputModel {
				Name = user.Name,
				PhoneNumber = phoneNumber,
				StreetAddress = user.StreetAddress,
				City = user.City,
				State = user.State,
				PostalCode = user.PostalCode,
			};
		}

		public async Task<IActionResult> OnGetAsync() {
			var user = await _userManager.GetUserAsync(User);
			if (user == null) {
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}
			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == user.Id);
			await LoadAsync(applicationUser);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync() {
			//var user = await _userManager.GetUserAsync(User);
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
			ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);
			if (applicationUser == null) {
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid) {
				await LoadAsync(applicationUser);
				return Page();
			}

			//var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			applicationUser.Name = Input.Name;
			applicationUser.PhoneNumber = Input.PhoneNumber;
			applicationUser.StreetAddress = Input.StreetAddress;
			applicationUser.City = Input.City;
			applicationUser.State = Input.State;
			applicationUser.PostalCode = Input.PostalCode;
			_unitOfWork.ApplicationUser.Update(applicationUser);
			_unitOfWork.Save();
			//_userManager.UpdateAsync(applicationUser).GetAwaiter().GetResult();

			//if (Input.PhoneNumber != phoneNumber) {
			//	var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);

			//	if (!setPhoneResult.Succeeded) {
			//		StatusMessage = "Unexpected error when trying to set phone number.";
			//		return RedirectToPage();
			//	}
			//}

			//await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}
	}
}

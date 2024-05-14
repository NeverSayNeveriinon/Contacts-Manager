using Core.Domain.IdentityEntities;
using Core.DTO;
using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Areas.Admin.Controllers;

namespace Web.Controllers
{
    [Route("[controller]/[action]")]
    // [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                                 RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


         // Register//
        [Authorize("NotAuthorized")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize("NotAuthorized")]
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            // Check for validation errors
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(registerDTO);
            }

            ApplicationUser user = new ApplicationUser()
            {
                PersonName = registerDTO.PersonName,
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded)
            {
                // Check status of radio button
                // admin
                if (registerDTO.UserType == UserTypeOptions.Admin)
                {
                    // If needed, Create 'Admin' role (if there is no seed data for "admin" role in 'DbContext' class) 
                    if ((await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString())) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                            { Name = UserTypeOptions.Admin.ToString() };
                        await _roleManager.CreateAsync(applicationRole);
                    }

                    // Add the new user into 'Admin' role
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
                }
                // user
                else if (registerDTO.UserType == UserTypeOptions.User)
                {
                    // If needed, Create 'User' role (if there is no seed data for "user" role in 'DbContext' class) 
                    if ((await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString())) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                            { Name = UserTypeOptions.User.ToString() };
                        await _roleManager.CreateAsync(applicationRole);
                    }

                    // Add the new user into 'User' role
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
                }

                // Sign in
                await _signInManager.SignInAsync(user, isPersistent: false);
                if (registerDTO.UserType == UserTypeOptions.Admin)
                {
                    return RedirectToAction(nameof(PersonsController.Index), "Persons", new {area = "Admin"});
                }
                else if (registerDTO.UserType == UserTypeOptions.User)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }

            }

            
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("Register", error.Description);
            }

            return View(registerDTO);
        }
        
        
        
        
         // Login //
        [Authorize("NotAuthorized")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [Authorize("NotAuthorized")]
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDTO, string? ReturnUrl)
        {
            // Check for validation errors
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(loginDTO);
            }

            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                if (string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                else
                {
                    ApplicationUser? user = await _userManager.FindByNameAsync(loginDTO.Email);
                    
                    if (user != null && await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction(nameof(PersonsController.Index), "Persons", new {area = "Admin"});
                    }
                    else if (user != null && await _userManager.IsInRoleAsync(user, UserTypeOptions.User.ToString()))
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
            }
            
            ModelState.AddModelError("Login", "The Email or Password is incorrect");
            return View(loginDTO);
        }
        
        
        
        // Logout //
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        
        
        
        
        // For remote validation //
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            // check if username is already exists or not
            ApplicationUser? user = await _userManager.FindByNameAsync(email);
            if (user is not null)
            {
                return Json(false);
            }
            return Json(true);
        }
    }
}

// ##Tips##


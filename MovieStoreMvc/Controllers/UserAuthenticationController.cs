using Microsoft.AspNetCore.Mvc;
using MovieStoreMvc.Models.DTO;
using MovieStoreMvc.Repositories.Abstract;

namespace MovieStoreMvc.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private IUserAuthenticationService authService;
        public UserAuthenticationController(IUserAuthenticationService authService)
        {
            this.authService = authService;
        }
        /* We will create a user with admin rights, after that we are going
          to comment this method because we need only
          one user in this application 
          If you need other users ,you can implement this registration method with view
          I have create a complete tutorial for this, you can check the link in description box
         */

        [HttpGet("Account/Register")]
        public IActionResult Register()
        {
            return View("~/Views/UserAuthentication/Register.cshtml");
        }

        [HttpPost("Account/Register")]
        public async Task<IActionResult> RegisterPost(RegistrationModel registrationModel)
        {
            registrationModel.Role = "User"; 
            var result = await authService.RegisterAsync(registrationModel);
            if (result.StatusCode == 1)
            {
                return RedirectToAction("Login");
            }
            else
            {
                TempData["msg"] = "Could not register ..";
                return RedirectToAction(nameof(Register));
            }
        }



        [HttpGet("Account/Login")]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost("Account/Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.LoginAsync(model);
            if (result.StatusCode == 1)
                return RedirectToAction("Index", "Home");
            else
            {
                TempData["msg"] = "Incorrect username or password ..";
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<IActionResult> Logout()
        {
           await authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

    }
}

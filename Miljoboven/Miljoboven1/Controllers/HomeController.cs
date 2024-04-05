using Microsoft.AspNetCore.Mvc;
using Miljoboven2.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Miljoboven2.Models;

namespace Miljoboven2.Controllers;

public class HomeController : Controller
{
    private UserManager<IdentityUser> _userManager;
    private SignInManager<IdentityUser> _signInManager;

    public HomeController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr)
    {
        _userManager = userMgr;
        _signInManager = signInMgr;
    }

    // This method returns the Index view
    public ViewResult Index()
    {
        // Get the Errand session object with key "NewErrand"
        var getSession = HttpContext.Session.Get<Errand>("NewErrand");
    
        // Set a ViewBag property to "Citizen"
        ViewBag.Role = "Citizen";

        // If the session object is null, return the default View
        if (getSession == null)
            return View();

        // If the session object is not null, return the View with the session object as the model
        return View(getSession);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        // Check if the model state is valid
        if (ModelState.IsValid)
        {
            // Attempt to retrieve the user with the provided username
            IdentityUser user = await _userManager.FindByNameAsync(loginModel.Username);
            // If a user was found
            if (user != null)
            {
                // Sign the user out
                await _signInManager.SignOutAsync();
                // Attempt to sign the user in with the provided password
                if ((await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
                {
                    // If the user is a coordinator, redirect them to the coordinator page
                    if (await _userManager.IsInRoleAsync(user, "Coordinator"))
                    {
                        return Redirect("/Coordinator/StartCoordinator");
                    }
                    // If the user is a manager, redirect them to the manager page
                    else if (await _userManager.IsInRoleAsync(user, "Manager"))
                    {
                        return Redirect("/Manager/StartManager");
                    }
                    // If the user is an investigator, redirect them to the investigator page
                    else if (await _userManager.IsInRoleAsync(user, "Investigator"))
                    {
                        return Redirect("/Investigator/StartInvestigator");
                    }
                }
            }
        }
        //If the login is not successful, generate an error message
        ModelState.AddModelError("", "Felaktigt användarnamn eller lösenord");
        return View(loginModel);
    }

    public async Task<RedirectResult> Logout(string returnUrl = "/")
    {
        // Sign the user out
        await _signInManager.SignOutAsync();
        return Redirect(returnUrl);
    }

    public ViewResult NoAccess()
    {
        // The page that is returned when a user tries to access a page they are not authorized to access
        return View();
    }

    public ViewResult Login()
    {
        // The login page
        return View();
    }
}
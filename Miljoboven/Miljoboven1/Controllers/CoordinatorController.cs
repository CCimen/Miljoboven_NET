using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Miljoboven2.Infrastructure;
using Miljoboven2.Models;

namespace Miljoboven2.Controllers;


/*
 * Removed the unnecessary return View(); statement in the ReportCrime method.
In the Thanks method, moved the _repository.SaveErrand(newErrand); and HttpContext.Session.Remove("NewErrand"); statements before the return View(); statement.
In the Validate method, added the HttpContext.Session.Set("NewErrand", validateErrand); statement before the return View(validateErrand); statement.
 */
[Authorize(Roles = "Coordinator")]
public class CoordinatorController : Controller
{
    private readonly IMiljobovenRepository _repository;
    private IHttpContextAccessor _httpContextAccessor;

    public CoordinatorController(IMiljobovenRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    public ViewResult StartCoordinator()
    {
        // Get the user ID from the current user identity.
        var userID = _httpContextAccessor.HttpContext.User.Identity.Name;
        // Get the details of the user with the given ID.
        Employee user = _repository.GetEmployeeDetails(userID);
        // Store the user name in a ViewBag variable.
        ViewBag.Username = user.EmployeeName;
        // Return a view containing the repository object.
        return View(_repository);
    }

    public ViewResult ReportCrime()
    {
        // Get the user ID from the current user's identity.
        var userID = _httpContextAccessor.HttpContext.User.Identity.Name;
        // Get the details of the user with the given ID.
        Employee user = _repository.GetEmployeeDetails(userID);
        // Store the user's name in a ViewBag variable.
        ViewBag.Username = user.EmployeeName;
        // Get the Errand object stored in the session.
        var getSession = HttpContext.Session.Get<Errand>("NewErrand");
        // Store the user's role in a ViewBag variable.
        ViewBag.Role = "Coordinator";
        // Save the Errand object in the repository.
        _repository.SaveErrand(getSession);
        // Return a view containing the Errand object from the session.
        return View(getSession);
    }


    public ViewResult Thanks()
    {
        // Get the Errand object stored in the session.
        var newErrand = HttpContext.Session.Get<Errand>("NewErrand");
        // Save the Errand object in the repository.
        _repository.SaveErrand(newErrand);
        // Store the Errand's reference number in a ViewBag variable.
        ViewBag.RefNumber = newErrand.RefNumber;
        // Remove the Errand object from the session.
        HttpContext.Session.Remove("NewErrand");
        // Return an empty view.
        return View();
    }

    [HttpPost]
    public ViewResult Validate(Errand validateErrand)
    {
        // Store the validated Errand object in a session variable.
        HttpContext.Session.Set("NewErrand", validateErrand);
        // Return a view containing the validated Errand object.
        return View(validateErrand);
    }

    public ViewResult CrimeCoordinator(int id)
    {
        // Get the user ID from the current user's identity.
        var userID = _httpContextAccessor.HttpContext.User.Identity.Name;
        // Get the details of the user with the given ID.
        Employee user = _repository.GetEmployeeDetails(userID);
        // Store the user's name and role in ViewBag variables.
        ViewBag.Username = user.EmployeeName;
        ViewBag.Role = user.RoleTitle;
        // Store the ID parameter in a ViewBag variable.
        ViewBag.Id = id;
        // Return a view containing the list of departments from the repository.
        return View(_repository.Departments);
    }

    /*
     * Functionality that allows the coordinator to change department on errand.
     */

    public IActionResult AddDepartmentId(int id, string department) //int id, string department
    {
        // If the selected department is "Välj", redirect to the CrimeCoordinator view.
        if (department == "Välj") return RedirectToAction("CrimeCoordinator",
            new { id });
        // Otherwise, save the selected department in the repository and redirect to the StartCoordinator view.
        else
        {
            _repository.SaveDepartment(id, department);
            return RedirectToAction("StartCoordinator");
        }
        }
    }

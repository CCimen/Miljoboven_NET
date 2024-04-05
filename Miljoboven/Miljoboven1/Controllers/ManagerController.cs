using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Miljoboven2.Models;

namespace Miljoboven2.Controllers;

// [Authorize] för hela klassen istället för att endast ha på t.ex startsidan.
/*
 * Removed the unnecessary foreach loop in the StartManager method and replaced it with a single call to FirstOrDefault to find the first EmployeeData object with a matching EmployeeId.
In the ChangeErrand method, added a return statement before the RedirectToAction call to avoid unnecessary code execution.
 */
[Authorize(Roles = "Manager")]
public class ManagerController : Controller
{
    private readonly IMiljobovenRepository _repository;
    private IHttpContextAccessor _httpContextAccessor;

    public ManagerController(IMiljobovenRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    public ViewResult CrimeManager(int id)
    {
        // Get the currently logged-in user's department ID
        var departmentId = _httpContextAccessor.HttpContext.User.Identity.Name;
        // Get the user's details from the repository
        Employee user = _repository.GetEmployeeDetails(departmentId);
        // Pass the user's department ID and name to the view using the ViewBag
        ViewBag.DepartmentId = user.DepartmentId;
        ViewBag.Username = user.EmployeeName;
        // Pass the ID parameter to the view using the ViewBag
        ViewBag.Id = id;
        // Return the view with the list of employees from the repository
        return View(_repository.Employees);
    }


    public ViewResult StartManager()
    {
        // Get the currently logged-in user's ID
        var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
        // Get the user's details from the repository
      EmployeeData details = _repository.UserData().FirstOrDefault(d => d.EmployeeId == userId);
        // Pass the user's department name and ID to the view using the ViewBag
        ViewBag.DepartmentName = details.DepartmentName;
        ViewBag.DepartmentId = details.DepartmentId;
        // Pass the user's name to the view using the ViewBag
        ViewBag.UserName = details.EmployeeName;
        // Return the view with the repository
        return View(_repository);
    }


    public IActionResult ChangeErrand(int id, string investigator, string reason, bool noAction)
    {
        // Check if the investigator was not selected and no action was not taken
        if (investigator == "Välj" && noAction == false)
        {
            // Redirect to the CrimeManager action with the ID parameter
            return RedirectToAction("CrimeManager", new { id });
        }
        else
        {
            // Update the errand in the repository
            _repository.ManagerEdit(id, reason, investigator, noAction);
            // Redirect to the StartManager action
            return RedirectToAction("StartManager");
        }
    }
}
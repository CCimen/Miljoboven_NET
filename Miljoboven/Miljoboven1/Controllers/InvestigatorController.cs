using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Miljoboven2.Models;

namespace Miljoboven2.Controllers;

//Refactored FileManager method to now save directly to the final destination instead of a temporary destination. 

[Authorize(Roles = "Investigator")]
public class InvestigatorController : Controller
{
    private readonly IMiljobovenRepository _repository;
    private IHttpContextAccessor _httpContextAccessor;

    public InvestigatorController(IMiljobovenRepository repository, IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    public ViewResult Crimeinvestigator(int id)
    {
        // Get the employee ID from the current user's identity
        var employeeId = _httpContextAccessor.HttpContext.User.Identity.Name;

        // Find the employee with the specified ID
        Employee investigator = null;
        foreach (Employee employee in _repository.Employees)
        {
            if (employeeId == employee.EmployeeId)
            {
                investigator = employee;
            }
        }

        // Set the employee's name in the view bag for use in the view
        ViewBag.UserName = investigator.EmployeeName;
        ViewBag.ID = id;

        // Return the view with the list of errand statuses
        return View(_repository.ErrandStatuses);
    }

// Displays a view with information about a crime investigator
    public ViewResult Startinvestigator()
    {
        // Get the employee ID from the current user's identity
        var employeeId = _httpContextAccessor.HttpContext.User.Identity.Name;

        // Find the employee with the specified ID
        Employee investigator = _repository.GetEmployeeDetails(employeeId);

        // Set the employee's name in the view bag for use in the view
        ViewBag.UserName = investigator.EmployeeName;

        // Return the view with the repository
        return View(_repository);
    }



    /*
     * This method saves the sample and picture that the investigator uploaded to wwwroot folders. It creates a unique name for them.
     */
    private async Task FileManager(int id, IFormFile file)
    {
        // Check if the file is not empty
        if (file.Length > 0)
        {
            // Create a temporary file to store the uploaded file
            var filePath = Path.GetTempFileName();

            // Save the uploaded file to the temporary file
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Generate a unique file name
            var fileName = Guid.NewGuid() + "_" + file.FileName;

            // Check the file type and save it to the appropriate directory
            if (file.ContentType == "application/pdf")
            {
                // Save the PDF file to the "uploaded-samples" directory
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploaded-files/uploaded-samples", fileName);
                System.IO.File.Move(filePath, path);
                _repository.SaveSample(id, fileName);
            }
            else if (file.ContentType == "image/jpeg")
            {
                // Save the JPEG file to the "uploaded-pictures" directory
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploaded-files/uploaded-pictures", fileName);
                System.IO.File.Move(filePath, path);
                _repository.SavePicture(id, fileName);
            }

            // Delete the temporary file
            System.IO.File.Delete(filePath);
        }
    }


    public async Task<IActionResult> ChangeErrand(int id, string events, string information, string status,
        IFormFile uploadImage, IFormFile uploadSample)
    {
        // Call the InvestigatorEdit method on the repository, passing in the given id and the other parameters
        _repository.InvestigatorEdit(id, events, information, status);

        // If the uploadImage parameter is not null, call the FileManager method with the id and uploadImage
        if (uploadImage != null) await FileManager(id, uploadImage);

        // If the uploadSample parameter is not null, call the FileManager method with the id and uploadSample
        if (uploadSample != null) await FileManager(id, uploadSample);

        // Redirect to the CrimeInvestigator action with the given id
        return RedirectToAction("CrimeInvestigator", new { id });
    }
}

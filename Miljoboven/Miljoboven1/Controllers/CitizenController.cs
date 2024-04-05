using Microsoft.AspNetCore.Mvc;
using Miljoboven2.Infrastructure;
using Miljoboven2.Models;

namespace Miljoboven2.Controllers;

public class CitizenController : Controller
{
    /*
     * TODO: Create a database connection
     */
    //private readonly MiljobovenDbContext _db;

    //public CitizenController(MiljobovenDbContext db)
    //{
    //    _db = db;
    //}

    private readonly IMiljobovenRepository _repository;

    public CitizenController(IMiljobovenRepository repository)
    {
        _repository = repository;
    }


    public ViewResult Services()
    {
        return View();
    }

    public ViewResult Contact()
    {
        return View();
    }

    public ViewResult Faq()
    {
        return View();
    }

    public ViewResult Thanks()
    {
        // Retrieve the "NewErrand" object from the session
        var newErrand = HttpContext.Session.Get<Errand>("NewErrand");
        // Save the new errand to the repository
        _repository.SaveErrand(newErrand);
        // Set the reference number in the ViewBag object
        ViewBag.RefNumber = newErrand.RefNumber;
        // Remove the "NewErrand" object from the session
        HttpContext.Session.Remove("NewErrand");
        // Return the view
        return View();
    }

    [HttpPost]
    public ViewResult Validate(Errand validateErrand)
    {
        // Store the validated Errand object in a session variable.
        HttpContext.Session.Set("NewErrand", validateErrand);
        // Store a copy of the validated Errand object in a ViewBag variable.
        ViewBag.ValidateNewErrand = validateErrand;
        // Return a view containing the validated Errand object.
        return View(validateErrand);
    }
}
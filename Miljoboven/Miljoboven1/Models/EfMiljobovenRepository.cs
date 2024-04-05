using Microsoft.EntityFrameworkCore;

namespace Miljoboven2.Models;

public class EfMiljobovenRepository : IMiljobovenRepository
{
    private readonly ApplicationDbContext _context;


    public EfMiljobovenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Picture> Pictures => _context.Pictures;
    public IQueryable<Sample> Samples => _context.Samples;
    public IQueryable<Sequence> Sequences => _context.Sequences;

    public IQueryable<Errand> Errands => _context.Errands.Include(e => e.Samples).Include(e => e.Pictures);
    public IQueryable<ErrandStatus> ErrandStatuses => _context.ErrandStatus1;
    public IQueryable<Employee> Employees => _context.Employees;
    public IQueryable<Department> Departments => _context.Departments;

    public Task<Errand> ShowErrandData(int id)
    {
        return Task.Run(() => Errands.FirstOrDefault(e => e.ErrandId == id));
    }

    public string SaveErrand(Errand newErrand)
    {
        if (newErrand == null)
        {
            // Return an empty string or throw an exception.
            return "";
        }

        if (newErrand.ErrandId < 1)
        {
            var sequence = Sequences.FirstOrDefault(s => s.Id == 1);
            newErrand.RefNumber = "2022-45-" + sequence.CurrentValue;
            newErrand.StatusId = "S_A";
            sequence.CurrentValue++;
            _context.Errands.Add(newErrand);
            _context.SaveChanges();
        }

        return newErrand.RefNumber;
    }


    //Save the department if its not "Välj" and its not errand.departmentid
    public void SaveDepartment(int id, string department)
    {
        var errand = Errands.FirstOrDefault(e => e.ErrandId == id);
        errand.DepartmentId = department;
        _context.SaveChanges();
    }


    public void ManagerEdit(int id, string reason, string investigator, bool noAction)
    {
        var errand = Errands.FirstOrDefault(e => e.ErrandId == id);

        if (noAction != false)
        {
            errand.StatusId = "S_B";
            errand.InvestigatorInfo = reason;
        }
        else
        {
            errand.StatusId = "S_A";
            errand.EmployeeId = investigator;
        }

        _context.SaveChanges();
    }

    public void InvestigatorEdit(int id, string events, string information, string status)
    {
        var errand = Errands.FirstOrDefault(e => e.ErrandId == id);
        ////If the textarea with the name events is not empty, save the text in the field "InvestigatorAction" in DbInitializer. Add spoace between the text.
        if (events != null) errand.InvestigatorAction += events + " ";

        ////If the textarea with the name information is not empty, save the text in the field "InvestigatorInfo" in DbInitializer. The added text should not remove the text that is already there.
        if (information != null) errand.InvestigatorInfo += information + " ";

        ////If the status is not "Välj" and the status is not empty, change the status to the new status.
        if (status != "Välj" && status != null) errand.StatusId = status;

        _context.SaveChanges();
    }

    /*
     * If the Investigator has uploaded files, these files shall be saved to the respective folder in wwwroot and the name of the files shall be saved to the respective table in EF (Pictures, Samples). On the detail pages, the files shall be displayed as clickable links that open up the files from wwwroot, alternatively a text stating that no files exist.
     * 
     */

    public void SavePicture(int id, string fileName)
    {
        var picture = new Picture();
        picture.ErrandId = id;
        picture.PictureName = fileName;
        _context.Pictures.Add(picture);
        _context.SaveChanges();
    }

    public void SaveSample(int id, string fileName)
    {
        var sample = new Sample();
        sample.ErrandId = id;
        sample.SampleName = fileName;
        _context.Samples.Add(sample);
        _context.SaveChanges();
    }


    public IQueryable<MyErrand> CoordinatorErrands()
    {
        var errandList = from errand in Errands
                         join status in ErrandStatuses on errand.StatusId equals status.StatusId
                         join department in Departments on errand.DepartmentId equals department.DepartmentId into departments
                         from dep in departments.DefaultIfEmpty()
                         join employee in Employees on errand.EmployeeId equals employee.EmployeeId into employees
                         from emp in employees.DefaultIfEmpty()
                         orderby errand.RefNumber descending
                         select new MyErrand
                         {
                             DateOfObservation = errand.DateOfObservation,
                             ErrandId = errand.ErrandId,
                             RefNumber = errand.RefNumber,
                             TypeOfCrime = errand.TypeOfCrime,
                             StatusName = status.StatusName,
                             DepartmentName = dep != null ? dep.DepartmentName : "Ej tillsatt",
                             EmployeeName = emp != null ? emp.EmployeeName : "Ej tillsatt"
                         };
        return errandList;
    }

    public IQueryable<EmployeeData> UserData()
    {
        // Join the Employees and Departments collections using a left outer join,
        // using the DepartmentId as the join key for both collections
        var joinedData = Employees.Join(
            Departments,
            user => user.DepartmentId,
            department => department.DepartmentId,
            (user, department) => new EmployeeData()
            {
                // Create a new EmployeeData object and populate it with data from
                // the joined Employee and Department objects
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName,
                EmployeeName = user.EmployeeName,
                EmployeeId = user.EmployeeId
            }
        );

        // Order the joined data by the EmployeeId
        var orderedData = joinedData.OrderBy(user => user.EmployeeId);

        // Return the ordered data as an IQueryable
        return orderedData;
    }


    public IQueryable<MyErrand> GetErrands(string filter)
    {
        // Get all the errands
        var errand = CoordinatorErrands();

        // If the filter is empty, return all the errands
        if (string.IsNullOrEmpty(filter))
        {
            return errand;
        }
        // If the filter starts with "DepartmentName:", filter the errands by department name
        else if (filter.StartsWith("DepartmentName:"))
        {
            // Get the department name from the filter string
            string departmentName = filter.Substring("DepartmentName:".Length);
            // Return the errands where the department name matches the department name in the filter
            return errand.Where(e => e.DepartmentName == departmentName);
        }
        // If the filter starts with "EmployeeName:", filter the errands by employee name
        else if (filter.StartsWith("EmployeeName:"))
        {
            // Get the employee name from the filter string
            string employeeName = filter.Substring("EmployeeName:".Length);
            // Return the errands where the employee name matches the employee name in the filter
            return errand.Where(e => e.EmployeeName == employeeName);
        }
        // If the filter does not match any of the above cases, return all the errands
        else
        {
            return errand;
        }
    }

    public IQueryable<MyErrand> investigatorErrand(string employee)
    {
        // Get all errands with the specified employee's name
        var errands = GetErrands("EmployeeName:" + employee);

        // Select only the errands assigned to the specified employee
        var investigatorErrands = errands.Where(errand => errand.EmployeeName == employee);

        return investigatorErrands;
    }



    //public IQueryable<MyErrand> GetErrands(string filter)
    //{
    //    // Get all the errands
    //    var errand = CoordinatorErrands();

    //    // Get the logged-in investigator's name
    //    string investigatorName = HttpContext.User.Identity.Name;

    //    // If the filter is empty, return all the errands assigned to the logged-in investigator
    //    if (string.IsNullOrEmpty(filter))
    //    {
    //        return errand.Where(e => e.EmployeeName == investigatorName);
    //    }
    //    // If the filter starts with "DepartmentName:", filter the errands by department name
    //    else if (filter.StartsWith("DepartmentName:"))
    //    {
    //        // Get the department name from the filter string
    //        string departmentName = filter.Substring("DepartmentName:".Length);
    //        // Return the errands where the department name matches the department name in the filter and the errands are assigned to the logged-in investigator
    //        return errand.Where(e => e.DepartmentName == departmentName && e.EmployeeName == investigatorName);
    //    }
    //    // If the filter starts with "EmployeeName:", filter the errands by employee name
    //    else if (filter.StartsWith("EmployeeName:"))
    //    {
    //        // Get the employee name from the filter string
    //        string employeeName = filter.Substring("EmployeeName:".Length);
    //        // Return the errands where the employee name matches the employee name in the filter and the errands are assigned to the logged-in investigator
    //        return errand.Where(e => e.EmployeeName == employeeName && e.EmployeeName == investigatorName);
    //    }
    //    // If the filter does not match any of the above cases, return all the errands assigned to the logged-in investigator
    //    else
    //    {
    //        return errand.Where(e => e.EmployeeName == investigatorName);
    //    }
    //}



    public Employee GetEmployeeDetails(string employeeId)
    {
        return Employees.FirstOrDefault(e => e.EmployeeId == employeeId);
    }

}
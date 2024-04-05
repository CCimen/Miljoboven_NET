namespace Miljoboven2.Models;
/*
 *  This is the interface for the repository.
 *  It is used to get data from the fake-database.
 */

public interface IMiljobovenRepository
{
    IQueryable<Errand> Errands { get; }
    IQueryable<ErrandStatus> ErrandStatuses { get; }
    IQueryable<Employee> Employees { get; }
    IQueryable<Department> Departments { get; }
    Task<Errand> ShowErrandData(int id);
    string SaveErrand(Errand newErrand);
    void SaveDepartment(int id, string department);
    void ManagerEdit(int id, string reason, string investigator, bool noAction);
    void InvestigatorEdit(int id, string events, string information, string status);
    void SaveSample(int id, string fileName);
    void SavePicture(int id, string fileName);
    IQueryable<MyErrand> CoordinatorErrands();
    IQueryable<MyErrand> GetErrands(string filter);
    Employee GetEmployeeDetails(string employeeId);
    IQueryable<EmployeeData> UserData();
    IQueryable<MyErrand> investigatorErrand(string employee);

}
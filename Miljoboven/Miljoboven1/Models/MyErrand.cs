namespace Miljoboven2.Models;

/*
 * En lista med attribut, finns ej i databasen utan är enbart en hjälpklass för att kunna skicka med attribut till vyn
 */
public class MyErrand
{
    public DateTime DateOfObservation { get; set; }
    public string RefNumber { get; set; }
    public int ErrandId { get; set; }
    public string TypeOfCrime { get; set; }
    public string StatusName { get; set; }
    public string DepartmentName { get; set; }
    public string EmployeeName { get; set; }
}
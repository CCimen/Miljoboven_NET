using System.ComponentModel.DataAnnotations;

namespace Miljoboven2.Models;

public class ErrandStatus
{
    [Key] public string StatusId { get; set; }

    public string StatusName { get; set; }
}
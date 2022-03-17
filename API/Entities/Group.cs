using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class Group
{
    //Default constructor for EF
    public Group()
    {
    }

    public Group(string name)
    {
        Name = name;
    }

    [Key]
    public string Name { get; set; }

    public ICollection<Connection> Connections { get; set; } = new List<Connection>();
}
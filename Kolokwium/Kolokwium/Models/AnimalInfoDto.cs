namespace Kolokwium.Models;

public class AnimalInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AnimalClass { get; set; }
    public DateTime AdmissionDate { get; set; }
    public OwnerDto Owner { get; set; }
}
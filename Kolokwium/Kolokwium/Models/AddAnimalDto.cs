namespace Kolokwium.Models;

public class AddAnimalDto
{
    public string Name { get; set; }
    public string AnimalClass { get; set; }
    public DateTime AdmissionDate { get; set; }
    public int OwnerId { get; set; }
    public List<ProcedureDto> Procedures { get; set; }
    
}
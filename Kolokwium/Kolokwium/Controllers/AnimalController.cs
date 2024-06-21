using Kolokwium.Models;
using Kolokwium.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium.Controllers;

[ApiController]
[Route("api/animals")]
public class AnimalController : ControllerBase
{
    
    private readonly IDbService _service;

    public AnimalController(IDbService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAnimalInfo(int id)
    {
        if (!await _service.DoesAnimalExist(id))
            return NotFound("Animal with given id doesn't exist");
        return Ok(await _service.GetAnimalInfo(id));
    }

    [HttpPost]
    public async Task<IActionResult> AddAnimalWithOptionalProcedures([FromBody] AddAnimalDto addAnimalDto)
    {
        if (!await _service.DoesOwnerExist(addAnimalDto.OwnerId))
            return NotFound("Owner not found");
        if (!await _service.DoesAnimalClassExist(addAnimalDto.AnimalClass))
            return NotFound("Class not found");
        foreach (var procedure in addAnimalDto.Procedures)
        {
            if (!await _service.DoesProcedureExist(procedure.ProcedureId))
                return NotFound($"Procedure {procedure.ProcedureId} not found");
        }

        await _service.AddAnimal(addAnimalDto);
        return Ok();
    }
}
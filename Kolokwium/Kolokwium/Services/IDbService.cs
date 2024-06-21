using Kolokwium.Models;

namespace Kolokwium.Services;

public interface IDbService
{
    Task<AnimalInfoDto> GetAnimalInfo(int id);
    Task<bool> DoesAnimalExist(int id);

    Task<bool> DoesOwnerExist(int id);

    Task<bool> DoesAnimalClassExist(string animalClass);

    Task<bool> DoesProcedureExist(int id);

    Task AddAnimal(AddAnimalDto addAnimalDto);
}
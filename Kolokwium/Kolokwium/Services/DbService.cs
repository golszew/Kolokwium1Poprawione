using Kolokwium.Models;
using Microsoft.Data.SqlClient;

namespace Kolokwium.Services;

public class DbService : IDbService
{
    private readonly IConfiguration _configuration;

    public DbService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<AnimalInfoDto> GetAnimalInfo(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText =
            "SELECT a.id as aid, a.name as aname, c.name as cname, a.AdmissionDate as adm, o.id as oid, o.firstname, o.lastname" +
            " from Animal a join owner o on a.ownerid = o.id join animal_class c on a.animalclassid = c.id where a.id = @id";
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();
        var animalId = reader.GetOrdinal("aid");
        var animalName = reader.GetOrdinal("aname");
        var animalClass = reader.GetOrdinal("cname");
        var admissionDate = reader.GetOrdinal("adm");
        var ownerId = reader.GetOrdinal("oid");
        var ownerFirst = reader.GetOrdinal("firstname");
        var ownerLast = reader.GetOrdinal("lastname");

        AnimalInfoDto animalInfoDto = null;
        ;
        while(await reader.ReadAsync())

        { 
             animalInfoDto = new AnimalInfoDto()
            {
                Id = reader.GetInt32(animalId),
                Name = reader.GetString(animalName),
                AnimalClass = reader.GetString(animalClass),
                AdmissionDate = reader.GetDateTime(admissionDate),
                Owner = new OwnerDto()
                {
                    Id = reader.GetInt32(ownerId),
                    FirstName = reader.GetString(ownerFirst),
                    LastName = reader.GetString(ownerLast)
                }
            };
        }


        return animalInfoDto;


    }

    public async Task<bool> DoesAnimalExist(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM ANIMAL WHERE ID = @id";
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        return await command.ExecuteScalarAsync() is not null;

    }

    public async Task<bool> DoesOwnerExist(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM owner WHERE ID = @id";
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        return await command.ExecuteScalarAsync() is not null;
    }

    public async Task<bool> DoesAnimalClassExist(string animalClass)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM Animal_Class WHERE name = @name";
        command.Parameters.AddWithValue("@name", animalClass);
        await connection.OpenAsync();
        return await command.ExecuteScalarAsync() is not null;
    }

    public async Task<bool> DoesProcedureExist(int id)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM [procedure] WHERE ID = @id";
        command.Parameters.AddWithValue("@id", id);
        await connection.OpenAsync();
        return await command.ExecuteScalarAsync() is not null;
    }

    public async Task AddAnimal(AddAnimalDto addAnimalDto)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await connection.OpenAsync();
        using SqlCommand commandAddAnimal = new SqlCommand();
        using SqlCommand retrievId = new SqlCommand();
        retrievId.Connection = connection;
        retrievId.CommandText = "SELECT id from animal_class where name = @name";
        retrievId.Parameters.AddWithValue("@name", addAnimalDto.AnimalClass);
        int ClassId = (int)await retrievId.ExecuteScalarAsync();
        commandAddAnimal.Connection = connection;
        commandAddAnimal.CommandText = "INSERT INTO animal (name, admissiondate, ownerid, animalclassid) OUTPUT INSERTED.id values(@name, @adm, @oid, @acid)";
        commandAddAnimal.Parameters.AddWithValue("@name", addAnimalDto.Name);
        commandAddAnimal.Parameters.AddWithValue("@adm", addAnimalDto.AdmissionDate);
        commandAddAnimal.Parameters.AddWithValue("@oid", addAnimalDto.OwnerId);
        commandAddAnimal.Parameters.AddWithValue("@acid", ClassId);
        int insertedId = (int)await commandAddAnimal.ExecuteScalarAsync();

        foreach (var procedure in addAnimalDto.Procedures)
        {
            using SqlCommand addProcedure = new SqlCommand();
            addProcedure.Connection = connection;
            addProcedure.CommandText = "INSERT INTO procedure_animal values(@pid, @aid, @date)";
            addProcedure.Parameters.AddWithValue("@pid", procedure.ProcedureId);
            addProcedure.Parameters.AddWithValue("@aid", insertedId);
            addProcedure.Parameters.AddWithValue("@date", procedure.Date);
            await addProcedure.ExecuteScalarAsync();
        }
    }
}
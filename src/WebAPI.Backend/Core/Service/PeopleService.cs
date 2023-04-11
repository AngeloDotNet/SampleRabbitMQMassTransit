namespace WebAPI.Backend.Core.Service;

public class PeopleService : IPeopleService
{
    private readonly IUnitOfWork<PersonEntity, int> unitOfWork;

    public PeopleService(IUnitOfWork<PersonEntity, int> unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<List<PersonEntity>> GetListItemAsync()
    {
        var people = await unitOfWork.ReadOnly.GetAllAsync();
        return people;
    }

    public async Task<PersonEntity> GetItemAsync(int id)
    {
        var person = await unitOfWork.ReadOnly.GetByIdAsync(id);
        return person;
    }

    public async Task CreateItemAsync(PersonEntity person)
    {
        await unitOfWork.Command.CreateAsync(person);
    }

    public async Task UpdateItemAsync(PersonEntity person)
    {
        await unitOfWork.Command.UpdateAsync(person);
    }

    public async Task DeleteItemAsync(PersonEntity person)
    {
        await unitOfWork.Command.DeleteAsync(person);
    }
}
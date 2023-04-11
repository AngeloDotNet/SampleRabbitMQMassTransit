namespace WebAPI.Backend.Core.Service;

public interface IPeopleService
{
    Task<List<PersonEntity>> GetListItemAsync();
    Task<PersonEntity> GetItemAsync(int id);
    Task CreateItemAsync(PersonEntity item);
    Task UpdateItemAsync(PersonEntity item);
    Task DeleteItemAsync(PersonEntity item);
}
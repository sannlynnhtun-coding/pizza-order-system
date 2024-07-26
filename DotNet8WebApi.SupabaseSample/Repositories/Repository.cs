using DotNet8WebApi.SupabaseSample.Services;
using Supabase.Postgrest.Models;

namespace DotNet8WebApi.SupabaseSample.Repositories;

public class Repository<T> where T : BaseModel, new()
{
    private readonly SupabaseService _supabaseService;

    public Repository(SupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var client = _supabaseService.GetClient();
        var response = await client.From<T>().Get();
        return response.Models;
    }

    public async Task<T> GetByIdAsync(int id)
    {
        var client = _supabaseService.GetClient();
        var response = await client.From<T>().Filter("id", Constants.Operator.Equals, id).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<IEnumerable<T>> InsertAsync(List<T> entities)
    {
        var client = _supabaseService.GetClient();
        var response = await client.From<T>().Insert(entities);
        return response.Models;
    }

    public async Task UpdateAsync(T entity)
    {
        var client = _supabaseService.GetClient();
        await client.From<T>().Update(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var client = _supabaseService.GetClient();
        await client.From<T>().Filter("id", Constants.Operator.Equals, id).Delete();
    }
}
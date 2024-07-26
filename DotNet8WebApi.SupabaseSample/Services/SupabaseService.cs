using Client = Supabase.Client;

namespace DotNet8WebApi.SupabaseSample.Services;

public class SupabaseService
{
    private readonly Client _client;

    public SupabaseService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"];
        var key = configuration["Supabase:Key"];
        _client = new Client(url, key);
    }

    public async Task InitializeAsync()
    {
        await _client.InitializeAsync();
    }

    public Client GetClient()
    {
        return _client;
    }
}
using DotNet8WebApi.SupabaseSample.Db;

namespace DotNet8WebApi.SupabaseSample.Repositories;

public class PizzaOrderRepository : Repository<PizzaOrder>
{
    public PizzaOrderRepository(SupabaseService supabaseService) : base(supabaseService)
    {
    }
}
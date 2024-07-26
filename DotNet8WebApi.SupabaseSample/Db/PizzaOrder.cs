using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DotNet8WebApi.SupabaseSample.Db;

[Table("pizza_orders")]
public class PizzaOrder : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("customer_name")]
    public string CustomerName { get; set; }

    [Column("pizza_type")]
    public string PizzaType { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("status")]
    public string Status { get; set; }
}
namespace DotNet8WebApi.SupabaseSample.Dtos;

public class PizzaOrderDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string PizzaType { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
}
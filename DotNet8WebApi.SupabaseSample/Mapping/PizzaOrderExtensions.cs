using DotNet8WebApi.SupabaseSample.Db;

namespace DotNet8WebApi.SupabaseSample.Mapping;

public static class PizzaOrderExtensions
{
    public static PizzaOrderDto ToDto(this PizzaOrder pizzaOrder)
    {
        return new PizzaOrderDto
        {
            Id = pizzaOrder.Id,
            CustomerName = pizzaOrder.CustomerName,
            PizzaType = pizzaOrder.PizzaType,
            Quantity = pizzaOrder.Quantity,
            Status = pizzaOrder.Status
        };
    }
}
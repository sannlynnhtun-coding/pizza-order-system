using DotNet8WebApi.SupabaseSample.Db;
using DotNet8WebApi.SupabaseSample.Mapping;
using DotNet8WebApi.SupabaseSample.Repositories;

namespace DotNet8WebApi.SupabaseSample.Features;

[ApiController]
[Route("api/[controller]")]
public class PizzaOrderController : ControllerBase
{
    private readonly PizzaOrderRepository _pizzaOrderRepository;

    public PizzaOrderController(PizzaOrderRepository pizzaOrderRepository)
    {
        _pizzaOrderRepository = pizzaOrderRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<PizzaOrderDto>> Get()
    {
        var orders = await _pizzaOrderRepository.GetAllAsync();
        return orders.Select(order => order.ToDto());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var order = await _pizzaOrderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] List<CreatePizzaOrderDto> orderDtos)
    {
        var orders = orderDtos.Select(dto => new PizzaOrder
        {
            CustomerName = dto.CustomerName,
            PizzaType = dto.PizzaType,
            Quantity = dto.Quantity,
            Status = dto.Status
        }).ToList();
        var insertedOrders = await _pizzaOrderRepository.InsertAsync(orders);
        return CreatedAtAction(nameof(Get), new { }, insertedOrders.Select(x => x.ToDto()));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CreatePizzaOrderDto orderDto)
    {
        var order = new PizzaOrder
        {
            Id = id,
            CustomerName = orderDto.CustomerName,
            PizzaType = orderDto.PizzaType,
            Quantity = orderDto.Quantity,
            Status = orderDto.Status
        };
        await _pizzaOrderRepository.UpdateAsync(order);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _pizzaOrderRepository.DeleteAsync(id);
        return NoContent();
    }
}
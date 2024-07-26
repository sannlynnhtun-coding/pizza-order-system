# Supabase Integration with .NET 8 ASP.NET Core Web API

This project demonstrates how to integrate Supabase with a .NET 8 ASP.NET Core Web API. It includes using environment variables for configuration, setting up a generic repository for CRUD operations, and creating a controller for managing pizza orders.

## Features

- ASP.NET Core Web API with Supabase integration
- Environment variable configuration
- Generic repository for data access
- CRUD operations for pizza orders

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Supabase Account](https://supabase.io/)
- [DotNetEnv](https://www.nuget.org/packages/DotNetEnv) package for environment variable management

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/SupabaseIntegration.git
cd SupabaseIntegration
```

### 2. Setup Environment Variables

Create a `.env` file in the root of the project and add your Supabase configuration:

```dotenv
SUPABASE_URL=https://your-supabase-url.supabase.co
SUPABASE_KEY=your-supabase-key
```

### 3. Install Dependencies

Install the necessary NuGet packages:

```bash
dotnet add package Supabase
dotnet add package DotNetEnv
dotnet add package Newtonsoft.Json
```

### 4. Create the `PizzaOrders` Table

Use the following SQL script to create the `PizzaOrders` table in your Supabase project:

```sql
CREATE TABLE public.PizzaOrders (
    Id SERIAL PRIMARY KEY,
    CustomerName VARCHAR(100) NOT NULL,
    PizzaType VARCHAR(100) NOT NULL,
    Quantity INTEGER NOT NULL,
    Status VARCHAR(50) NOT NULL
);
```

### 5. Insert Sample Data

Use the following SQL script to insert 5 rows of sample data into the `PizzaOrders` table:

```sql
INSERT INTO public.PizzaOrders (CustomerName, PizzaType, Quantity, Status) VALUES 
('John Doe', 'Margherita', 2, 'Ordered'),
('Jane Smith', 'Pepperoni', 1, 'Preparing'),
('Alice Johnson', 'Veggie', 3, 'Delivered'),
('Bob Brown', 'BBQ Chicken', 2, 'Cancelled'),
('Charlie Davis', 'Hawaiian', 4, 'Ordered');
```

### 6. Update Program.cs

Update `Program.cs` to load environment variables and configure the application using top-level statements:

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DotNetEnv;
using SupabaseIntegration;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
builder.Services.AddSingleton<SupabaseService>();
builder.Services.AddScoped<PizzaOrderRepository>();
builder.Services.AddScoped(typeof(Repository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

var supabaseService = app.Services.GetRequiredService<SupabaseService>();
await supabaseService.InitializeAsync();

app.Run();
```

### 7. Configure SupabaseService

Create a `SupabaseService` to manage Supabase client initialization:

```csharp
using Supabase;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;

namespace SupabaseIntegration
{
    public class SupabaseService
    {
        private readonly Client _client;

        public SupabaseService(IConfiguration configuration)
        {
            var url = Environment.GetEnvironmentVariable("SUPABASE_URL");
            var key = Environment.GetEnvironmentVariable("SUPABASE_KEY");
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
}
```

### 8. Create Models and DTOs

Define the `PizzaOrder` model and DTOs:

#### Models/PizzaOrder.cs

```csharp
using Supabase;
using Supabase.Postgrest.Attributes;

namespace SupabaseIntegration.Models
{
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
}
```

#### DTOs/PizzaOrderDto.cs

```csharp
namespace SupabaseIntegration.DTOs
{
    public class PizzaOrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string PizzaType { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}
```

#### DTOs/CreatePizzaOrderDto.cs

```csharp
namespace SupabaseIntegration.DTOs
{
    public class CreatePizzaOrderDto
    {
        public string CustomerName { get; set; }
        public string PizzaType { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
    }
}
```

### 9. Create Repository

Create a generic `Repository<T>` class and a specific `PizzaOrderRepository`:

#### Repositories/Repository.cs

```csharp
using Supabase;
using Supabase.Postgrest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupabaseIntegration.Repositories
{
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
}
```

#### Repositories/PizzaOrderRepository.cs

```csharp
namespace SupabaseIntegration.Repositories
{
    public class PizzaOrderRepository : Repository<PizzaOrder>
    {
        public PizzaOrderRepository(SupabaseService supabaseService) : base(supabaseService)
        {
        }
    }
}
```

### 10. Create Controller

Create the `PizzaOrderController` to handle HTTP requests:

```csharp
using Microsoft.AspNetCore.Mvc;
using SupabaseIntegration.DTOs;
using SupabaseIntegration.Models;
using SupabaseIntegration.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupabaseIntegration.Controllers
{
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
}
```

### 11. Create Extension Method for DTO Conversion

Ensure you have the extension method for converting `PizzaOrder` to `PizzaOrderDto`.

#### Extensions/PizzaOrderExtensions.cs

```csharp
namespace SupabaseIntegration.Extensions
{
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
}
```

---

Sure, here's a detailed guide on how to create a Supabase project and obtain the URL and Key.

### Creating a Supabase Project and Obtaining URL and Key

#### Step 1: Sign Up and Log In

1. **Sign Up:**
    - Go to [Supabase](https://supabase.io/).
    - Click on `Sign Up` and create an account using your email, GitHub, or any other supported method.

2. **Log In:**
    - After signing up, log in to your Supabase account.

#### Step 2: Create a New Project

1. **Dashboard:**
    - Once logged in, you will be redirected to the Supabase dashboard.

2. **Create a New Project:**
    - Click on the `New Project` button.
    - Fill in the details for your new project:
        - **Project Name:** Choose a unique name for your project.
        - **Organization:** Select the organization for your project (if applicable).
        - **Database Password:** Set a strong password for your database. Ensure you remember this password as it will be needed for database access.
        - **Region:** Select the region closest to you or your user base.

3. **Create Project:**
    - Click on `Create Project` to create your new Supabase project. This may take a few moments.

#### Step 3: Accessing Project URL and API Key

1. **Project Settings:**
    - Once the project is created, you will be redirected to the project dashboard.
    - Click on the `Settings` tab in the left sidebar.

2. **API Settings:**
    - Under `Settings`, click on `API`.
    - Here, you will find the `Project URL` and `API Key`.

3. **Copy the URL and Key:**
    - **Project URL:** This is the base URL for your Supabase project. Copy the URL, which looks something like `https://xyzcompany.supabase.co`.
    - **API Key:** This is the secret key to access your Supabase project. Copy the `anon` key under the `Project API keys` section for client-side usage. For server-side, use the `service_role` key.

### Example Configuration in .env

Create a `.env` file in the root of your project and add the copied URL and Key:

```dotenv
SUPABASE_URL=https://your-supabase-url.supabase.co
SUPABASE_KEY=your-supabase-key
```

Replace `https://your-supabase-url.supabase.co` and `your-supabase-key` with the actual URL and key you obtained from the Supabase dashboard.

### Example Configuration in appsettings.json

Alternatively, you can add these settings to your `appsettings.json` file:

```json
{
  "Supabase": {
    "Url": "https://your-supabase-url.supabase.co",
    "Key": "your-supabase-key"
  }
}
```

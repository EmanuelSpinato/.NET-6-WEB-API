using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapPost("/products", (Product product) => {
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product.Code);
});

app.MapGet("/products/{code}", ([FromRoute] string code) => {
    var product = ProductRepository.GetBy(code);
    if(product != null){
        Console.WriteLine("Product found");
        return Results.Ok(product);
    } 
    return Results.NotFound();
});

app.MapPut("/products", (Product product) => {
    var productSaved = ProductRepository.GetBy(product.Code);
    // Verifica se o produto existe no repositório
    if (productSaved == null) {
        return Results.NotFound(); // Retorna 404 se o produto não for encontrado
    }
    productSaved.Name = product.Name;
    return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] string code) => {
    var productSaved = ProductRepository.GetBy(code);
    // Verifica se o produto existe no repositório
    if (productSaved == null)
    {
        return Results.NotFound(); // Retorna 404 se o produto não for encontrado
    }
    ProductRepository.Remove(productSaved);
    return Results.Ok();
});

if(app.Environment.IsStaging())
    app.MapGet("/configuration/database", (IConfiguration configuration) => {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });

app.Run();

public static class ProductRepository {
    public static List<Product> Products { get; set; } = Products = new List<Product>();

    public static void Init(IConfiguration configuration) {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products ?? new List<Product>();
    }

    public static void Add(Product product) { 
        Products.Add(product);
    }

    public static Product GetBy(string code) {
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(Product product){
        Products.Remove(product);
    }
}

public class Product {
    public string Code { get; set; }
    public string Name { get; set; }
}
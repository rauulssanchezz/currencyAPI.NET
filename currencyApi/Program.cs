using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Welcome to exchange currency API!\nUse /Currencies?originCurrency=eur&currency=usd&amount=1 to get the exchange\nyou can use any currency you want!");

app.MapGet("/Currencies", async (String originCurrency, String currency, Double amount) => {
    var conversorDouble = await GetCurrency(currency.ToLower(), originCurrency.ToLower());
    return amount * conversorDouble;
});

app.Run();

static async Task<double> GetCurrency (string currency, string originCurrency = "eur"){
    using var httpClient = new HttpClient();

    var url = $"https://cdn.jsdelivr.net/npm/@fawazahmed0/currency-api@latest/v1/currencies/{originCurrency}.json";

    var result = 0.0;

    try{
        var response = await httpClient.GetAsync(url);

        if(response.IsSuccessStatusCode){
            var content = await response.Content.ReadAsStringAsync();
            
            var json = JsonDocument.Parse(content);
            var root = json.RootElement;

            if(root.TryGetProperty("eur", out var eurObject)){
                if(eurObject.TryGetProperty(currency, out var gbpValue)){
                    Console.WriteLine(gbpValue.GetDouble());
                    result = gbpValue.GetDouble();
                }else{
                    Console.WriteLine("No GBP property");
                }
            } else {
                Console.WriteLine("No EUR property");
            }

        }
    }catch(Exception e){
        Console.WriteLine(e.Message);
    }
    return result;
}
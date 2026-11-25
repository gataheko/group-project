using System.Net.Http.Headers;
using System.Text.Json;

namespace SmallBusinessInventory.Services;

public record FoodProduct(string Name, string? Brand, string? Quantity, string? Categories);

public class OpenFoodFactsService(HttpClient httpClient)
{
    public async Task<FoodProduct?> FindByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode)) return null;

        httpClient.DefaultRequestHeaders.UserAgent.Clear();
        httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("SmallBusinessInventory", "1.0"));

        var url = $"https://world.openfoodfacts.org/api/v2/product/{Uri.EscapeDataString(barcode.Trim())}.json";
        using var response = await httpClient.GetAsync(url, cancellationToken);
        if (!response.IsSuccessStatusCode) return null;

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var root = json.RootElement;
        if (!root.TryGetProperty("status", out var status) || status.GetInt32() != 1 ||
            !root.TryGetProperty("product", out var product)) return null;

        string? Read(string key) =>
            product.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString() : null;

        var name = Read("product_name") ?? Read("product_name_en");
        if (string.IsNullOrWhiteSpace(name)) return null;
        return new FoodProduct(name, Read("brands"), Read("quantity"), Read("categories"));
    }
}

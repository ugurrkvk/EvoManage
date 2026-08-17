using System.Net.Http.Json;

namespace EvoManage.Infrastructure.Integrations.ERP.Legacy;

public sealed class FakeLegacyErpClient(HttpClient httpClient) : ILegacyErpClient
{
    public async Task SendStockTransactionAsync(LegacyErpStockRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync(
            "api/legacy-stock-transactions",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wms.Core.Interfaces.Repositories;
using Wms.Core.Interfaces.Services;
using Wms.Core.Interfaces.Services.Notifications;

namespace Wms.Services.BackgroundServices;

public class NotificationBackgroundService(
    ILogger<NotificationBackgroundService> logger,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при выполнении фоновой проверки уведомлений.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task CheckNotificationsAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var batchRepository = scope.ServiceProvider.GetRequiredService<IBatchRepository>();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var today = DateTime.UtcNow.Date;
        var threeDaysFromNow = today.AddDays(3);
        var expiringBatches =
            await batchRepository.GetBatchesByExpiryDateRangeAsync(today, threeDaysFromNow, cancellationToken);
        foreach (var batch in expiringBatches)
        {
            const string title = "Срок годности истекает";
            var message =
                $"Партия товара '{batch.Product?.Name ?? "unknown"}' (id {batch.Id}) истекает {batch.ExpiryDate:dd.MM.yyyy}. Остаток: {batch.Quantity - batch.ReservedQuantity}.";
            await notificationService.NotifyManagersAsync(title, message, cancellationToken);
        }

        var lowStockProducts = await productRepository.GetLowStockProductsAsync(cancellationToken);
        foreach (var product in lowStockProducts)
        {
            const string title = "Низкий остаток товара";
            var message =
                $"Общий доступный остаток товара '{product.Name}' составляет {product.TotalAvailable} (порог {product.MinStockThreshold}).";
            await notificationService.NotifyManagersAsync(title, message, cancellationToken);
        }
    }
}
using System;
using System.Collections.Generic;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.NotificationsContext;
using OmniProduct_CoreDomain.Models.SuppliersContext;
using OmniProduct_CoreDomain.ValueObjects;

namespace OmniProduct_CoreDomain.Services;

public sealed class Notifications(TimeProvider clock)
{
    private const string CustomersRecipient = "customers@omniproduct.com";

    private readonly List<Notification> _sent = [];

    public void AnnounceSale(ProductId productId, string name, IEnumerable<Supplier> suppliers, int quantity, int remainingStock)
    {
        ArgumentNullException.ThrowIfNull(suppliers);
        foreach (var supplier in suppliers)
        {
            Send(productId, supplier.Email, new Message(
                $"Product sold: {name}",
                $"{quantity} unit(s) of {name} were sold. Remaining stock: {remainingStock}."));
        }
    }

    public void AnnounceDeprecation(ProductId productId, string name, IEnumerable<Supplier> suppliers)
    {
        ArgumentNullException.ThrowIfNull(suppliers);
        foreach (var supplier in suppliers)
        {
            Send(productId, supplier.Email, new Message(
                $"Product deprecated: {name}",
                $"The product {name} has been deprecated and removed from the catalog."));
        }

        Send(productId, CustomersRecipient, new Message(
            $"Product no longer available: {name}",
            $"{name} is no longer available."));
    }

    public IReadOnlyList<Notification> Sent => _sent;

    private void Send(ProductId productId, string recipient, Message message) =>
        _sent.Add(new Notification(NotificationId.New(), productId, recipient, message, NotificationChannel.Email, clock.GetUtcNow()));
}

using System;
using OmniProduct_CoreDomain.Abstractions;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.ValueObjects;

namespace OmniProduct_CoreDomain.Models.NotificationsContext;

public sealed class Notification : IDentifiable
{
    public Notification(NotificationId id, ProductId productId, string recipient, Message message,
                        NotificationChannel channel, DateTimeOffset sentAt)
    {
        Id = id;
        ProductId = productId;
        Recipient = recipient;
        Message = message;
        Channel = channel;
        SentAt = sentAt;
    }

    public NotificationId Id { get; }
    public ProductId ProductId { get; }
    public string Recipient { get; }
    public Message Message { get; }
    public NotificationChannel Channel { get; }
    public DateTimeOffset SentAt { get; }

    EntityId IDentifiable.Id => new(Id.Value);
}

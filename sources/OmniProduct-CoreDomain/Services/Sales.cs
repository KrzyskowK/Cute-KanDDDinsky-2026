using OmniProduct_CoreDomain.Identifiers;

namespace OmniProduct_CoreDomain.Services;

public sealed class Sales(Catalog catalog, Storage storage, Suppliers suppliers, Notifications notifications)
{
    public void Sell(ProductId productId, int quantity)
    {
        storage.ReleaseStock(productId, quantity);

        notifications.AnnounceSale(
            productId,
            catalog.Get(productId).Name,
            suppliers.GetSuppliersOf(productId),
            quantity,
            storage.GetStock(productId).Stock);
    }
}

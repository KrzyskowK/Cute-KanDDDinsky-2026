using AwesomeAssertions;
using OmniProduct_CoreDomain.Identifiers;
using OmniProduct_CoreDomain.Models.CatalogContext;
using OmniProduct_CoreDomain.Services;

namespace OmniProduct_Core.Test;

public class ProductServiceTests
{
    private readonly Lifecycle _lifecycle;
    private readonly Storage _storage = new();
    private readonly Suppliers _suppliers = new();
    private readonly Pricing _pricing = new();
    private readonly Transport _transport = new();
    private readonly Notifications _notifications;
    private readonly Catalog _catalog;
    private readonly Sales _sales;
    private readonly Assortment _assortment;

    public ProductServiceTests()
    {
        _lifecycle = new Lifecycle(TimeProvider.System);
        _notifications = new Notifications(TimeProvider.System);
        _catalog = new Catalog(_lifecycle, _storage, _suppliers);
        _sales = new Sales(_catalog, _storage, _suppliers, _notifications);
        _assortment = new Assortment(_lifecycle, _catalog, _pricing, _storage, _suppliers, _transport, _notifications);
    }

    [Fact]
    public void CatalogProduct_Name_ShouldBeSet()
    {
        var product = new CatalogProduct(ProductId.New(), "Super Widget");

        product.Name.Should().Be("Super Widget");
        product.Slug.Should().Be("super-widget");
    }

    [Fact]
    public void AddSupplier_ShouldReturnSupplierWithCorrectRegion()
    {
        var supplier = _suppliers.AddSupplier("Acme", "acme@example.com", "FR");

        supplier.Region.Should().Be("FR");
    }

    [Fact]
    public void FullProductLifecycle_ShouldWork()
    {
        _suppliers.AddSupplier("Acme", "acme@example.com", "FR");
        _storage.AddWarehouse("Paris Hub", "1 rue de la Paix", "FR");

        var productId = _assortment.AddProduct("Super Widget", "FR", 100m, "EUR");

        _catalog.Get(productId).Name.Should().Be("Super Widget");
        _storage.GetStock(productId).Stock.Should().Be(0);
        _lifecycle.Get(productId).IsDeprecated.Should().BeFalse();

        _storage.ReceiveStock(productId, 50);
        _storage.GetStock(productId).Stock.Should().Be(50);

        _sales.Sell(productId, 10);
        _storage.GetStock(productId).Stock.Should().Be(40);
        _storage.GetStock(productId).IsOutOfStock.Should().BeFalse();

        _sales.Sell(productId, 40);
        _storage.GetStock(productId).Stock.Should().Be(0);
        _storage.GetStock(productId).IsOutOfStock.Should().BeTrue();
        _catalog.GetDisplayLabel(productId).Should().Be("[OUT OF STOCK] Super Widget");

        _pricing.GetResellerPrice(productId).Should().Be(124m); // 100 + 20% margin + 20% VAT on margin

        _assortment.DeprecateProduct(productId);
        _lifecycle.Get(productId).IsDeprecated.Should().BeTrue();
        _storage.GetStock(productId).Stock.Should().Be(0);

        _catalog.GetCatalog("FR").Should().NotContain(p => p.Id == productId);
    }

    [Fact]
    public void Selling_more_than_in_stock_is_rejected()
    {
        _suppliers.AddSupplier("Acme", "acme@example.com", "FR");
        _storage.AddWarehouse("Paris Hub", "1 rue de la Paix", "FR");
        var productId = _assortment.AddProduct("Super Widget", "FR", 100m, "EUR");
        _storage.ReceiveStock(productId, 5);

        var sell = () => _sales.Sell(productId, 6);

        sell.Should().Throw<InvalidOperationException>();
        _storage.GetStock(productId).Stock.Should().Be(5);
    }

    [Fact]
    public void Selling_notifies_each_regional_supplier_once()
    {
        _suppliers.AddSupplier("Acme", "acme@example.com", "FR");
        _storage.AddWarehouse("Paris Hub", "1 rue de la Paix", "FR");
        var productId = _assortment.AddProduct("Super Widget", "FR", 100m, "EUR");
        _storage.ReceiveStock(productId, 5);

        _sales.Sell(productId, 2);

        _notifications.Sent.Should().ContainSingle()
            .Which.Recipient.Should().Be("acme@example.com");
    }

    [Fact]
    public void Deprecating_notifies_suppliers_and_customers()
    {
        _suppliers.AddSupplier("Acme", "acme@example.com", "FR");
        _storage.AddWarehouse("Paris Hub", "1 rue de la Paix", "FR");
        var productId = _assortment.AddProduct("Super Widget", "FR", 100m, "EUR");

        _assortment.DeprecateProduct(productId);

        _notifications.Sent.Select(n => n.Recipient)
            .Should().BeEquivalentTo("acme@example.com", "customers@omniproduct.com");
        _catalog.GetDisplayLabel(productId).Should().Be("[DISCONTINUED] Super Widget");
    }
}

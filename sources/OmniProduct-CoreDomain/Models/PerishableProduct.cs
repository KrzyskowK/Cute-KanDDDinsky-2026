namespace OmniProduct_CoreDomain.Models;

// Deliberately dirty OO design (HA4.1 exercise): perishable products modelled by subclassing
// Product, with mutable state, a throwing constructor, clock access and a type-code check.
public class PerishableProduct : Product
{
    public DateTime ExpirationDate { get; set; }
    public DateTime ManufacturedDate { get; set; }

    public PerishableProduct(Ulid id, string name, string slug, DateTime manufacturedDate, DateTime expirationDate)
        : base(id, name, slug)
    {
        if (expirationDate <= manufacturedDate)
            throw new ArgumentException("expiration must be after manufacture");

        ManufacturedDate = manufacturedDate;
        ExpirationDate = expirationDate;
    }

    public bool IsExpired() => DateTime.Now > ExpirationDate;

    public new bool CanSell(DateTime sellByDate)
    {
        return !IsExpired() && base.CanSell(sellByDate);
    }
}

// Second level of inheritance, to make it worse.
public class FreshFoodProduct : PerishableProduct
{
    public int MaxFridgeDays { get; set; }

    public FreshFoodProduct(Ulid id, string name, string slug, DateTime manufacturedDate, DateTime expirationDate, int maxFridgeDays)
        : base(id, name, slug, manufacturedDate, expirationDate)
    {
        MaxFridgeDays = maxFridgeDays;
    }

    // Liskov violations (hiding Product's contract with 'new'): code written against Product
    // gets different behaviour, or an exception, when handed a FreshFoodProduct.

    // Strengthened precondition: Product.CanSell accepts any date; this one refuses past the fridge limit.
    public new bool CanSell(DateTime sellByDate)
    {
        if (sellByDate > DateTime.Now.AddDays(MaxFridgeDays))
            throw new InvalidOperationException("cannot plan a sale beyond the fridge life");

        return base.CanSell(sellByDate);
    }

    // Weakened postcondition: Product.Sell flips the status to out_of_stock at 0; this one never does.
    public new void Sell(int remainingStock)
    {
        UpdatedAt = DateTime.Now;
    }

    // Unsupported inherited behaviour: Product.Deprecate always works; fresh food cannot be deprecated.
    public new void Deprecate()
    {
        throw new NotSupportedException("fresh food is discarded, never deprecated");
    }
}

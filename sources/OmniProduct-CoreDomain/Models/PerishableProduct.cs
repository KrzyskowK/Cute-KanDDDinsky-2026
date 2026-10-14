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
}

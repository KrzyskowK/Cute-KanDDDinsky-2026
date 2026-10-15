namespace OmniProduct_CoreDomain.ValueObjects;

// Margin and Vat are percentages; VAT applies on the margin only.
public sealed record Price(decimal Amount, string Currency, decimal Margin = 20m, decimal Vat = 20m)
{
    public decimal CalculateResellerPrice()
    {
        var marginAmount = Amount * Margin / 100;
        var vatAmount = marginAmount * Vat / 100;
        return Amount + marginAmount + vatAmount;
    }
}

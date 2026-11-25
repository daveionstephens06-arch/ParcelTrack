namespace ParcelTrack.Services
{
    public interface IDeliveryFeeCalculator
    {
        decimal Calculate(decimal weightKg, string? parish);
        dynamic Calculate(double weightKg, string? parish);
    }
}

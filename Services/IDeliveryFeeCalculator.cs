namespace ParcelTrack.Services
{
    public interface IDeliveryFeeCalculator
    {
        decimal Calculate(double weightKg, string? parish);
    }
}

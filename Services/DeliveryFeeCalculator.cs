namespace ParcelTrack.Services
{
    public class DeliveryFeeCalculator : IDeliveryFeeCalculator
    {
        public decimal Calculate(decimal weightKg, string? parish)
        {
            decimal wholeKg = Math.Ceiling(weightKg);
            decimal fee = 500m + (150m * wholeKg);

            bool isKingstonOrStAndrew =
                string.Equals(parish, "Kingston", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(parish, "St Andrew", StringComparison.OrdinalIgnoreCase);

            if (!isKingstonOrStAndrew)
            {
                fee += 300m;
            }

            return fee;
        }

        public dynamic Calculate(double weightKg, string? parish)
        {
            throw new NotImplementedException();
        }
    }
}

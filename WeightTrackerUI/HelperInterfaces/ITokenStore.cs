namespace WeightTrackerUI.HelperInterfaces
{
    public interface ITokenStore
    {
        public static abstract int? GetFarmerId(string token);

        public static abstract string GenerateToken(int farmerId);


    }
}

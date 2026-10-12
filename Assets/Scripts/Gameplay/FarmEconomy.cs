namespace HarvestGrove.Gameplay
{
    public static class FarmEconomy
    {
        public const int FertilizerPrice = 10;
        public const int PlotPrice = 100;

        public static bool CanAfford(int balance, int price)
        {
            return price >= 0 && balance >= price;
        }

        public static bool TryTransaction(int balance, int amount, out int newBalance)
        {
            long result = (long)balance + amount;
            newBalance = balance;
            if (balance < 0 || result < 0 || result > int.MaxValue)
                return false;

            newBalance = (int)result;
            return true;
        }
    }
}

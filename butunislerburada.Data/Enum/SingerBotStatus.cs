namespace butunislerburada.Data.Enum
{
    public enum SingerBotStatus : int
    {
        Hayır = 0,
        Evet = 1
    }

    public class SingerBotStatusValue
    {
        public static string StatusValueName(int StatusID)
        {
            string returnValue = "Hayır";

            if (StatusID == 1)
            {
                returnValue = "Evet";
            }

            return returnValue;
        }
    }
}

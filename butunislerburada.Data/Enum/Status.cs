namespace butunislerburada.Data.Enum
{
    public enum Status : int
    {
        Pasif = 0,
        Aktif = 1
    }

    public class StatusValue
    {
        public static string StatusValueName(int StatusID)
        {
            string returnValue = "Pasif";

            if (StatusID == 1)
            {
                returnValue = "Aktif";
            }

            return returnValue;
        }
    }
}

namespace butunislerburada.Data.Enum
{
    public enum SaveUpdateStatus : int
    {
        Save = 1,
        Update = 2
    }

    public class SaveUpdateStatusValue
    {
        public static string StatusValueName(int StatusID)
        {
            string returnValue = "Kaydedildi";

            if (StatusID == 2)
            {
                returnValue = "Güncellendi";
            }

            return returnValue;
        }
    }
}

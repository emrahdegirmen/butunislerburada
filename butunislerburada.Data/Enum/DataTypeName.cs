namespace butunislerburada.Data.Enum
{
    public enum DataTypeName : int
    {
        Singer = 1,
        Lyrics = 2,
        Blog = 3
    }

    public class DataTypeNameValue
    {
        public static string StatusValueName(int StatusID)
        {
            string returnValue = "Şarkıcı";

            if (StatusID == 2)
            {
                returnValue = "Şarkı Sözü";
            }
            else if (StatusID == 3)
            {
                returnValue = "Blog";
            }

            return returnValue;
        }
    }
}

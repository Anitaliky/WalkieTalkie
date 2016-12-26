using System.Data;

namespace Server.Data
{
    public class RowNotUniqueException : DataException
    {
        public override string Message
        {
            get
            {
                return "Obtained row is not unique.";
            }
        }
    }
}

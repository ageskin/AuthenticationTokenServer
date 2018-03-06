namespace Exiger.JWT.Core.Extensions
{
    public static class BooleanExtensions
    {
        private const string YES_STRING = "Yes";
        private const string NO_STRING = "No";

        public static string ToYesNoString(this bool value)
        {
            return value ? YES_STRING : NO_STRING;
        }        
    }
}

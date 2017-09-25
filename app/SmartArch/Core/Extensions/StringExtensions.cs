namespace SmartArch.Core.Extensions
{
    public static class StringExtensions
    {
         public static string ToUpperFirstLetter(this string str)
         {
             if (string.IsNullOrEmpty(str))
             {
                 return str;
             }

             char[] charArray = str.ToCharArray();
             charArray[0] = char.ToUpper(charArray[0]);

             return new string(charArray);
         }
    }
}
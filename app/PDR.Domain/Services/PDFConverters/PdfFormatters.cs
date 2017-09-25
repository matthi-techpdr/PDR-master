using System;

namespace PDR.Domain.Services.PDFConverters
{
    public static class PdfFormatters
    {
         public static string ToPhoneFormat(this string phone)
         {
             try
             {
                 var code = phone.Substring(0, 3);
                 var first = phone.Substring(3, 3);
                 var second = phone.Substring(6, 4);
                 var baseNumber = string.Format("({0}) {1}-{2}", code, first, second);
                 return phone.Length > 10 ? string.Format("{0} X{1}", baseNumber, phone.Substring(10)) : baseNumber;
             }
             catch (Exception)
             {
                 return phone;
             }
         }
    }
}
using System;

namespace PDR.Domain.Services.GeneratePassword
{
    public static class PasswordGenerator
    {
         public static string Generate()
         {
             var password = Guid.NewGuid().ToString().Substring(0, 8);
             return password;
         }
    }
}
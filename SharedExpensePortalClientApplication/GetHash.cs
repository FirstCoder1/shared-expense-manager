using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SharedExpensePortalClientApplication
{
    public static class GetHash
    {
        public static string GetHashForString(string str)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(str))
                );
        }
    }
}
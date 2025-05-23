﻿using System.Security.Cryptography;
using System.Text;

namespace CabPaymentService.Infrastructures.Helpers
{
    public class EncryptionHelper
    {
        public static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        
        public static string Sha256(string key, string inputData)
        {
            throw new NotImplementedException();
        }
    }
}

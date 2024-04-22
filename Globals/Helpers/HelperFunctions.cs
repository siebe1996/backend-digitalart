using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Helpers
{
    public class HelperFunctions
    {
        public static byte[] ConvertBase64ToByteArray(string base64String)
        {
            try
            {
                // Convert Base64 String to byte[]
                return Convert.FromBase64String(base64String);
            }
            catch (FormatException ex)
            {
                // Handle the situation where the string is not a valid Base64 string.
                Console.WriteLine("Error converting Base64 to byte array: " + ex.Message);
                return null; // or handle the error appropriately
            }
        }
    }
}

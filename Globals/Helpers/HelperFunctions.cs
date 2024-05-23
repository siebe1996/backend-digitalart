using Globals.Interfaces;
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

        public static List<Guid> WeightedRandomSelection<T>(List<T> items, int count) where T : IWeighted
        {
            var random = new Random();
            var selected = new List<Guid>();

            // Implement a simple weighted random selection (e.g., roulette wheel approach)
            var totalWeight = items.Sum(x => x.Score);
            while (selected.Count < count && selected.Count < items.Count)
            {
                var randomNumber = random.NextDouble() * totalWeight;
                var cumulativeWeight = 0.0;
                foreach (var item in items)
                {
                    cumulativeWeight += item.Score;
                    if (randomNumber <= cumulativeWeight)
                    {
                        selected.Add(item.Id);
                        break;
                    }
                }
            }

            return selected.Distinct().ToList();
        }
    }
}

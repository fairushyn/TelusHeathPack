using System;
using System.Collections.Generic;

namespace TelusHeathPack.Models
{
    public class User
    {
        public string Id { get; set; }
        public string Alias { get; set; }
        public int TestNumber { get; set; }
        public bool IsApproved { get; set; }

        public static int GenerateNumber()
        {
            var ran = new Random(DateTime.Now.Millisecond);
            var keys = new List<int>();
            var key = 0;
            
            do
            {
                key = ran.Next(1000000000, int.MaxValue);

            } while (keys.Contains(key));

            keys.Add(key);
            return key;
        }
    }
}

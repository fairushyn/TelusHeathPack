using System;

namespace TelusHeathPack.Models
{
    public class UserCredential
    {
        public string Alias { get; set; }
        public int TestNumber { get; set; }
    }
    
    public class AddUser
    {
        public string Alias { get; set; }
    }
}
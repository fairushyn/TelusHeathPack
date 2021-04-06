namespace TelusHeathPack.Models
{
    public class UserInfo
    {
        public string FullName { get; set; }
        
        public int Age { get; set; }
        public override string ToString() => FullName;
    }
}
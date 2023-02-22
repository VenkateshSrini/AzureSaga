namespace AzureSaga.Domain
{
   enum UserType
    {
        Coach = 0,
        Fan=1
    }
    public class User
    {
        public string? Id { get;set; }
        public string? Username { get;set; }
        public string? Avatar { get;set; }

    }
}
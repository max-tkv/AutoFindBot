namespace AutoFindBot.Entities
{
    public class AppUser : BaseEntity
    {
        public long ChatId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Tarif Tarif { get; set; }
        public bool Confirm { get; set; }
        
        public virtual List<Action> Actions { get; set; } = new List<Action>();
        public virtual List<Payment> Payments { get; set; } = new List<Payment>();
        public virtual List<Car> Cars { get; set; } = new List<Car>();
        public virtual List<UserFilter> UserFilters { get; set; } = new List<UserFilter>();
    }
}
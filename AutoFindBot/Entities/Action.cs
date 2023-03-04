namespace AutoFindBot.Entities
{
    public class Action : BaseEntity
    {
        public virtual long UserId { get; set; }
        public string? CommandName { get; set; }
        public Categories? Category { get; set; }
        public int Page { get; set; }
        public string? ActionText { get; set; }
        public string? PageData { get; set; }
        
        public virtual AppUser User { get; set; }
    }
}
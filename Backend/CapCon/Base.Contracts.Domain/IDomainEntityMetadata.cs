namespace Base.Contracts.Domain;

public interface IDomainEntityMetadata
{
    public string CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }
}
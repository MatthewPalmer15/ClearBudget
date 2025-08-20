using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace ClearBudget.Database.Entities;

/// <summary>
///     Represents a base entity with common properties such as ID, creation date, and status.
/// </summary>
public class BaseEntity<T> : IBaseEntity<T> where T : struct
{
    [Key]
    [XmlElement("Id")]
    [JsonProperty("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public T Id { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the entity has been marked as deleted.
    /// </summary>
    [XmlElement("Deleted")]
    [JsonProperty("deleted")]
    public bool Deleted { get; set; }
}

public interface IBaseEntity<T> where T : struct
{
    public T Id { get; set; }
    public bool Deleted { get; set; }
}
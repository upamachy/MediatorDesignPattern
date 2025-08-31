using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DemoLibrary.Models
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<ObjectId>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;
        
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
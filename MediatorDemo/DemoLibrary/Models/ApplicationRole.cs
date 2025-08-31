using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using MongoDB.Bson;

namespace DemoLibrary.Models
{
    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole<ObjectId>
    {
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
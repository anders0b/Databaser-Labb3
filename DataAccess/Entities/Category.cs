using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;

namespace DataAccess.Entities;

public class Category
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
}
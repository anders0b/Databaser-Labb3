using Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccess.Entities;

public class Question
{
    public ObjectId Id { get; set; }
    public Category Category { get; set; }
    public string Content { get; set; }
    public List<string> Answers { get; set; }
    public string CorrectAnswer { get; set; }
}
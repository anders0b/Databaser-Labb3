using Common.DTO;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess.Services;

public class CategoryRepository
{
    private readonly IMongoCollection<Category> _category;
    public static event Action CategoryListChanged;

    public CategoryRepository()
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "QuizDB";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _category = database.GetCollection<Category>("Categories", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }
    public void AddCategory(CategoryRecord categoryRecord)
    {
        var newC = new Category();
        newC.Name = categoryRecord.Name;
        _category.InsertOne(newC);
        CategoryListChanged.Invoke();
    }

    public IEnumerable<CategoryRecord> GetAllCategories()
    {
        var filter = Builders<Category>.Filter.Empty;
        var allCategories = _category.Find(filter).ToList().Select(c => new CategoryRecord(c.Id.ToString(), c.Name));
        return allCategories;
    }

    public void UpdateCategory(CategoryRecord categoryRecord)
    {
        var filter = Builders<Category>.Filter.Eq("_id", ObjectId.Parse(categoryRecord.Id));
        var update = Builders<Category>.Update
            .Set(c => c.Name, categoryRecord.Name);
        _category.UpdateOne(filter, update);
        CategoryListChanged.Invoke();
    }

    public void DeleteCategory(string id)
    {
        var filter = Builders<Category>.Filter.Eq("_id", ObjectId.Parse(id));
        _category.DeleteOne(filter);
        CategoryListChanged.Invoke();
    }

    public void CreateInitialCategories()
    {
        var music = new CategoryRecord("", "Music");
        var geography = new CategoryRecord("", "Geography");
        var history = new CategoryRecord("", "History");
        var mixed = new CategoryRecord("", "Mixed");
        AddCategory(music);
        AddCategory(geography);
        AddCategory(history);
        AddCategory(mixed);
    }
}
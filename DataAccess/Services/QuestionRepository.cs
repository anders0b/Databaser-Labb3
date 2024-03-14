using Common.DTO;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess.Services;

public class QuestionRepository
{
    private readonly IMongoCollection<Question> _question;
    public static event Action QuestionListChanged;

    public QuestionRepository()
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "QuizDB";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _question = database.GetCollection<Question>("Question", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }
    public void AddQuestion(QuestionRecord questionRecord)
    {
        var categoryRep = new CategoryRepository();
        var selectedCategory = questionRecord.Category;
        var newQ = new Question();
        newQ.Category = new Category { Id = ObjectId.Parse(questionRecord.Category.Id), Name = questionRecord.Category.Name };
        newQ.Content = questionRecord.Content;
        newQ.Answers = questionRecord.Answers;
        newQ.CorrectAnswer = questionRecord.CorrectAnswer;
        _question.InsertOne(newQ);
        QuestionListChanged.Invoke();
    }
    public IEnumerable<QuestionRecord> GetAllQuestions()
    {
        var filter = Builders<Question>.Filter.Empty;
        var allQuestions = _question.Find(filter).ToList().Select(q =>
            new QuestionRecord(q.Id.ToString(),new CategoryRecord(q.Category.Id.ToString(), q.Category.Name), q.Content, q.Answers, q.CorrectAnswer));
        return allQuestions;
    }

    public void UpdateQuestion(QuestionRecord questionRecord)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(questionRecord.Id));
        var update = Builders<Question>.Update
            .Set(q => q.Category, new Category()
            {
                Id = ObjectId.Parse(questionRecord.Id),
                Name = questionRecord.Category.Name
            })
            .Set(q => q.Content, questionRecord.Content)
            .Set(q => q.Answers, questionRecord.Answers)
            .Set(q => q.CorrectAnswer, questionRecord.CorrectAnswer);

        _question.UpdateOne(filter, update);
        QuestionListChanged.Invoke();
    }

    public void DeleteQuestion(string id)
    {
        var filter = Builders<Question>.Filter.Eq("_id", ObjectId.Parse(id));
        _question.DeleteOne(filter);
        QuestionListChanged.Invoke();
    }
}
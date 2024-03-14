using Common.DTO;
using DataAccess.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataAccess.Services;

public class QuizRepository
{
    private readonly IMongoCollection<Quiz> _quiz;
    public static event Action QuizListChanged;

    public QuizRepository()
    {
        var hostName = "localhost";
        var port = "27017";
        var databaseName = "QuizDB";
        var client = new MongoClient($"mongodb://{hostName}:{port}");
        var database = client.GetDatabase(databaseName);
        _quiz = database.GetCollection<Quiz>("Quiz", new MongoCollectionSettings() { AssignIdOnInsert = true });
    }

    public void AddQuiz(QuizRecord quizRecord)
    {
        var newQuiz = new Quiz();
        newQuiz.Category = new Category{Id = ObjectId.Parse(quizRecord.Category.Id), Name = quizRecord.Category.Name };
        newQuiz.Name = quizRecord.Name;
        newQuiz.Description = quizRecord.Description;
        newQuiz.Questions = new List<Question>();
        foreach (var question in quizRecord.Questions)
        {
            var q = new Question()
            {
                Id = ObjectId.Parse(question.Id),
                Content = question.Content,
                Answers = question.Answers,
                CorrectAnswer = question.CorrectAnswer
            };
            newQuiz.Questions.Add(q);
        }
        _quiz.InsertOne(newQuiz);
        QuizListChanged.Invoke();
    }

    public IEnumerable<QuizRecord> GetAllQuizzes()
    {
        var filter = Builders<Quiz>.Filter.Empty;
        var allQuizzes = _quiz.Find(filter).ToList().Select(q =>
            new QuizRecord(q.Id.ToString(), new CategoryRecord(q.Category.Id.ToString(), q.Category.Name), q.Name, q.Description,
                q.Questions.ConvertAll(input => new QuestionRecord(input.Id.ToString(),
                    new CategoryRecord(
                        q.Category.Id.ToString(),
                        q.Category.Name),
                    input.Content,
                    input.Answers, input.CorrectAnswer))));
        return allQuizzes;
    }

    public void UpdateQuiz(QuizRecord quizRecord)
    {
        var newQuestions = new List<Question>();
        foreach (var question in quizRecord.Questions)
        {
            var q = new Question()
            {
                Id = ObjectId.Parse(question.Id),
                Category = new Category{Id = ObjectId.Parse(question.Category.Id), Name = question.Category.Name},
                Content = question.Content,
                Answers = question.Answers,
                CorrectAnswer = question.CorrectAnswer
            };
            newQuestions.Add(q);
            QuizListChanged.Invoke();
        }

        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(quizRecord.Id));
        var update = Builders<Quiz>.Update
            .Set(q => q.Category, new Category()
            {
                Id = ObjectId.Parse(quizRecord.Id),
                Name = quizRecord.Category.Name
            })
            .Set(q => q.Name, quizRecord.Name)
            .Set(q => q.Description, quizRecord.Description)
            .Set(q => q.Questions, newQuestions);

        _quiz.UpdateOne(filter, update);
        QuizListChanged.Invoke();
    }

    public void RemoveQuiz(string id)
    {
        var filter = Builders<Quiz>.Filter.Eq("_id", ObjectId.Parse(id));
        _quiz.DeleteOne(filter);
        QuizListChanged.Invoke();
    }
}
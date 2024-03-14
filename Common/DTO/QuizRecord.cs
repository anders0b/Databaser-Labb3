namespace Common.DTO;

public record QuizRecord(string Id, CategoryRecord Category, string Name, string Description,
    List<QuestionRecord> Questions);


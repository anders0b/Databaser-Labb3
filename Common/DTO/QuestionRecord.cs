namespace Common.DTO;

public record QuestionRecord(string Id, CategoryRecord Category, string Content, List<string> Answers, string CorrectAnswer);
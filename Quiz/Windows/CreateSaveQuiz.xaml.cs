using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Common.DTO;
using DataAccess.Services;
using MongoDB.Bson;

namespace Quiz.Windows
{
    /// <summary>
    /// Interaction logic for CreateSaveQuiz.xaml
    /// </summary>
    public partial class CreateSaveQuiz : Window
    {
        public CreateSaveQuiz()
        {
            InitializeComponent();
            var categoryRep = new CategoryRepository();
            var list = categoryRep.GetAllCategories();
            foreach (var category in list)
            {
                QuizCategoryBox.Items.Add(category.Name);
            }
        }

        private void AddQBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuizId.Text != "")
            {
                var quizRepository = new QuizRepository();
                var findQuizzes = quizRepository.GetAllQuizzes();
                var quizToEdit = findQuizzes.FirstOrDefault(i => i.Id == QuizId.Text);
                var addQuestionsWindow = new QuestionList();
                foreach (var questions in quizToEdit.Questions)
                {
                    addQuestionsWindow.QuestionsAdded.Items.Add(questions);
                }
                var questionRepository = new QuestionRepository();
                var questionList = questionRepository.GetAllQuestions();
                foreach (var question in questionList)
                {
                    addQuestionsWindow.QuestionsToAdd.Items.Add(question);
                }
                addQuestionsWindow.Show();
            }
            else
            {
                return;
            }

        }
        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var categoryRepository = new CategoryRepository();
            var list = categoryRepository.GetAllCategories();
            var selectedCategory = list.FirstOrDefault(c => c.Name == QuizCategoryBox.Text);
            if (selectedCategory == null)
            {
                MessageBox.Show("Please select a category", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var quizRepository = new QuizRepository();
            if (QuizId.Text == "")
            {
                var quizToAdd = new QuizRecord("", selectedCategory, QuizName.Text, QuizDescription.Text, new List<QuestionRecord>());
                foreach (var question in QuestionList.Items)
                {
                    quizToAdd.Questions.Add((QuestionRecord)question);
                }
                quizRepository.AddQuiz(quizToAdd);
                Close();
            }
            else
            {
                var quizToUpdate = new QuizRecord(QuizId.Text, selectedCategory, QuizName.Text, QuizDescription.Text, new List<QuestionRecord>());
                foreach (var question in QuestionList.Items)
                {
                    quizToUpdate.Questions.Add((QuestionRecord)question);
                }
                quizRepository.UpdateQuiz(quizToUpdate);
                Close();
            }
        }
    }
}

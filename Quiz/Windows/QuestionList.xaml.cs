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
    /// Interaction logic for QuestionList.xaml
    /// </summary>
    public partial class QuestionList : Window
    {
        public QuestionList()
        {
            InitializeComponent();
            var questionRepository = new QuestionRepository();
            var questionList = questionRepository.GetAllQuestions();
            foreach (var question in questionList)
            {
                QuestionsToAdd.Items.Add(question);
            }
            var categoryRepository = new CategoryRepository();
            var categoryList = categoryRepository.GetAllCategories();
            foreach (var category in categoryList)
            {
                CategoryFilter.Items.Add(category.Name);
            }
        }

        private void AddQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuestionsToAdd.SelectedItem is QuestionRecord selectedQuestion)
            {
                    QuestionsAdded.Items.Add(selectedQuestion);
            }
        }

        private void RemoveQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuestionsAdded.SelectedItem is QuestionRecord selectedQuestion)
            {
                QuestionsAdded.Items.Remove(selectedQuestion);
            }
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var quizRepository = new QuizRepository();
            var quizList = quizRepository.GetAllQuizzes();
            var selectedQuiz = quizList.FirstOrDefault(q => q.Id == QuizId.Text);
            selectedQuiz.Questions.Clear();
            foreach (QuestionRecord question in QuestionsAdded.Items)
            {
                selectedQuiz.Questions.Add(question);
            }
            quizRepository.UpdateQuiz(selectedQuiz);
            Close();
        }

        private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var filter = SearchBox.Text;
            QuestionsToAdd.Items.Clear();
            var questionRepository = new QuestionRepository();
            var questionList = questionRepository.GetAllQuestions();
            var filteredQuestionList = questionList.Where(q => q.Content.Contains(filter));
            foreach (var question in filteredQuestionList)
            {
                QuestionsToAdd.Items.Add(question);
            }
        }

        private void ResetFilter_OnClick(object sender, RoutedEventArgs e)
        {
            QuestionsToAdd.Items.Clear();
            var questionRepository = new QuestionRepository();
            var questionList = questionRepository.GetAllQuestions();
            foreach (var question in questionList)
            {
                QuestionsToAdd.Items.Add(question);
            }

            CategoryFilter.Text = "";
            SearchBox.Text = "";
        }

        private void CategoryFilter_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilter.SelectedValue != null)
            {
                var filter = CategoryFilter.SelectedValue as string;
                
                QuestionsToAdd.Items.Clear();
                var questionRepository = new QuestionRepository();
                var questionList = questionRepository.GetAllQuestions();
                var filteredQuestionList = questionList.Where(q => q.Category.Name == filter).ToList();
                foreach (var question in filteredQuestionList)
                {
                    QuestionsToAdd.Items.Add(question);
                }
            }
        }
    }
}

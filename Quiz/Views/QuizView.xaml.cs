using System.Configuration;
using System.Windows;
using DataAccess.Services;
using System.Windows.Controls;
using System.Windows.Input;
using Common.DTO;
using Quiz.Windows;
using System.Text.RegularExpressions;

namespace Quiz.Views
{
    /// <summary>
    /// Interaction logic for QuizView.xaml
    /// </summary>
    public partial class QuizView : UserControl
    {
        public QuizView()
        {
            InitializeComponent();
            var quizrepository = new QuizRepository();
            var qrepository = new QuestionRepository();
            var quizlist = quizrepository.GetAllQuizzes();
            foreach (var quiz in quizlist)
            {
                QuizList.Items.Add(quiz);
            }
            var qlist = qrepository.GetAllQuestions();
            foreach (var q in qlist)
            {
                QuestionList.Items.Add(q);
            }
            QuizRepository.QuizListChanged += QuizRepository_QuizListCHanged;
            QuestionRepository.QuestionListChanged += QuestionRepository_QuestionListChanged;
            CategoryRepository.CategoryListChanged += CategoryRepository_CategoryListChanged;
            var categoryRepository = new CategoryRepository();
            var categoryList = categoryRepository.GetAllCategories();
            if (categoryList.Any() == false)
            {
                categoryRepository.CreateInitialCategories();
                MessageBox.Show("This app is designed to create exciting quizzes for you to use. For starters a few categories have been created for you to use, but feel free to add more.",
                    "Welcome!",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                foreach (var category in categoryList)
                {
                    CategoryFilter.Items.Add(category.Name);
                }
            }
        }

        private void CategoryRepository_CategoryListChanged()
        {
            CategoryFilter.Items.Clear();
            var categoryRepository = new CategoryRepository();
            var categoryList = categoryRepository.GetAllCategories();
            foreach (var category in categoryList)
            {
                CategoryFilter.Items.Add(category.Name);
            }
        }

        private void QuizRepository_QuizListCHanged()
        {
            QuizList.Items.Clear();
            var quizRepository = new QuizRepository();
            var quizList = quizRepository.GetAllQuizzes();
            foreach (var quiz in quizList)
            {
                QuizList.Items.Add(quiz);
            }
        }
        private void QuestionRepository_QuestionListChanged()
        {
            QuestionList.Items.Clear();
            var questionRepository = new QuestionRepository();
            var questionList = questionRepository.GetAllQuestions();
            foreach (var q in questionList)
            {
                QuestionList.Items.Add(q);
            }
        }

        private void CreateSaveQuizBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var create = new CreateSaveQuiz();
            create.Show();
        }

        private void EditBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuizList.SelectedItem is QuizRecord selectedQuiz)
            {
                var edit = new CreateSaveQuiz();
                edit.QuizId.Text = selectedQuiz.Id;
                edit.QuizName.Text = selectedQuiz.Name;
                edit.QuizCategoryBox.SelectedItem = selectedQuiz.Category.Name;
                edit.QuizDescription.Text = selectedQuiz.Description;
                foreach (var question in selectedQuiz.Questions)
                {
                    edit.QuestionList.Items.Add(question);
                }
                edit.Show();
            }
            if (QuestionList.SelectedItem is QuestionRecord selectedQuestion)
            {
                var edit = new CreateSaveQuestion();
                edit.QuestionId.Text = selectedQuestion.Id;
                edit.QuestionCategoryBox.SelectedItem = selectedQuestion.Category.Name;
                edit.ContentBox.Text = selectedQuestion.Content;
                edit.Answer1Box.Text = selectedQuestion.Answers[0];
                edit.Answer2Box.Text = selectedQuestion.Answers[1];
                edit.Answer3Box.Text = selectedQuestion.Answers[2];
                edit.CorrectAnswerBox.Text = selectedQuestion.CorrectAnswer;
                edit.Show();
            }
        }

        private void QuizList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuestionList.SelectedItems.Clear();
        }

        private void QuestionList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QuizList.SelectedItems.Clear();
        }

        private void CreateSaveQuestionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var create = new CreateSaveQuestion();
            create.Show();
        }

        private void CreateSaveCategoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var categoryWindow = new CreateSaveCategory();
            categoryWindow.Show();
        }
        private void AddRemoveQuestionsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuizList.SelectedItem is QuizRecord selectedQuiz)
            {
                var questionList = new QuestionList();
                questionList.QuizNameBlock.Text = selectedQuiz.Name + "'s questions";
                questionList.QuizId.Text = selectedQuiz.Id;
                foreach (QuestionRecord question in selectedQuiz.Questions)
                {
                    questionList.QuestionsAdded.Items.Add(question);
                }
                questionList.Show();
            }
        }

        private void CategoryFilter_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryFilter.SelectedValue != null)
            {
                var filter = CategoryFilter.SelectedValue as string;
                QuestionList.Items.Clear();
                var questionRepository = new QuestionRepository();
                var questionList = questionRepository.GetAllQuestions();
                var filteredQuestionList = questionList.Where(q => q.Category.Name == filter).ToList();
                foreach (var question in filteredQuestionList)
                {
                    QuestionList.Items.Add(question);
                }
            }

        }

        private void ResetFilter_OnClick(object sender, RoutedEventArgs e)
        {
            QuestionRepository_QuestionListChanged();
            CategoryFilter.Text = "";
        }

        private void RemoveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (QuizList.SelectedItem is QuizRecord selectedQuiz)
            {
                var quizRepository = new QuizRepository();
                quizRepository.RemoveQuiz(selectedQuiz.Id);

            }
            if (QuestionList.SelectedItem is QuestionRecord selectedQuestion)
            {
                var questionRepository = new QuestionRepository();
                questionRepository.DeleteQuestion(selectedQuestion.Id);
            }
        }
    }
}

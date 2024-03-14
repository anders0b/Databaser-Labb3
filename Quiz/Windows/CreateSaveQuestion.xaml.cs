using Common;
using Common.DTO;
using DataAccess.Entities;
using DataAccess.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Quiz.Windows
{
    /// <summary>
    /// Interaction logic for CreateSaveQuestion.xaml
    /// </summary>
    public partial class CreateSaveQuestion : Window
    {
        public CreateSaveQuestion()
        {
            InitializeComponent();
            var categoryRep = new CategoryRepository();
            var list = categoryRep.GetAllCategories();
            foreach (var category in list)
            {
                QuestionCategoryBox.Items.Add(category.Name);
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
            var selectedCategory = list.FirstOrDefault(c => c.Name == QuestionCategoryBox.Text);
            if (selectedCategory == null)
            {
                MessageBox.Show("Please select a category", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var questionRepository = new QuestionRepository();
            if (QuestionId.Text == "")
            {
                var questionToAdd = new QuestionRecord("", selectedCategory,
                    ContentBox.Text, new List<string> { Answer1Box.Text, Answer2Box.Text, Answer3Box.Text },
                    CorrectAnswerBox.Text);
                bool matchAnswer = questionToAdd.Answers.Contains(CorrectAnswerBox.Text);
                if (matchAnswer == false)
                {
                    MessageBox.Show("The correct answer must match one of the answer options", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else
                {
                    questionRepository.AddQuestion(questionToAdd);
                    Close();
                }
            }
            else
            {
                var questionToUpdate = new QuestionRecord(QuestionId.Text, selectedCategory,
                    ContentBox.Text, new List<string> { Answer1Box.Text, Answer2Box.Text, Answer3Box.Text },
                    CorrectAnswerBox.Text);
                bool matchAnswer = questionToUpdate.Answers.Contains(CorrectAnswerBox.Text);
                if (matchAnswer == false)
                {
                    MessageBox.Show("The correct answer must match one of the answer options", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    questionRepository.UpdateQuestion(questionToUpdate);
                    Close();
                }
            }
        }
    }
}

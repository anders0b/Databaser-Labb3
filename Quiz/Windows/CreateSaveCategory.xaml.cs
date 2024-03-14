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
using DataAccess.Entities;
using DataAccess.Services;

namespace Quiz.Windows
{
    /// <summary>
    /// Interaction logic for CreateSaveCategory.xaml
    /// </summary>
    public partial class CreateSaveCategory : Window
    {
        public CreateSaveCategory()
        {
            InitializeComponent();
            var categoryRep = new CategoryRepository();
            var list = categoryRep.GetAllCategories();
            foreach (var category in list)
            {
                CategoryList.Items.Add(category);
            }
            CategoryRepository.CategoryListChanged += CategoryRepository_CategoryListChanged;

        }
        private void CategoryRepository_CategoryListChanged()
        {
            CategoryList.Items.Clear();
            var categoryRepository = new CategoryRepository();
            var categoryList = categoryRepository.GetAllCategories();
            foreach (var category in categoryList)
            {
                CategoryList.Items.Add(category);
            }
        }

        private void CloseWindowBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RemoveCategoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (CategoryList.SelectedItem is CategoryRecord selectedCategory)
            {
                var categoryRep = new CategoryRepository();
                categoryRep.DeleteCategory(selectedCategory.Id);
                MessageBox.Show("You removed " + selectedCategory.Name, "" , MessageBoxButton.OK);
                CategoryList.Items.Clear();
                var list = categoryRep.GetAllCategories();
                foreach (var category in list)
                {
                    CategoryList.Items.Add(category);
                }
            }
        }

        private void UpdateCategoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            if (CategoryList.SelectedItem is CategoryRecord selectedCategory)
            {
                var categoryRep = new CategoryRepository();
                var categoryToUpdate = new CategoryRecord(selectedCategory.Id, CategoryNameBox.Text);
                categoryRep.UpdateCategory(categoryToUpdate);
                MessageBox.Show("Successfully renamed","Done", MessageBoxButton.OK);
            }
        }

        private void CreateCategoryBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var categoryRep = new CategoryRepository();
            var categoryList = categoryRep.GetAllCategories();
            var check = categoryList.FirstOrDefault(c => c.Name == CategoryNameBox.Text);
            if (check != null)
            {
                MessageBox.Show("A category with this name already exists", "Error", MessageBoxButton.OK);
            }
            else
            {
                var categoryToAdd = new CategoryRecord("", CategoryNameBox.Text);
                categoryRep.AddCategory(categoryToAdd);
                MessageBox.Show("You created " + categoryToAdd.Name, "Done", MessageBoxButton.OK);
                CategoryList.Items.Clear();
                var list = categoryRep.GetAllCategories();
                foreach (var category in list)
                {
                    CategoryList.Items.Add(category);
                }
            }
        }

        private void CategoryList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryList.SelectedItem is CategoryRecord selectedCategory)
            {
                CategoryNameBox.Text = selectedCategory.Name;
            }
        }
    }
}

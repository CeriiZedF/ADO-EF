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

namespace ADO_EF
{
    /// <summary>
    /// Логика взаимодействия для CrudDepartmentWindow.xaml
    /// </summary>
    public partial class CrudDepartmentWindow : Window
    {
        public Data.Entity.Department? Department { get; set; }
        public CrudDepartmentWindow()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Department.Name = NameTextBox.Text;
            this.DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IdTextBox.Text = Department?.Id.ToString() ?? "";
            NameTextBox.Text = Department?.Name?.ToString() ?? "";
        }

        private void SoftDeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void HardDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Department = null;
            DialogResult = true;
        }
    }
}

using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using Microsoft.Win32;
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

namespace ClassroomAssignment
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Course> _sortedCourses;
        public MainWindow()
        {
            InitializeComponent();
            _sortedCourses = InMemoryCourseRepository.getInstance().Courses;
            _sortedCourses.Sort(CompareCourses);
            CoursesDataGrid.ItemsSource = _sortedCourses;
        }

        private int CompareCourses(Course c1, Course c2)
        {
            var value1 = CourseValue(c1);
            var value2 = CourseValue(c2);
            if (value1 == value2)
            {
                var val1 = int.Parse(c1.ClassID);
                var val2 = int.Parse(c2.ClassID);
                return val1 - val2;
            }
            else return value2 - value1;
        }

        private static int CourseValue(Course course)
        {
            if (course.NeedsRoom)
            {
                if (!course.AlreadyAssignedRoom) return 4;
                else return 3;
            }
            else return 2;
        }

        private void Menu_Export(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Worksheets|*.xls";
            if (saveFileDialog.ShowDialog() == true)
            {
                var fileName = saveFileDialog.FileName;
            }
            
        }

        
    }
}

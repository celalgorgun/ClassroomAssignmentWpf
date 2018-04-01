using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Model.Repo
{
    class InMemoryCourseRepository : ICourseRepository
    {
        private static InMemoryCourseRepository _instance;
        private ObservableCollection<Course> _courses;

        public event EventHandler<CourseCollectionEventArgs> CourseModified;
        public event EventHandler<CourseCollectionEventArgs> CourseAdded;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICollection<Course> Courses {
            get => _courses;
        }

        public static InMemoryCourseRepository GetInstance()
        {
            return _instance;
        }
        public static void initInstance(ICollection<Course> courses)
        {
            _instance = new InMemoryCourseRepository(courses);
        }

        private InMemoryCourseRepository(ICollection<Course> courses)
        {
            _courses = new ObservableCollection<Course>(courses);
            _courses.CollectionChanged += _courses_CollectionChanged;
        }

        private void _courses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CourseCollectionEventArgs eventArgs = new CourseCollectionEventArgs(CourseCollectionEventArgs.EventType.Added, e.NewItems);
        }

        public class CourseCollectionEventArgs : EventArgs
        {
            public enum EventType { Modified, Added, Deleted };
            public readonly EventType Type;
            public readonly ICollection<Course> CoursesInvolved;

            public CourseCollectionEventArgs(EventType eventType, ICollection courses)
            {
                Type = eventType;
                CoursesInvolved = new List<Course>(courses);
            }
        }



    }
}

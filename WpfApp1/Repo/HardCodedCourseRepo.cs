using ClassroomAssignment.Model;
using ClassroomAssignment.Model.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Repo
{
    public class HardCodedCourseRepo : ICourseRepository
    {

        public List<Course> Courses { get; }

        public HardCodedCourseRepo()
        {
            this.Courses = new List<Course>();

            Course testCourse = new Course();
            testCourse.ClassID = "239";
            testCourse.SIS_ID = "12615";
            testCourse.TermCode = "1188";
            testCourse.DepartmentCode = "UNO-BIOI";
            testCourse.SubjectCode = "BIOI";
            testCourse.CatalogNumber = "1000";
            testCourse.Course_Title = "BIOI 1000";
            testCourse.Section_Number = "1";
            testCourse.MeetingPattern = "MW 1:30pm-2:45pm";
            testCourse.Instructor = "Bastola, Dhundy";
            testCourse.Room = "General Assignment Room";
            testCourse.Status = "Active";
            testCourse.Session = "Regular Academic Session";
            testCourse.Campus = "UNO";
            testCourse.InstructionMethod = "In Person";
            testCourse.Comments = "PKI 153";
            testCourse.Notes = "PKI 157";
            testCourse.AlreadyAssignedRoom = true;
            testCourse.StartTime = new TimeSpan(13, 30, 0);
            testCourse.EndTime = new TimeSpan(14, 45, 0);

            // This is the "normalized" name for the room
            testCourse.RoomAssignment = "PKI 157";
            testCourse.MeetingDays = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Wednesday };

            Courses.Add(testCourse);

        }
    }
}

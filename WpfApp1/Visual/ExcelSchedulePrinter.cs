using ClassroomAssignment.Model.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using static ClassroomAssignment.Model.ClassScheduleTemplate;
using System.Collections.Specialized;
using System.Collections;

namespace ClassroomAssignment.Model.Visual
{
    class ExcelSchedulePrinter : ISchedulePrinter
    {
        static Tuple<int, int> RoomNameLocation = Tuple.Create<int, int>(2, 0);
        static Tuple<int, int> RoomCapacityLocation = Tuple.Create<int, int>(2, 1);
        static Tuple<int,int> TimeHeaderLocation = Tuple.Create<int, int>(4, 1);
        static int CellSpanPerTimeInterval = 2;
        static int startTimeLocationRow = 6;

        static TimeSpan StartTime = new TimeSpan(7, 30, 0);
        static TimeSpan EndTime = new TimeSpan(22, 0, 0);
        static TimeSpan TimeInterval = new TimeSpan(0, 30, 0);
        
        static Dictionary<TimeSpan, int> TimeMap = new Dictionary<TimeSpan, int>();
        static Dictionary<DayOfWeek, int> DayMap = new Dictionary<DayOfWeek, int>();

        private IWorkbook _workbook;
        public IWorkbook Workbook
        {
            get
            {
                return _workbook;
            }
            set
            {
                _workbook = value;
                scheduleTemplate = _workbook.GetSheet(ClassScheduleTemplate.SCHEDULE_TEMPLATE_NAME);
            }
        }

        private ISheet scheduleTemplate;

        static ExcelSchedulePrinter()
        {

            // initialize TimeMap, maps times to row location
            TimeMap.Add(StartTime, startTimeLocationRow);
            var currTime = StartTime;
            var currRow = startTimeLocationRow;
            while(currTime.CompareTo(EndTime) < 0)
            {
                currTime = currTime.Add(TimeInterval);
                currRow += CellSpanPerTimeInterval;
                TimeMap.Add(currTime, currRow);
            }

            // initialize DayMap: Maps days to column locations
            DayMap.Add(DayOfWeek.Monday, 2);
            DayMap.Add(DayOfWeek.Tuesday, 3);
            DayMap.Add(DayOfWeek.Wednesday, 4);
            DayMap.Add(DayOfWeek.Thursday, 6);
            DayMap.Add(DayOfWeek.Friday, 7);

        }
        

        public void Print(ICourseRepository courseRepo, IRoomRepository roomRepo)
        {
            List<Course> courses = courseRepo.Courses;
            IEnumerable<Course> roomedCourses = courses.Where<Course>(x => x.AlreadyAssignedRoom && x.MeetingDays != null);
            Dictionary<string, List<Course>> roomCourseMap = getRoomNameToCoursesMap(roomedCourses);

            List<string> roomWithCourses = roomCourseMap.Keys.ToList<string>();

            foreach (string room in roomWithCourses)
            {
                
                ISheet sheet = Workbook.CloneSheet(Workbook.GetSheetIndex(scheduleTemplate));
                var sheetIndex = Workbook.GetSheetIndex(sheet);
                Workbook.SetSheetName(sheetIndex, room);
                Workbook.SetSheetHidden(sheetIndex, SheetState.Visible);

                ICell cell = sheet.GetRow(RoomNameLocation.Item1).GetCell(RoomNameLocation.Item2);
                cell.SetCellValue(room);
                
                PrintCourses(sheet, roomCourseMap[room]);
                printLegend(sheet);
            }            
        }

        private void printLegend(ISheet sheet)
        {
            CellReference cellReference = new CellReference("J5");
            int rowIndex = cellReference.Row;
            int cellIndex = cellReference.Col;

            OrderedDictionary subjectColorMap =  ClassScheduleTemplate.GetSubjectColorMap();
            foreach(DictionaryEntry entry in subjectColorMap)
            {
                IRow row = sheet.GetRow(rowIndex);
                ICell cell = row.GetCell(cellIndex);
                cell.CellStyle = ClassScheduleTemplate.GetCellStyle(Workbook, (short) entry.Value);
                cell.SetCellValue((string) entry.Key);
                rowIndex++;
            }
        }

        private Dictionary<string, List<Course>> getRoomNameToCoursesMap(IEnumerable<Course> courses) 
        {

            Dictionary<string, List<Course>> roomCourseMap = new Dictionary<string, List<Course>>();

            foreach (Course course in courses)
            {
                if (roomCourseMap.ContainsKey(course.RoomAssignment))
                {
                    roomCourseMap[course.RoomAssignment].Add(course);
                }
                else
                {
                    roomCourseMap.Add(course.RoomAssignment, new List<Course>() { course });
                }
            }

            return roomCourseMap;
        }

        private void PrintCourses(ISheet sheet, List<Course> courses)
        {
            foreach (Course course in courses)
            {
                foreach (DayOfWeek meetingDay in course.MeetingDays)
                {
                    int column = DayMap[meetingDay];
                    int startRow = GetRowForTime(course.StartTime.Value);
                    int endRow = GetRowForTime(course.EndTime.Value);

                    
                    //Get cell
                    var row = sheet.GetRow(startRow);
                    var cell = row.GetCell(column);

                    // Style cell
                    cell.CellStyle = ClassScheduleTemplate.GetCellStyle(Workbook, course.Color());

                    var cellValue = getCourseLabel(course);
                    cell.SetCellValue(cellValue);

                    CellRangeAddress cellRange = new CellRangeAddress(startRow, endRow, column, column);
                    var regionIndex = sheet.AddMergedRegion(cellRange);
                    sheet.AutoSizeColumn(column, true);
                }
            }

        }

        private string getCourseLabel(Course course)
        {
            return course.Course_Title + Environment.NewLine + course.Instructor + Environment.NewLine + course.MeetingPattern;
        }

        private int GetRowForTime(TimeSpan time)
        {
            int minutes = time.Minutes;
            if (minutes % 30 == 0)
            {
                return TimeMap[time];
            }
            else
            {
                minutes = (minutes / 30) * 30;
                return TimeMap[new TimeSpan(time.Hours, minutes, 0)];
            }
        }

        
    }

    
}

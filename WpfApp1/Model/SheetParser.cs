using ClassroomAssignment.Model.Repo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassroomAssignment.Model
{
    public sealed class SheetParser
    {
        const int LAST_ROW_OF_HEADER = 3;
        static bool fileHasMoreRecords = true;
        private static IRoomRepository roomRepo;
     

        public static List<Course> Parse(string[] filePaths, IRoomRepository roomRepo)
        {
            SheetParser.roomRepo = roomRepo;

            var courses = new List<Course>();
            
            foreach (string file in filePaths)
            {
                fileHasMoreRecords = true;
                var coursesFromFile = parseFile(file);
                courses.AddRange(coursesFromFile);
            }

            return courses;
        }

        static List<Course> parseFile(string file)
        {
            var coursesForFile = new List<Course>();

            using (StreamReader fileStream = File.OpenText(file))
            {
                var csvReader = new CsvHelper.CsvReader(fileStream);
                
                // configure csv reader
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Configuration.RegisterClassMap<CourseClassMap>();

                skipHeaders(csvReader);
                csvReader.Read(); // read first header
                while(fileHasMoreRecords)
                {
                    coursesForFile.AddRange(parseRecordsForCourse(csvReader));
                }
            }

            return coursesForFile;
        }

        private static List<Course> parseRecordsForCourse(CsvHelper.CsvReader reader)
        {

            // make sure not at header or end of file
            List<Course> courseList = new List<Course>();

            while((fileHasMoreRecords = reader.Read()) && courseHasMoreRecords(reader))
            {
                Course course = reader.GetRecord<Course>();
                course.SetDerivedProperties();
                courseList.Add(course);
            }

            return courseList;
        }


        static bool courseHasMoreRecords(CsvHelper.CsvReader reader)
        {
            string courseHeader = reader.GetField(0);
            string firstFieldOfRecord = reader.GetField(1);
            bool hasRecordsLeft = string.IsNullOrEmpty(courseHeader) && !string.IsNullOrEmpty(firstFieldOfRecord);
            return hasRecordsLeft;
        }

        static void skipHeaders(CsvHelper.CsvReader reader)
        {
            for (int i = 0; i < LAST_ROW_OF_HEADER; i++)
            {
                reader.Read();
            }

        }
    }
}

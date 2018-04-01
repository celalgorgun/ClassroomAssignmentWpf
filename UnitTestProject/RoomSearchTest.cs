using System.Collections.Generic;
using ClassroomAssignment.Model;
using ClassroomAssignment.Repo;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class RoomSearchTest
    {
        //hardcoded repos
        private HardCodedCourseRepo myhardCodedCourseRepo = new HardCodedCourseRepo();
        private static HardCodedRoomRepo hardCodedRoomRepo = new HardCodedRoomRepo();

        public string term = "Spring 2018";

        //list with all the courses
        public List<Course> courseListing;

        //list with all the hardcoded rooms
        private List<Room> roomListing = hardCodedRoomRepo.Rooms;

        public List<Room> roomSearchResults;


        [TestMethod]
        public void TestMethod1()
        {
            Debug.WriteLine("\nPrinting List\n");
            if (roomListing.Count > 0)
            {

                foreach (var x in roomListing)
                {
                    Debug.WriteLine("{0} : {1}", x.roomName, x.maxCapcity);
                }
            }
        }
    }
}

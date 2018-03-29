using ClassroomAssignment.Model.Repo;
using ClassroomAssignment.Model.Utils;
using ClassroomAssignment.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ClassroomAssignment.Model.DataConstants;

namespace ClassroomAssignment.Model
{
    public class Course : INotifyPropertyChanged
    {
        // Original Attributes of Supplied Spreadsheets
        public string ClassID { get; set; }
        public string SIS_ID { get; set; }
        public String Term { get; set; } 
        public string TermCode { get; set; }
        public string DepartmentCode { get; set; }
        public string SubjectCode { get; set; }
        public string CatalogNumber { get; set; }
        public string CourseName { get; set; }     // Course
        public string Section_Number { get; set; }
        public string Course_Title { get; set; }
        public string Section_Type { get; set; }
        public string Topic { get; set; }  // "Title/Topic"

        private string _meetingPattern;
        public string MeetingPattern
        {
            get { return _meetingPattern; }

            set
            {
                Regex regex = new Regex(DataConstants.MeetingPatternOptions.TIME_PATTERN);
                Match match = regex.Match(value);

                if (match.Success)
                {
                    var daysCapture = match.Groups[1].Captures;
                    MeetingDays = new List<DayOfWeek>();
                    foreach(Capture c in daysCapture)
                    {
                        DayOfWeek day = DateUtil.AbbreviationToDayOfWeek(c.Value);
                        MeetingDays.Add(day);
                    }

                    var startTimeStr = match.Groups[2].Value;
                    StartTime = TimeUtil.StringToTimeSpan(startTimeStr);
                    var endTimeStr = match.Groups[3].Value;
                    EndTime = TimeUtil.StringToTimeSpan(endTimeStr);
                }

                _meetingPattern = value;
            }
        
        }

        public string Instructor { get; set; }

        public string Room { get; set; }

        public String Status { get; set; }
        public String Session { get; set; }
        public String Campus { get; set; }
        public String InstructionMethod { get; set; }
        public String IntegerPartner { get; set; }
        public String Schedule { get; set; }
        public String Consent { get; set; }
        public string CreditHrsMin { get; set; }
        public string CreditHrs { get; set; }
        public String GradeMode { get; set; }
        public String Attributes { get; set; }
        public String RoomAttributes { get; set; }
        public string Enrollment { get; set; }
        public string MaximumEnrollment { get; set; }
        public string PriorEnrollment { get; set; }
        public string ProjectedEnrollment { get; set; }
        public string WaitCap { get; set; }
        public string RoomCapRequest { get; set; }
        public String CrossListings { get; set; }
        public String LinkTo { get; set; }
        public String Comments { get; set; }
        public String Notes { get; set; }

        // Derived information
        public bool AmbiguousState {
            get
            {
                if (!NeedsRoom || AlreadyAssignedRoom) return false;

                return HasMultipleRoomAssignments();
            }

            private set { }
        }

        private bool HasMultipleRoomAssignments()
        {
            bool multipleAssignments = false;

            Regex longPKI = new Regex(RoomOptions.PETER_KIEWIT_INSTITUTE_REGEX);
            Regex shortPKI = new Regex(RoomOptions.PKI_REGEX);

            Match roomColMatch = longPKI.Match(Room);
            Match commentColMatch = shortPKI.Match(Comments);
            Match notesColMatch = shortPKI.Match(Notes);

            if (roomColMatch.Success || commentColMatch.Success || notesColMatch.Success)
            {
                if (roomColMatch.Success ^ commentColMatch.Success ^ notesColMatch.Success)
                {
                    multipleAssignments = false; ;
                }
                else
                {
                    multipleAssignments = true;
                }
            }

            return multipleAssignments;

        }



        private bool _needsRoom;
        public bool NeedsRoom
        {
            get => _needsRoom;

            set
            {
                _needsRoom = value;
                OnPropertyChanged();
            }
        }



        public bool AlreadyAssignedRoom { get; set; }

        private string _roomAssignment;
        public string RoomAssignment
        {
            get => _roomAssignment;

            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    _roomAssignment = null;
                    AlreadyAssignedRoom = false;
                }
                else
                {
                    _roomAssignment = value;
                    AlreadyAssignedRoom = true;
                }

                OnPropertyChanged();
            }
        }
        public List<DayOfWeek> MeetingDays { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AmbiguousState)));
        }

        public void SetDerivedProperties()
        {
            SetNeedsRoom();
            SetRoomAssignment();
        }

        private void SetNeedsRoom()
        {
            if (InstructionMethod?.Equals(InstructionMethods.OFF_CAMPUS) ?? false)
            {
                NeedsRoom = false;
            }
            else if (Room.Equals(RoomOptions.NO_ROOM_NEEDED))
            {
                NeedsRoom = false;
            }
            else
            {
                NeedsRoom = true;
            }
            
        }

        private void SetRoomAssignment()
        {
            if (AmbiguousState) return;

            Regex longPKI = new Regex(RoomOptions.PETER_KIEWIT_INSTITUTE_REGEX);
            Regex shortPKI = new Regex(RoomOptions.PKI_REGEX);

            Match roomColMatch = longPKI.Match(Room);
            Match commentColMatch = shortPKI.Match(Comments);
            Match notesColMatch = shortPKI.Match(Notes);

            
            if (roomColMatch.Success)
            {
                //IRoomRepository roomRepository = InMemoryRoomRepository.getInstance();
                //var room = roomRepository.getNormalizedRoomName(Room);
                var room = Room.Replace("Peter Kiewit Institute", "PKI");
                RoomAssignment = room;
            }
            else if (commentColMatch.Success)
            {
                RoomAssignment = Comments;
            }
            else if (notesColMatch.Success)
            {
                RoomAssignment = Notes;
            }

        }
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Model.Repo
{
    public interface IRoomRepository
    {
        string getNormalizedRoomName(string roomName);
    }
}

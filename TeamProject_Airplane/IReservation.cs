using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject_Airplane
{
    interface IReservation
    {
        void addReservation();
        void checkReservation(string reservationNo);
        void editReservation(string reservationNo);
        void cancelReservation(string reservationNo);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Application.Constants
{
    public class AppointmentStatuses
    {
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";

        public static readonly List<string> All = new()
        {
            Pending,
            Confirmed,
            Completed,
            Cancelled
        };
    }
}

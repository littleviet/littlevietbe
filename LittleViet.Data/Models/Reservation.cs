using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittleViet.Data.Models
{
    internal class Reservation
    {
        public string Id { get; set; }
        public int NoOfPeople { get; set; }
        public DateTime BookingDate { get; set; }
        public int AccountType { get; set; }
        public int Status { get; set; }
        public string PhoneNumber1 { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string FurtherRequest { get; set; }
        public bool IsDeleted { get; set; }
    }
}

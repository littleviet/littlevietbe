﻿using LittleViet.Data.ServiceHelper;

namespace LittleViet.Data.Models;

    internal class Reservation : AuditableEntity
    {
        public int NoOfPeople { get; set; }
        public DateTime BookingDate { get; set; }
        public Guid AccountId { get; set; }
        public ReservationStatus Status { get; set; }
        public string PhoneNumber { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string FurtherRequest { get; set; }
    }


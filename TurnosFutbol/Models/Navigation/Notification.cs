using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace TurnosFutbol.Models.Navigation
{
    public class Notification
    {
        public DateTime Time { get; set; }
        public Color IconColor { get; set; }
        public string TimeDif { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Topic { get; set; }
        public string User { get; set; }
        public string Token { get; set; }
        public string Phone { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Place { get; set; }
    }
    
}


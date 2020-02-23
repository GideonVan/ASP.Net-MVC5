using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventFrameHandler.Models
{
    public class DowntimeModelView
    {
        public string UniqueID { get; set; }
        public string Name { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-mm-dd hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Duration { get; set; }
        public string Equipment { get; set; }
        public string Equipment_Number { get; set; }
        public string Site { get; set; }
        public string Site_Area { get; set; }
        public string Site_Section { get; set; }
        public string Equipment_Type { get; set; }
        public string ElementID { get; set; }
        public Nullable<bool> IsEscalated { get; set; }
    }
}
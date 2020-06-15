using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventFrameHandler.Models
{
    public class EventModelView
    {
        public string uniqueID { get; set; }
        public string name { get; set; }
        //[DisplayFormat(DataFormatString = "{0:yyyy-mm-dd hh:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public string duration { get; set; }
        public string equipmentName { get; set; }
        public string equipmentNumber { get; set; }
        public string site { get; set; }
        public string siteArea { get; set; }
        public string siteSection { get; set; }
        public string equipmentType { get; set; }
        public string ElementID { get; set; }
        public Nullable<bool> IsEscalated { get; set; }
    }
}
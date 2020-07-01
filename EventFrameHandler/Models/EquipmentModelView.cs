using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventFrameHandler.Models
{
    public class EquipmentModelView
    {
        public string elementID { get; set; }

        public string equipName { get; set; }

        public string equipNumber { get; set; }

        public string equipType { get; set; }

        public string site { get; set; }

        public string siteArea { get; set; }

        public string siteSection { get; set; }

        public SelectList equipTypeList { get; set; }

        public SelectList equipNumberList { get; set; }

        public SelectList equipNameList{ get; set; }

    }
}
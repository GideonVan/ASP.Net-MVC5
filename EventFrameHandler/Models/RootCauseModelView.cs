using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventFrameHandler.Models
{
    public class RootCauseModelView
    {
        public string contCategory { get; set; }

        public string contType { get; set; }

        public string contRootCause { get; set; }

        public string contCkey { get; set; }

        public SelectList contCategoryList { get; set; }

        public SelectList contTypeList { get; set; }

        public SelectList contRootCauseList { get; set; }
    }
}
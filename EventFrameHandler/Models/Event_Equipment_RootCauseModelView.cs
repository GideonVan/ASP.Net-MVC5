using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventFrameHandler.Models
{
    public class Event_Equipment_RootCauseModelView
    {
        public RootCauseModelView rcModelView { get; set; }

        public EquipmentModelView equipModelView { get; set; }

        public EventModelView eventModelView { get; set; }
    }
}
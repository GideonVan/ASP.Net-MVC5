using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using EventFrameHandler.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EventFrameHandler.Controllers
{
    public class CustomJsonResult : JsonResult
    {
        private const string _dateFormat = "yyyy-MM-dd HH:mm:ss";

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!String.IsNullOrEmpty(ContentType))
            {
                response.ContentType = ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                // Using Json.NET serializer
                var isoConvert = new IsoDateTimeConverter();
                isoConvert.DateTimeFormat = _dateFormat;
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }

    /// <summary>
    /// Class that encapsulates most common parameters sent by DataTables plugin
    /// </summary>
    public class jQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }
    }

    public class EventController : Controller
    {
        // GET: Event
        public ActionResult Events()
        {
            //MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities();

            //var siteList = db.tbl_factFailureDowntimeCMMS.Select(x => new EventModelView { Site = x.Site }).Distinct().ToList();

            //ViewBag.SiteList = new SelectList(siteList, "", "Site");

            return View();
        }

        [HttpGet]
        public ActionResult GetEvents() //All the events which have not been escalated
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                List<EventModelView> dtList = db.tbl_factFailureDowntimeCMMS.Where(x => x.IsEscalated == false).OrderByDescending(x => x.StartTime).Select(x => new EventModelView { equipmentName = x.EquipmentName, startTime = x.StartTime, endTime = x.EndTime, duration = x.Duration, equipmentNumber = x.EquipmentNumber, site = x.Site, uniqueID = x.UniqueID }).ToList();

                return Json(new { data = dtList }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SetFirstResponderPartial(string EventID, string Site)
        {

            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                //ViewBag.equipTypeList = new SelectList(GetEquipmentTypeList(Site), "elementID", "equipType");
                //ViewBag.contCategoryList = new SelectList(GetContributingDowntimeCategoryList(), "", "contCategory");

                var dtEvent = db.tbl_factFailureDowntimeCMMS.SingleOrDefault(x => x.UniqueID == EventID);

                Event_Equipment_RootCauseModelView parent = new Event_Equipment_RootCauseModelView
                {
                    eventModelView = new EventModelView(),
                    equipModelView = new EquipmentModelView(),
                    rootCauseModelView = new RootCauseModelView()
                };

                parent.equipModelView.equipTypeList = new SelectList(GetEquipmentTypeList(Site), "", "equipType");
                parent.rootCauseModelView.contCategoryList = new SelectList(GetContributingDowntimeCategoryList(), "", "contCategory");

                parent.equipModelView.equipNumberList = new SelectList("");
                parent.equipModelView.equipNameList = new SelectList("");
                parent.rootCauseModelView.contTypeList = new SelectList("");
                parent.rootCauseModelView.contRootCauseList = new SelectList("");

                parent.eventModelView.uniqueID = dtEvent.UniqueID;
                parent.eventModelView.equipmentName = dtEvent.EquipmentName;
                parent.eventModelView.equipmentNumber = dtEvent.EquipmentNumber;
                parent.eventModelView.startTime = dtEvent.StartTime;
                parent.eventModelView.endTime = dtEvent.EndTime;
                parent.eventModelView.duration = dtEvent.Duration;
                parent.eventModelView.site = dtEvent.Site;

                return PartialView("EventAndRootCausePartialGrid", parent);
            }
        }

        public ActionResult SetWorkOrderPartial(string EventID, string Site)
        {

            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                Event_Equipment_RootCauseModelView parent = new Event_Equipment_RootCauseModelView();

                ViewBag.equipTypeList = new SelectList(GetEquipmentTypeList(Site), "equipNumber", "equipType");

                parent.eventModelView = new EventModelView();

                tbl_factFailureDowntimeCMMS dtEvent = db.tbl_factFailureDowntimeCMMS.SingleOrDefault(x => x.UniqueID == EventID);

                parent.eventModelView.uniqueID = dtEvent.UniqueID;
                parent.eventModelView.equipmentName = dtEvent.EquipmentName;
                parent.eventModelView.equipmentNumber = dtEvent.EquipmentNumber;
                parent.eventModelView.startTime = dtEvent.StartTime;
                parent.eventModelView.endTime = dtEvent.EndTime;
                parent.eventModelView.duration = dtEvent.Duration;
                parent.eventModelView.site = dtEvent.Site;

                return PartialView("AnnotationWorkOrderPartial", parent);
            }
        }

        public List<EquipmentModelView> GetEquipmentTypeList(string Site)
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                List<EquipmentModelView> equipType = db.tbl_DimSiteEquipment.Where(x => x.Site == Site).Select(x => new EquipmentModelView { equipType = x.EquipmentType }).Distinct().ToList();

                return equipType;
            }
        }

        public JsonResult GetEquipmentNumberList(string EquipmentType)
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                List<EquipmentModelView> equipmentNumberList = db.tbl_DimSiteEquipment.Where(x => x.EquipmentType == EquipmentType).Select(x => new EquipmentModelView { equipNumber = x.EquipmentNumber }).Distinct().ToList();

                return Json(equipmentNumberList, JsonRequestBehavior.AllowGet);

            }
        }

        //public ActionResult GetEquipmentNameList(string EquipmentNumber)
        //{
        //    using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
        //    {
        //        List<EquipmentModelView> equipmentNameList = db.tbl_DimSiteEquipment.Where(x => x.EquipmentNumber== EquipmentNumber).Select(x => new EquipmentModelView { equipName = x.EquipmentName }).Distinct().ToList();

        //        //ViewBag.equipmentNameList = new SelectList(equipmentNameList, "equipNumber", "equipName");

        //        Event_Equipment_RootCauseModelView parent = new Event_Equipment_RootCauseModelView();

        //        parent.equipModelView = new EquipmentModelView();

        //        parent.equipModelView.equipNameList = new SelectList(equipmentNameList, "equipNumber", "equipName");

        //        return PartialView("EquipmentNamePartial");
        //    }
        //}

        public JsonResult GetEquipmentNameList(string EquipmentNumber)
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                List<EquipmentModelView> equipmentNameList = db.tbl_DimSiteEquipment.Where(x => x.EquipmentNumber == EquipmentNumber).Select(x => new EquipmentModelView { equipName = x.EquipmentName }).Distinct().ToList();

                return Json(equipmentNameList, JsonRequestBehavior.AllowGet);
            }
        }

        public List<RootCauseModelView> GetContributingDowntimeCategoryList()
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                List<RootCauseModelView> contDowntimeCategoryList = db.tbl_DimNonWorkOrderDowntimeAnnnotations.Select(x => new RootCauseModelView { contCategory = x.Category }).Distinct().ToList();

                return (contDowntimeCategoryList);
            }
        }

        public JsonResult GetContributingDowntimeTypeList(string ContDowntimeCategory)
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                List<RootCauseModelView> contDowntimeTypeList = db.tbl_DimNonWorkOrderDowntimeAnnnotations.Where(x => x.Category == ContDowntimeCategory).Select(x => new RootCauseModelView { contType = x.DowntimeType }).Distinct().ToList();

                //ViewBag.contDowntimeTypeList = new SelectList(contDowntimeTypeList, "contCkey", "contType");

                //Event_Equipment_RootCauseModelView parent = new Event_Equipment_RootCauseModelView();

                //parent.rootCauseModelView = new RootCauseModelView();

                //parent.rootCauseModelView.contTypeList = new SelectList(contDowntimeTypeList, "", "contType");

                return Json(contDowntimeTypeList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetContributingDowntimeRootCauseList(string ContDowntimeCategory, string ContDowntimeType)
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {

                List<RootCauseModelView> contDowntimeRootCauseList = db.tbl_DimNonWorkOrderDowntimeAnnnotations.Where(x => x.Category == ContDowntimeCategory && x.DowntimeType == ContDowntimeType).Select(x => new RootCauseModelView { contRootCause = x.RootCause }).Distinct().ToList();

                //ViewBag.contDowntimeRootCauseList = new SelectList(contDowntimeRootCauseList, "contCkey", "contRootCause");

                //Event_Equipment_RootCauseModelView parent = new Event_Equipment_RootCauseModelView();

                //parent.rootCauseModelView = new RootCauseModelView();

                //parent.rootCauseModelView.contRootCauseList = new SelectList(contDowntimeRootCauseList, "", "contRootCause");

                return Json(contDowntimeRootCauseList, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult InsertFirstResponderAnnotation(Event_Equipment_RootCauseModelView model, FormCollection collection)
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                try
                {
                    var result = UpdateMasterDowntimeTable(model.eventModelView.uniqueID);

                    var annotation = new tbl_factMaintenanceAnnotations();
                    annotation.UniqueID = model.eventModelView.uniqueID;
                    annotation.FailureTypeDescription = model.rootCauseModelView.contType;
                    annotation.FailureDescription = model.rootCauseModelView.contRootCause;
                    annotation.EquipmentType = model.equipModelView.equipType;
                    annotation.EquipmentName = model.equipModelView.equipName;
                    annotation.EquipmentNumber = model.equipModelView.equipNumber;

                    annotation.ModifyStamp = DateTime.Now;

                    db.tbl_factMaintenanceAnnotations.Add(annotation);

                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return RedirectToAction("Events");
            }
        }

        public int UpdateMasterDowntimeTable(string uniqueId)
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                try
                {
                    var eventToUpdate = db.tbl_factFailureDowntimeCMMS.Find(uniqueId);

                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return 0;
            }
        }
    }
}
//public ActionResult AjaxHandler(jQueryDataTableParamModel param)
//{

//    using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
//    {
//        List<DowntimeModelView> dtList = db.tbl_factFailureDowntimeCMMS.Where(x => x.IsEscalated == false).Select(x => new DowntimeModelView { UniqueID = x.UniqueID, Equipment = x.Equipment, StartTime = x.StartTime, EndTime = x.EndTime, Duration = x.Duration, Equipment_Number = x.Equipment_Number, Site = x.Site }).ToList();

//        //var result = from i in dtList select new[] { i.UniqueID, i.Equipment, i.Equipment_Number, i.StartTime.ToString(), i.EndTime.ToString(), i.Duration, i.Site };

//        return Json(new
//        {
//            sEcho = param.sEcho,
//            iTotalRecords = dtList.Count(),
//            iTotalDisplayRecords = dtList.Count(),
//            data = dtList
//        }, JsonRequestBehavior.AllowGet);
//    }
//}

//public ActionResult GetEventsFiltered(DateTime startTime)
//{
//    using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
//    {
//        List<DowntimeModelView> dtList = db.tbl_factFailureDowntimeCMMS.Where(x => x.IsEscalated == false && x.StartTime > startTime).Select(x => new DowntimeModelView { Equipment = x.Equipment, StartTime = x.StartTime, EndTime = x.EndTime, Duration = x.Duration, Equipment_Number = x.Equipment_Number, Site = x.Site, UniqueID = x.UniqueID }).ToList();

//        return Json( new { data = dtList }, JsonRequestBehavior.AllowGet );
//    }
//}
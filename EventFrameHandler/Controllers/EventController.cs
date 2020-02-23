using System;
using System.Collections.Generic;
using System.Linq;
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
            MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities();

            var siteList = db.tbl_factFailureDowntimeCMMS.Select(x => new DowntimeModelView { Site = x.Site }).Distinct().ToList();

            ViewBag.SiteList = new SelectList(siteList, "", "Site");

            return View();
        }

        [HttpGet]
        public ActionResult GetEvents()
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                List<DowntimeModelView> dtList = db.tbl_factFailureDowntimeCMMS.Where(x => x.IsEscalated == false).OrderByDescending(x=>x.StartTime).Select(x => new DowntimeModelView { Equipment = x.Equipment, StartTime = x.StartTime, EndTime = x.EndTime, Duration = x.Duration, Equipment_Number = x.Equipment_Number, Site = x.Site, UniqueID = x.UniqueID }).ToList();

                return Json(new { data = dtList }, JsonRequestBehavior.AllowGet);
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

        public ActionResult GetEventsFiltered(DateTime startTime)
        {
            using (MOMSEventFrameInterfaceEntities db = new MOMSEventFrameInterfaceEntities())
            {
                List<DowntimeModelView> dtList = db.tbl_factFailureDowntimeCMMS.Where(x => x.IsEscalated == false && x.StartTime > startTime).Select(x => new DowntimeModelView { Equipment = x.Equipment, StartTime = x.StartTime, EndTime = x.EndTime, Duration = x.Duration, Equipment_Number = x.Equipment_Number, Site = x.Site, UniqueID = x.UniqueID }).ToList();

                return Json( new { data = dtList }, JsonRequestBehavior.AllowGet );
            }
        }
    }
}
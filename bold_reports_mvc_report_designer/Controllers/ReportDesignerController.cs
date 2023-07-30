using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BoldReports.Web;
using BoldReports.Web.ReportViewer;
using BoldReports.Web.ReportDesigner;
using System.IO;
using System.Reflection;
using System.Web;

namespace bold_reports_mvc_report_designer.Controllers
{
    public class ReportDesignerController : ApiController, IReportDesignerController
    {
        /// <summary>
        /// Get the path of specific file
        /// </summary>
        /// <param name="itemName">Name of the file to get the full path</param>
        /// <param name="key">The unique key for report designer</param>
        /// <returns>Returns the full path of file</returns>
        [NonAction]
        private string GetFilePath(string itemName, string key)
        {
            string dirPath = Path.Combine(HttpContext.Current.Server.MapPath("~/") + "Cache", key);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            return Path.Combine(dirPath, itemName);
        }

        /// <summary>
        /// Action (HttpGet) method for getting resource of images in the report.
        /// </summary>
        /// <param name="key">The unique key for request identification.</param>
        /// <param name="image">The name of requested image.</param>
        /// <returns>Returns the image as HttpResponseMessage content.</returns>
        [System.Web.Http.ActionName("GetImage")]
        [AcceptVerbs("GET")]
        public object GetImage(string key, string image)
        {
            return ReportDesignerHelper.GetImage(key, image, this);
        }

        /// <summary>
        /// Action (HttpPost) method for posting the request for designer actions.
        /// </summary>
        /// <param name="jsonData">A collection of keys and values to process the designer request.</param>
        /// <returns>Json result for the current request.</returns>
        public object PostDesignerAction(Dictionary<string, object> jsonData)
        {
            //Processes the designer request and returns the result.
            return ReportDesignerHelper.ProcessDesigner(jsonData, this, null);
        }

        /// <summary>
        /// Sets the resource into storage location.
        /// </summary>
        /// <param name="key">The unique key for request identification.</param>
        /// <param name="itemId">The unique key to get the required resource.</param>
        /// <param name="itemData">Contains the resource data.</param>
        /// <param name="errorMessage">Returns the error message, if the write action is failed.</param>
        /// <returns>Returns true, if resource is successfully written into storage location.</returns>
        [NonAction]
        public bool SetData(string key, string itemId, ItemInfo itemData, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (itemData.Data != null)
            {
                File.WriteAllBytes(this.GetFilePath(itemId, key), itemData.Data);
            }
            else if (itemData.PostedFile != null)
            {
                var fileName = itemId;
                if (string.IsNullOrEmpty(itemId))
                {
                    fileName = Path.GetFileName(itemData.PostedFile.FileName);
                }
                itemData.PostedFile.SaveAs(this.GetFilePath(fileName, key));
            }
            return true;
        }

        /// <summary>
        /// Gets the resource from storage location.
        /// </summary>
        /// <param name="key">The unique key for request identification.</param>
        /// <param name="itemId">The unique key to get the required resource.</param>
        ///  <returns>Returns the resource data and error message.</returns>
        [NonAction]
        public ResourceInfo GetData(string key, string itemId)
        {
            var resource = new ResourceInfo();
            try
            {
                var filePath = this.GetFilePath(itemId, key);
                if (itemId.Equals(Path.GetFileName(filePath), StringComparison.InvariantCultureIgnoreCase) && File.Exists(filePath))
                {
                    resource.Data = File.ReadAllBytes(filePath);
                }
                else
                {
                    resource.ErrorMessage = "File not found from the specified path";
                }
            }
            catch (Exception ex)
            {
                resource.ErrorMessage = ex.Message;
            }

            return resource;
        }

        /// <summary>
        /// Action (HttpPost) method for posted or uploaded file actions.
        /// </summary>
        public void UploadReportAction()
        {
            //Processes the designer file upload request's.
            ReportDesignerHelper.ProcessDesigner(null, this, System.Web.HttpContext.Current.Request.Files[0]);
        }

        /// <summary>
        /// Send a GET request and returns the requested resource for a report.
        /// </summary>
        /// <param name="key">The unique key to get the desired resource.</param>
        /// <param name="resourcetype">The type of the requested resource.</param>
        /// <param name="isPrint">If set to <see langword="true"/>, then the resource is generated for printing.</param>
        /// <returns> Resource object for the given key</returns>
        [System.Web.Http.ActionName("GetResource")]
        [AcceptVerbs("GET")]
        public object GetResource(string key, string resourcetype, bool isPrint)
        {
            //Returns the report resource for the requested key.
            return ReportHelper.GetResource(key, resourcetype, isPrint);
        }

        /// <summary>
        /// Report Initialization method that is triggered when report begin processed.
        /// </summary>
        /// <param name="reportOptions">The ReportViewer options.</param>
        [NonAction]
        public void OnInitReportOptions(ReportViewerOptions reportOption)
        {
            //You can update report options here
        }

        /// <summary>
        /// Report loaded method that is triggered when report and sub report begins to be loaded.
        /// </summary>
        /// <param name="reportOptions">The ReportViewer options.</param>
        [NonAction]
        public void OnReportLoaded(ReportViewerOptions reportOption)
        {
            //You can update report options here
        }

        /// <summary>
        /// Action (HttpPost) method for posting the request for report process.
        /// </summary>
        /// <param name="jsonData">The JSON data posted for processing report.</param>
        /// <returns>The object data.</returns>
        public object PostReportAction(Dictionary<string, object> jsonData)
        {
            //Processes the report request and returns the result.
            return ReportHelper.ProcessReport(jsonData, this as IReportController);
        }
    }
}

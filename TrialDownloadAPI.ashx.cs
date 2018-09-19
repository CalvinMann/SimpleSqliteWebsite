using SqlDatabaseManager.SimpleSqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SimpleSqliteWebsite
{
    /// <summary>
    /// Summary description for TrialDownload
    /// </summary>
    public class TrialDownloadAPI : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //Save a trial user to the database
                string version = ConfigurationManager.AppSettings["Version"];

                if (string.IsNullOrEmpty(version))
                {
                    //throw error and return
                    return;
                }

                string trialDownloadPath = "~\\Releases\\" + version + "\\API\\Downloadables\\Trial\\SimpleSqliteAPI.zip";

                //Transmit the files
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.ClearContent();
                response.Clear();
                response.ContentType = "text/plain";
                response.AddHeader("Content-Disposition", "attachment; filename=SimpleSqliteAPI.zip;");
                response.TransmitFile(trialDownloadPath);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                //Pop up dialog to let user know an error occurred
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
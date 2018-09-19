using SqlDatabaseManager.SimpleSqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SimpleSqliteWebsite
{
    /// <summary>
    /// Summary description for TrialDownloadUtility
    /// </summary>
    public class TrialDownloadUtility : IHttpHandler
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

                string trialDownloadPath = "~\\Releases\\" + version + "\\Utility\\Downloadables\\Trial\\SimpleSqliteUtility.zip";

                //Transmit the files
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.ClearContent();
                response.Clear();
                response.ContentType = "text/plain";
                response.AddHeader("Content-Disposition", "attachment; filename=SimpleSqliteUtility.zip;");
                response.TransmitFile(trialDownloadPath);
                response.Flush();
                response.End();
            }
            catch (Exception ex)
            {
                //Report error in an email 
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
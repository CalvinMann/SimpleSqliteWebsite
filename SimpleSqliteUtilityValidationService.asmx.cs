using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace SimpleSqliteWebsite
{
    /// <summary>
    /// Summary description for SimpleSqliteUtilityValidationService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SimpleSqliteUtilityValidationService : System.Web.Services.WebService
    {

        [WebMethod]
        public byte[] Activate(byte[] license)
        {
            string version = ConfigurationManager.AppSettings["Version"];
            string intelliLockDatabasePassword = ConfigurationManager.AppSettings["IntelliLockDatabasePassword"];

            string releasesDirectory = AppDomain.CurrentDomain.GetData("Releases").ToString();
            string intelliLockDatabaseDirectory = releasesDirectory + "Databases\\SimpleSqliteUtilityLicenses.ildb";
            string intelliLockProjectDirectory = releasesDirectory + version + "\\Utility\\IntelliLockProject\\IntelliLockProject.ilproj";


            List<string> licenseValidationChecks = new List<string>();
            licenseValidationChecks.Add("Customer");
            licenseValidationChecks.Add("FirstName");
            licenseValidationChecks.Add("LastName");

             IntelliLockManager.LicenseManager licenseManager = new IntelliLockManager.LicenseManager() ;
             licenseManager.ValidateUtilityLicense(license, intelliLockProjectDirectory, intelliLockDatabaseDirectory, intelliLockDatabasePassword, licenseValidationChecks);

             return null;
        }
    }
}

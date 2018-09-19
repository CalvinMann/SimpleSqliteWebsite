using SqlDatabaseManager.SimpleSqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimpleSqliteWebsite
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnDownloadTrial_Click(object sender, EventArgs e)
        {
            //Get the user data from the form
            TrialUser trialUser = new SqlDatabaseManager.SimpleSqlite.TrialUser();
            trialUser.Email = "testEmail@test.com";
            trialUser.Name = "test name";
            trialUser.Version = ConfigurationManager.AppSettings["Version"];

            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["SimpleSqliteDatabaseConnString"];

            if (connectionStringSettings == null || string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
            {
                //throw error and return
                return;
            }

            Database.TrialDatabase.AddTrialUser(trialUser, connectionStringSettings.ConnectionString);


            //SEE IF THIS CAN BE REFACTORED INTO ONE HANDLER
            Response.Redirect("TrialDownloadAPI.ashx", false);

            //Response.Redirect("TrialDownloadUtility.ashx", false);
        }

        protected void btn_BuyNow_Click(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

    }
}
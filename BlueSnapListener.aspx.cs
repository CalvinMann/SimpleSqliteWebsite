using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlDatabaseManager.SimpleSqlite;
using System.IO;
using System.Configuration;

namespace SimpleSqliteWebsite
{
    public partial class BlueSnapListener : System.Web.UI.Page
    {
        string version;
        string simpleSqliteDatabaseConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

#if DEBUG
            //SendTestEmail();
#endif

#if DEBUG
            WriteResponseValues();
#endif

            string transactiontype = GetResponseParam("transactionType");
            simpleSqliteDatabaseConnectionString = ConfigurationManager.ConnectionStrings["SimpleSqliteDatabaseConnString"].ConnectionString;

            if ((transactiontype == "REFUND") || (transactiontype == "CHARGEBACK") || (transactiontype == "CANCELLATION") || (transactiontype == "CANCELLATION_REFUND") || (transactiontype == "CANCELLATION_CHARGEBACK") || (transactiontype == "CANCEL") || (transactiontype == "DECLINE") || string.IsNullOrEmpty(transactiontype))
                return;

            version = ConfigurationManager.AppSettings["Version"];
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["SimpleSqliteDatabaseConnString"];

            if (connectionStringSettings == null || string.IsNullOrEmpty(connectionStringSettings.ConnectionString))
                return;
            

            Customer customer = null;

            customer = CreateCustomerRecordFromRequest();

            AddCustomerToDatabase(customer);

            CreateLicensesForProducts(customer);

            //SentAPILicenseFile(customer, apiLicense);
        }


        #region Customer Creation

        private Customer AddCustomerToDatabase(Customer customer)
        {
            try
            {
                if (customer == null)
                    return null;

                Guid existingCustomerId = Database.CustomerDatabase.TryGetExistingCustomer(customer, simpleSqliteDatabaseConnectionString);

                if (existingCustomerId != System.Guid.Empty)
                    Database.CustomerDatabase.UpdateCustomer(customer, simpleSqliteDatabaseConnectionString);
                else
                    Database.CustomerDatabase.AddCustomer(customer, simpleSqliteDatabaseConnectionString);

                return customer;
            }
            catch (Exception ex)
            {
                //Email issue to account so i can be notified of an issue if one exists
#if DEBUG
                File.WriteAllText(@"D:\Development\SimpleSqlite\SimpleSqliteWebsite\Error.txt", ex.Message);
#endif
                return null;
            }
        }

        private Customer CreateCustomerRecordFromRequest()
        {
            try
            {
                Customer customer = new Customer();
                customer.CustomerId = Guid.NewGuid();

                Address address = CreateAddressFromRequest();

                List<Order> orders = CreateOrderFromRequest();

                if (address != null)
                    customer.Address = address;
                else
                { /*throw error and email my account from the catch*/ }

                if (orders != null)
                    customer.Orders = orders;
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("email"))
                    customer.Email = Request.Form["email"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("firstName"))
                    customer.FirstName = Request.Form["firstName"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("lastName"))
                    customer.LastName = Request.Form["lastName"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("workPhone"))
                    customer.PhoneNumber = Request.Form["workPhone"].Trim();
                else if (Request.Form.AllKeys.Contains("mobilePhone"))
                    customer.PhoneNumber = Request.Form["mobilePhone"].Trim();
                else if (Request.Form.AllKeys.Contains("homePhone"))
                    customer.PhoneNumber = Request.Form["homePhone"].Trim();
                else
                { /* no phone number was supplied and thats okay if not required*/ }


                if (Request.Form.AllKeys.Contains("language"))
                    customer.Language = Request.Form["language"].Trim();
                else
                { /*throw error and email my account from the catch*/ }

                return customer;
            }
            catch (Exception ex)
            {
                //Email error here
#if DEBUG
                File.WriteAllText(@"D:\Development\SimpleSqlite\SimpleSqliteWebsite\Error.txt", ex.Message);
#endif
                return null;
            }
        }

        private Address CreateAddressFromRequest()
        {
            try
            {
                Address address = new Address();
                address.AddressId = Guid.NewGuid();

                if (Request.Form.AllKeys.Contains("address1"))
                    address.Line1 = Request.Form["address1"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("address2"))
                    address.Line2 = Request.Form["address2"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("city"))
                    address.City = Request.Form["city"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("state"))
                    address.State = Request.Form["state"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("zipCode"))
                    address.ZipCode = Request.Form["zipCode"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("country"))
                    address.Country = Request.Form["country"].Trim();
                else
                { /*throw error and email my account from the catch*/ }

                return address;
            }
            catch (Exception ex)
            {
                //Email error here

#if DEBUG
                File.WriteAllText(@"D:\Development\SimpleSqlite\SimpleSqliteWebsite\Error.txt", ex.Message);
#endif
                return null;
            }
        }

        private List<Order> CreateOrderFromRequest()
        {
            try
            {

                List<Order> orders = new List<Order>();

                Order order = new Order();
                order.OrderId = Guid.NewGuid();

                if (Request.Form.AllKeys.Contains("transactionDate"))
                    order.TransactionDate = Request.Form["transactionDate"].Trim();
                else
                { /*throw error and email my account from the catch*/ }

                if (Request.Form.AllKeys.Contains("Company or Developer Name"))
                    order.DeveloperOrCompanyName = Request.Form["Company or Developer Name"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("invoiceChargeAmount"))
                    order.ChargeAmount = Request.Form["invoiceChargeAmount"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("currency"))
                    order.Currency = Request.Form["currency"].Trim();
                else
                { /*throw error and email my account from the catch*/ }


                if (Request.Form.AllKeys.Contains("referenceNumber"))
                    order.ReferenceNumber = Request.Form["referenceNumber"].Trim();
                else
                { /*throw error and email my account from the catch*/ }

                if (Request.Form.AllKeys.Contains("How did you hear about us?"))
                    order.HowDidYouHearAboutUs = Request.Form["How did you hear about us?"].Trim();


                if (Request.Form.AllKeys.Contains("remoteAddress"))
                    order.OrderIPAddress = Request.Form["remoteAddress"].Trim();


                List<Product> products = CreateProductsFromRequest();

                if (products != null)
                    order.Products = products;
                else
                { /*throw error and email my account from the catch*/ }

                orders.Add(order);
                return orders;
            }
            catch (Exception ex)
            {
                //Email error here
#if DEBUG
                File.WriteAllText(@"D:\Development\SimpleSqlite\SimpleSqliteWebsite\Error.txt", ex.Message);
#endif
                return null;
            }

        }

        private List<Product> CreateProductsFromRequest()
        {
            try
            {
                List<Product> products = new List<Product>();

                CreateMainPurchasedProductsFromRequest(products);

                CreatePromoProductsFromRequest(products);

                return products;
            }
            catch (Exception ex)
            {
                //Email error here
#if DEBUG
                File.WriteAllText(@"D:\Development\SimpleSqlite\SimpleSqliteWebsite\Error.txt", ex.Message);
#endif
                return null;
            }
        }

        private void CreatePromoProductsFromRequest(List<Product> products)
        {
            try
            {
                ///Get the promotion products
                bool exitLoop = false;
                for (int promoProdIndex = 0; exitLoop == false; promoProdIndex++)
                {
                    string promotionContractQuantity = "promoteContractQuantity" + promoProdIndex;
                    if (Request.Form.AllKeys.Contains(promotionContractQuantity))
                    {
                        string promoteContrQntyStr = Request.Form[promotionContractQuantity].Trim();
                        int promoteContrQnty = 1;
                        int.TryParse(promoteContrQntyStr, out promoteContrQnty);

                        for (int promoteContrQntyIndex = 0; promoteContrQntyIndex < promoteContrQnty; promoteContrQntyIndex++)
                        {

                            Product product = new Product();
                            product.ProductId = Guid.NewGuid();

                            string promoteContractChargePrice = "promoteContractChargePrice" + promoProdIndex;
                            if (Request.Form.AllKeys.Contains(promoteContractChargePrice))
                                product.ChargePrice = Request.Form[promoteContractChargePrice].Trim();
                            else
                            { /*throw error and email my account from the catch*/ }


                            string promoteProductId = "promoteProductId" + promoProdIndex;
                            if (Request.Form.AllKeys.Contains(promoteProductId))
                                product.ProductIdInStore = Request.Form[promoteProductId].Trim();
                            else
                            { /*throw error and email my account from the catch*/ }


                            string promoteContractId = "promoteContractId" + promoProdIndex;
                            if (Request.Form.AllKeys.Contains(promoteContractId))
                                product.ContractId = Request.Form[promoteContractId].Trim();
                            else
                            { /*throw error and email my account from the catch*/ }


                            string promoteContractName = "promoteContractName" + promoProdIndex;
                            if (Request.Form.AllKeys.Contains(promoteContractName))
                                product.ProductName = Request.Form[promoteContractName].Trim();
                            else
                            { /*throw error and email my account from the catch*/ }



                            if (product.ProductName.ToLower().Contains("utility") && product.ProductName.ToLower().Contains("api"))
                            {
                                product.ProductType = ProductType.API_Utility;
                            }
                            else if (product.ProductName.ToLower().Contains("utility"))
                            {
                                product.ProductType = ProductType.Utility;
                            }
                            else if (product.ProductName.ToLower().Contains("api"))
                            {
                                product.ProductType = ProductType.API;
                            }
                            else
                                product.ProductType = ProductType.API;

                            products.Add(product);
                        }
                    }
                    else
                        exitLoop = true;
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                File.WriteAllText(@"D:\Development\SimpleSqlite\SimpleSqliteWebsite\Error.txt", ex.Message);
#endif
            }
        }

        private void CreateMainPurchasedProductsFromRequest(List<Product> products)
        {
            try
            {
                int mainProdQuantity = 1;
                if (Request.Form.AllKeys.Contains("quantity"))
                {
                    string strQuantity = Request.Form["quantity"].Trim();

                    if (int.TryParse(strQuantity, out mainProdQuantity) == false)
                    {
                        //report error and return out
                        return;
                    }
                }
                else
                {
                    //report error and return out
                    return;
                }

                for (int mainProdCount = 0; mainProdCount < mainProdQuantity; mainProdCount++)
                {

                    Product product = new Product();
                    product.ProductId = Guid.NewGuid();

                    string contractChargePrice = "contractChargePrice";
                    if (Request.Form.AllKeys.Contains(contractChargePrice))
                        product.ChargePrice = Request.Form[contractChargePrice].Trim();
                    else
                    { /*throw error and email my account from the catch*/ }


                    string productId = "productId";
                    if (Request.Form.AllKeys.Contains(productId))
                        product.ProductIdInStore = Request.Form[productId].Trim();
                    else
                    { /*throw error and email my account from the catch*/ }


                    string contractId = "contractId";
                    if (Request.Form.AllKeys.Contains(contractId))
                        product.ContractId = Request.Form[contractId].Trim();
                    else
                    { /*throw error and email my account from the catch*/ }


                    string contractName = "contractName";
                    if (Request.Form.AllKeys.Contains(contractName))
                        product.ProductName = Request.Form[contractName].Trim();
                    else
                    { /*throw error and email my account from the catch*/ }


                    if (product.ProductName.ToLower().Contains("utility") && product.ProductName.ToLower().Contains("api"))
                    {
                        product.ProductType = ProductType.API_Utility;
                    }
                    else if (product.ProductName.ToLower().Contains("utility"))
                    {
                        product.ProductType = ProductType.Utility;
                    }
                    else if (product.ProductName.ToLower().Contains("api"))
                    {
                        product.ProductType = ProductType.API;
                    }
                    else
                        product.ProductType = ProductType.API;


                    products.Add(product);
                }

            }
            catch (Exception ex)
            {
#if DEBUG
                File.WriteAllText(@"D:\Development\SimpleSqlite\SimpleSqliteWebsite\Error.txt", ex.Message);
#endif
            }
        }

        private void CreateLicensesForProducts(SqlDatabaseManager.SimpleSqlite.Customer customer)
        {
            List<IntelliLockManager.License> apiLicenses = new List<IntelliLockManager.License>();
            List<IntelliLockManager.License> utilityLicenses = new List<IntelliLockManager.License>();



            foreach (Order order in customer.Orders)
            {
                Dictionary<string, string> licenseInformationKeyValues = new Dictionary<string, string>();
                licenseInformationKeyValues.Add("[AssemblyCompany]", order.DeveloperOrCompanyName);
                licenseInformationKeyValues.Add("FirstName", customer.FirstName);
                licenseInformationKeyValues.Add("LastName", customer.LastName);
                licenseInformationKeyValues.Add("Email", customer.Email);

                foreach (Product product in order.Products)
                {
                    switch (product.ProductType)
                    {
                        case ProductType.API_Utility:

                            //Create an api and utility license
                            IntelliLockManager.License apiLicenseForDualPackage = CreateLicenseFile("API", licenseInformationKeyValues);
                            apiLicenses.Add(apiLicenseForDualPackage);

                            IntelliLockManager.License utilityLicenseForDualPackage = CreateLicenseFile("Utility", licenseInformationKeyValues);
                            utilityLicenses.Add(utilityLicenseForDualPackage);
                            break;

                        case ProductType.API:

                            IntelliLockManager.License apiLicenseForSinglePackage = CreateLicenseFile("API", licenseInformationKeyValues);
                            apiLicenses.Add(apiLicenseForSinglePackage);
                            break;

                        case ProductType.Utility:
                            IntelliLockManager.License utilityLicenseForSinglePackage = CreateLicenseFile("Utility", licenseInformationKeyValues);
                            utilityLicenses.Add(utilityLicenseForSinglePackage);
                            break;

                    }
                }

            }


            if (apiLicenses.Count > 0)
            {
                string apiEmailTemplateLocation = MapPath("~\\Releases\\" + version + "\\API\\EmailTemplate\\ApiEmailTemplate.html");
                string licenseFileName = ConfigurationManager.AppSettings["ApiLicenseFileName"] ?? "api.sslic";
                string emailSubject = ConfigurationManager.AppSettings["APIEmailSubject"] ?? "SimpleSqlite API License File(s)";
                string productName = "SimpleSqlite API";

                SendLicenseFileEmail(customer, apiLicenses, apiEmailTemplateLocation, licenseFileName, emailSubject, productName);
            }

            if (utilityLicenses.Count > 0)
            {
                string utilityEmailTemplateLocation = MapPath("~\\Releases\\" + version + "\\Utility\\EmailTemplate\\UtilityEmailTemplate.html");
                string licenseFileName = ConfigurationManager.AppSettings["UtilityLicenseFileName"] ?? "util.sslic";
                string emailSubject = ConfigurationManager.AppSettings["UtilityEmailSubject"] ?? "SimpleSqlite Utility License File(s)";
                string productName = "SimpleSqlite Utility";

                SendLicenseFileEmail(customer, utilityLicenses, utilityEmailTemplateLocation, licenseFileName, emailSubject, productName);
            }
        }

        #endregion

        #region IntelliLockManager


        private IntelliLockManager.License CreateLicenseFile(string projectTypeFolderName, Dictionary<string, string> licenseInformationKeyValues)
        {
            string apiIntelliLockProjLocation = MapPath("~\\Releases\\" + version + "\\" + projectTypeFolderName + "\\IntelliLockProject\\IntelliLockProject.ilproj");
            string companyOrDeveloperName = string.Empty;

            if (Request.Form.AllKeys.Contains("Company or Developer Name"))
                companyOrDeveloperName = Request.Form["Company or Developer Name"].Trim();
            else
            { /*throw error and email my account from the catch*/ }


            IntelliLockManager.LicenseManager licenseManager = new IntelliLockManager.LicenseManager();
            IntelliLockManager.License license = licenseManager.CreateLicense(apiIntelliLockProjLocation, licenseInformationKeyValues);


            if (license.LicenseFile == null)
            { /*throw error and email my account from the catch*/ }

            return license;
        }

        private void SendLicenseFileEmail(Customer customer, List<IntelliLockManager.License> licenses, string emailTemplateLocation, string attachmentFileName, string emailSubject, string productName)
        {

            Dictionary<string, string> templateKeyVals = new Dictionary<string, string>();
            templateKeyVals.Add("[customer_name]", customer.FirstName);
            templateKeyVals.Add("[product_name]", productName);


            List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
            foreach (IntelliLockManager.License license in licenses)
            {
                System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(new System.IO.MemoryStream(license.LicenseFile), attachmentFileName);
                attachments.Add(attachement);
            }

            EmailManager.Email email = new EmailManager.Email();
            email.Attachments = attachments;
            email.Body = EmailManager.Email.Helper.FormatEmailTemplate(emailTemplateLocation, templateKeyVals);
            email.FromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"];
            email.FromHost = ConfigurationManager.AppSettings["FromHost"];
            email.FromName = ConfigurationManager.AppSettings["FromName"];
            email.FromPort = ConfigurationManager.AppSettings["FromPort"];
            email.FromUserName = ConfigurationManager.AppSettings["FromUserName"];
            email.FromUserPassword = ConfigurationManager.AppSettings["FromUserPassword"];
            email.IsUsingSSL = false;
            email.Subject = emailSubject;
            email.ToEmailAddress = customer.Email;

            EmailManager.EmailManager emailManager = new EmailManager.EmailManager();
            emailManager.SendEmail(email);

        }


        #endregion

        #region Helpers

        string GetResponseParam(string paramName)
        {
            string _param = Request.Params[paramName];

            if (_param != null)
                return _param;

            return "";
        }

        private void WriteResponseValues()
        {
            string requestVars = string.Empty;
            foreach (string key in Request.Form.Keys)
            {
                requestVars += key + ": " + Request.Form[key] + Environment.NewLine;
            }
            File.WriteAllText(@"D:\Development\SimpleSqlite\SimpleSqliteWebsite\ResponseKeys.txt", requestVars);
        }

        private void SendTestEmail()
        {
            Dictionary<string, string> templateKeyVals = new Dictionary<string, string>();
            templateKeyVals.Add("[customer_name]", "Test FirstName");
            templateKeyVals.Add("[product_name]", "Test product name");

            //Create an email object
            version = ConfigurationManager.AppSettings["Version"];

            string apiEmailTemplateLocation = MapPath("~\\Releases\\" + version + "\\API\\EmailTemplate\\ApiEmailTemplate.html");

            EmailManager.Email email = new EmailManager.Email();

            email.Body = EmailManager.Email.Helper.FormatEmailTemplate(apiEmailTemplateLocation, templateKeyVals);
            email.FromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"];
            email.FromHost = ConfigurationManager.AppSettings["FromHost"];
            email.FromName = ConfigurationManager.AppSettings["FromName"];
            email.FromPort = ConfigurationManager.AppSettings["FromPort"];
            email.FromUserName = ConfigurationManager.AppSettings["FromUserName"];
            email.FromUserPassword = ConfigurationManager.AppSettings["FromUserPassword"];
            email.IsUsingSSL = false;
            email.Subject = ConfigurationManager.AppSettings["EmailSubject"];
            //email.ToEmailAddress = "Calvin_m@Hotmail.com";            
            email.ToEmailAddress = "mann.Calvin.s@gmail.com";
            

            EmailManager.EmailManager emailManager = new EmailManager.EmailManager();
            emailManager.SendEmail(email);
        }

        #endregion
    }
}
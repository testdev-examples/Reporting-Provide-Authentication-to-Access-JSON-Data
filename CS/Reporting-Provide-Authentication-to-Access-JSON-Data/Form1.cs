using DevExpress.DataAccess.Json;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Xtrareport_json_datasource_with_authorization.ReportCustomization;

namespace Xtrareport_json_datasource_with_authorization
{
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void CreateReportDataSourceFromConnectionStringButton_Click(object sender, EventArgs e) {
            // XtraReport1 does not have assigned data sources
            var report = new XtraReport1(); 

            // Create JsonDataSource from the specified connection string
            var jsonDataSource = CreateReportDataSourceFromConnectionString();
            // Retrieve data to populate the Report Designer's Field List
            jsonDataSource.Fill();

            report.DataSource = jsonDataSource;
            report.DataMember = "Customers";

            new DevExpress.XtraReports.UI.ReportDesignTool(report).ShowDesigner();
        }
        public static JsonDataSource CreateReportDataSourceFromConnectionString() {
            JsonDataSource jsonDataSource = new DevExpress.DataAccess.Json.JsonDataSource() {
                // The application's configuration file should include the "JsonConnection" connection string
                ConnectionName = "JsonConnection"
            };
            return jsonDataSource;
        }

        private void CreateReportDataSourceWithAuthenticationInCodeButton_Click(object sender, EventArgs e) {
            // XtraReport1 does not have assigned data sources
            var report = new XtraReport1();

            // Create JsonDataSource in code
            JsonDataSource jsonDataSource = CreateReportDataSourceWithAuthenticationInCode();
            // Retrieve data to populate the Report Designer's Field List
            jsonDataSource.Fill();

            report.DataSource = jsonDataSource;
            report.DataMember = "Customers";

            new DevExpress.XtraReports.UI.ReportDesignTool(report).ShowDesigner();
        }
        public static JsonDataSource CreateReportDataSourceWithAuthenticationInCode() {
            // Create a new UriJsonSource object and configure authentication data in it
            var jsonSource = new DevExpress.DataAccess.Json.UriJsonSource();
            jsonSource.Uri = new Uri(@"https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json");


            jsonSource.AuthenticationInfo.Username = "user";
            jsonSource.AuthenticationInfo.Password = "pwd";

            jsonSource.HeaderParameters.Add(new HeaderParameter("MyAuthHeader1", "secretToken1"));
            jsonSource.HeaderParameters.Add(new HeaderParameter("MyAuthHeader2", "secretToken2"));

            jsonSource.QueryParameters.Add(new QueryParameter("id", "123456"));
            jsonSource.QueryParameters.Add(new QueryParameter("name", "MyName"));
            // Create a JsonDataSource object and assign the UriJsonSource object to it
            var jsonDataSource = new DevExpress.DataAccess.Json.JsonDataSource() {
                JsonSource = jsonSource
            };

            return jsonDataSource;
        }

    }
    public static class JsonDatasourceAuthorization_Example {
        public static string ConvertReportWithMyUriJsonSourceTo191(string repxContent, out List<string> connectionString) {
            var report = new XtraReport();
            using(var ms = new MemoryStream(Encoding.UTF8.GetBytes(repxContent))) {
                report.LoadLayoutFromXml(ms);
            }

            connectionString = new List<string>();

            int i = 0;
            foreach(var component in report.ComponentStorage) {
                var jsonDS = (component as DevExpress.DataAccess.Json.JsonDataSource);
                var jsonSource = (jsonDS?.JsonSource as MyUriJsonSource);
                if(jsonSource != null) {
                    i++;
                    jsonDS.ConnectionName = string.Format("newJsonConnection_{0}{1}", report.Name, i.ToString());

                    var builder = new DbConnectionStringBuilder();
                    builder.Add("Uri", jsonSource.Uri.OriginalString);
                    builder.Add("UserName", jsonSource.UserName);
                    builder.Add("Password", jsonSource.Password);

                    connectionString.Add(string.Format("<add name=\"{0}\" connectionString=\"{1}\" providerName=\"JsonSourceProvider\" />",
                        jsonDS.ConnectionName, builder.ConnectionString));

                    jsonDS.JsonSource = null;
                }
            }
            using(var ms = new MemoryStream()) {
                report.SaveLayoutToXml(ms);
                ms.Position = 0;
                StreamReader reader = new StreamReader(ms);
                return reader.ReadToEnd();
            }
        }
    }
}

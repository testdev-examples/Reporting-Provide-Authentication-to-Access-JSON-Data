using DevExpress.DataAccess.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DevExpress.DataAccess.Native;
using System.Security.Cryptography;
using System.IO;
using System.Web.Security;

namespace Xtrareport_json_datasource_with_authorization.ReportCustomization {
    public class MyUriJsonSource : UriJsonSource {

        public string UserName { get; set; }

        [PasswordPropertyText(true)]
        public string Password { get; set; }

        protected override void LoadFromXml(XElement connection) {
            base.LoadFromXml(connection);
            var cred = MySecretStorage.SecretStorage.Instance.GetCredentials(Uri.Authority);
            if(cred != null) {
                UserName = cred.Item1;
                Password = cred.Item2;
            }
        }
    }
}

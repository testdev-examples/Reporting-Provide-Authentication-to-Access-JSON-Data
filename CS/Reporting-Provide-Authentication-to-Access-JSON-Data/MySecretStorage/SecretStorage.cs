using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Xtrareport_json_datasource_with_authorization.MySecretStorage {
    public class SecretStorage {
        const string storageFileName = "data.dat";
        const string stringSeparator = "@";

        static object lockObj = new object();
        static SecretStorage instance;
        Dictionary<string, Tuple<string, string>> storage;
        Dictionary<string, Tuple<string, string>> Storage {
            get {
                if(storage == null) {
                    storage = new Dictionary<string, Tuple<string, string>>();
                }
                return storage;
            }
        }

        public static SecretStorage Instance {
            get {
                if(instance == null) {
                    lock(lockObj) {
                        if(instance == null) {
                            instance = new SecretStorage();
                        }
                    }
                }
                return instance;
            }
        }

        private SecretStorage() {
            try {
                LoadData();
            } catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("SecretStorage: {0}", e);
            }
        }

        public Tuple<string, string> GetCredentials(string uri) {
            Tuple<string, string> cred;
            if(Storage.TryGetValue(uri.ToLowerInvariant(), out cred)) {
                return cred;
            }
            return null;
        }

        public void SaveCredentials(string uri, Tuple<string, string> cred) {
            if(!Storage.Any(x => x.Key.Equals(uri, StringComparison.InvariantCultureIgnoreCase))) {
                Storage.Add(uri, cred);
            } else {
                Storage[uri.ToLowerInvariant()] = cred;
            }
        }

        void LoadData() {
            var lines = File.ReadAllLines(storageFileName);
            foreach(var line in lines) {
                string[] strs = line.Split(stringSeparator.ToCharArray()[0]);
                if(strs.Length != 3) {
                    continue;
                }
                var str1 = Encoding.UTF8.GetString(Convert.FromBase64String(strs[0]));
                var str2 = Encoding.UTF8.GetString(Convert.FromBase64String(strs[1]));
                var str3 = Encoding.UTF8.GetString(Convert.FromBase64String(strs[2]));
                Storage.Add(str1, new Tuple<string, string>(str2, str3));
            }
        }
        void SaveData() {
            var strings = new List<string>();
            foreach(var item in Storage) {
                var str1 = Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Key));
                var str2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Value.Item1));
                var str3 = Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Value.Item2));
                strings.Add(str1 + stringSeparator + str2 + stringSeparator + str3);
            }
            File.WriteAllLines(storageFileName, strings.ToArray());
        }
        ~SecretStorage() {
            try {
                SaveData();
            } catch(Exception e) {
                System.Diagnostics.Debug.WriteLine("~SecretStorage: {0}", e);
            }
        }
    }
}

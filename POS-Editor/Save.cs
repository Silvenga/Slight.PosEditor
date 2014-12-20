#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

#endregion

namespace POS_Editor {

    internal class Save {

        public const string SaveLocation = "com.silvenga.pos-editor.save.db";

        public string Port {
            get;
            set;
        }

        public List<String> Profiles {
            get;
            set;
        }

        public int? Columns {
            get;
            set;
        }

        public int? Rows {
            get;
            set;
        }

        public static void SaveObject(Save obj) {

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SaveLocation);
            var json = new JavaScriptSerializer().Serialize(obj);

            File.WriteAllText(path, json);
        }

        public static Save OpenObject() {

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SaveLocation);

            if(!File.Exists(path)) {
                return null;
            }

            try {

                var json = File.ReadAllText(path);
                var save = new JavaScriptSerializer().Deserialize<Save>(json);
                return save;

            } catch(Exception) {

                return null;
            }
        }

    }

}
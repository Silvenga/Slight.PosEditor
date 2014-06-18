using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace POS_Editor {

    class Save {

        public const string SaveLocation = "com.silvenga.pos-editor.save.db";

        public string Port {
            get;
            set;
        }

        public List<String> Profiles {
            get;
            set;
        }

        public static void SaveObject(Save obj) {

            string json = new JavaScriptSerializer().Serialize(obj);

            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SaveLocation), json);
        }

        public static Save OpenObject() {

            if(!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    SaveLocation)))
                return null;

            try {

                string json = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), SaveLocation));
                Save save = new JavaScriptSerializer().Deserialize<Save>(json);
                return save;

            } catch(Exception) {
                return null;
            }

            return null;
        }
    }
}

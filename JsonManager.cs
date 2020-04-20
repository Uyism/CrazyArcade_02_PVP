using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

    public class JsonFactory
    {
        public static string Load(string file_name) 
        {
            string path = GetPath() + file_name;
            System.IO.FileInfo file = new System.IO.FileInfo(path);
            if (file.Exists)
            {
                string str = File.ReadAllText(path);
                return str;
            }

            return null;
        }

        public static void Write(string file_name, object data) 
        {
            string path = GetPath() + file_name;
            string str = JsonUtility.ToJson(data);
            File.WriteAllText(path, str);
        }

         public static void WriteString(string file_name, string data)
    {
            string path = GetPath() + file_name;
            
            File.WriteAllText(path, data);
         }

    static string GetPath()
        {
            #if UNITY_EDITOR
                return Application.dataPath + "/Resources/Data/";
            #elif UNITY_ANDROID
                return Application.persistentDataPath+"/";
            #else
                return Application.dataPath +"/";
            #endif
    }
}

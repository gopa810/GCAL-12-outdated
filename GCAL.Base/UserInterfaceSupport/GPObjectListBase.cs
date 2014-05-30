using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace GCAL.Base
{
    public class GPObjectListBase
    {
        public bool Modified = false;

        public virtual string GetDefaultResourceName()
        {
            throw new Exception("DefaultResourecName should be overriden in child class.");
        }

        public virtual void InsertNewObjectFromStrings(string[] parts)
        {
            throw new Exception("New object from strings has to be overriden in child class.");
        }

        /// <summary>
        /// Reading content from stream
        /// </summary>
        /// <param name="reader"></param>
        public void ReadStream(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split('\t');
                InsertNewObjectFromStrings(parts);
            }
        }

        public void InitializeFromResources()
        {
            using (StringReader reader = new StringReader(GetDefaultResourceName()))
            {
                ReadStream(reader);
            }
        }

        public virtual void SaveData(StreamWriter writer)
        {
            throw new Exception("SaveData needs to be overiden");
        }

        public virtual string GetDefaultFileName()
        {
            return "List.txt";
        }

        public string getFullPathForFile(string fileName)
        {
            string dir = GPFileHelper.getAppDataDirectory();
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.Combine(dir, fileName);
        }

        ~GPObjectListBase()
        {
            if (Modified)
            {
                string fileName = getFullPathForFile(GetDefaultFileName());
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    SaveData(sw);
                }
            }
        }
    }
}

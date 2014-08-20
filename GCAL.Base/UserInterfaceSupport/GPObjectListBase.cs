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
        public enum FileKey
        {
            Primary,
            Secondary,
            Tertial
        }

        public bool Modified = false;

        public virtual string GetDefaultResourceForKey(FileKey key)
        {
            throw new Exception("DefaultResourecName should be overriden in child class.");
        }

        public virtual void InsertNewObjectFromStrings(string[] parts, FileKey key)
        {
            throw new Exception("New object from strings has to be overriden in child class.");
        }

        public virtual FileKey[] GetFileKeys()
        {
            return new FileKey[] { FileKey.Primary };
        }

        /// <summary>
        /// Reading content from stream
        /// </summary>
        /// <param name="reader"></param>
        public void ReadStream(TextReader reader, FileKey key)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split('\t');
                InsertNewObjectFromStrings(parts, key);
            }
        }

        /// <summary>
        /// Reads data for object list from file on permanent storage
        /// </summary>
        public void InitializeFromResources()
        {
            FileKey[] keys = GetFileKeys();

            foreach (FileKey key in keys)
            {
                string fileName = getFullPathForFile(GetDefaultFileNameForKey(key));
                if (File.Exists(fileName))
                {
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        ReadStream(reader, key);
                    }
                }
                else
                {
                    using (StringReader reader = new StringReader(GetDefaultResourceForKey(key)))
                    {
                        ReadStream(reader, key);
                    }
                }
            }

        }

        public virtual void SaveData(StreamWriter writer, FileKey key)
        {
            throw new Exception("SaveData needs to be overiden");
        }

        public virtual string GetDefaultFileNameForKey(FileKey key)
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

        public virtual void Save()
        {
            if (Modified)
            {
                FileKey[] keys = GetFileKeys();

                foreach (FileKey key in keys)
                {
                    string fileName = getFullPathForFile(GetDefaultFileNameForKey(key));
                    using (StreamWriter sw = new StreamWriter(fileName))
                    {
                        SaveData(sw, key);
                    }
                }
            }
        }
    }
}

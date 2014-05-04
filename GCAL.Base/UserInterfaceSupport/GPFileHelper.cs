using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace GCAL.Base
{
    public class GPFileHelper
    {
        private static string VersionString = string.Empty;

        public static string getAppDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GCAL");
        }

        public static string getAppDataFile(string fileName)
        {
            return Path.Combine(getAppDataDirectory(), fileName);
        }

        public static string getLanguageDirectory()
        {
            return Path.Combine(getAppDataDirectory(), "languages");
        }

        public static string[] EnumerateLanguageFiles()
        {
            string languageFilesDirectory = getLanguageDirectory();
            List<String> list = new List<string>();

            if (Directory.Exists(languageFilesDirectory))
            {
                return Directory.GetFiles(languageFilesDirectory);
            }
            else
            {
                Directory.CreateDirectory(languageFilesDirectory);
            }

            return new string[] {};
        }

        public static string UniqueFile(string dir, string filePrefix, string fileSuffix)
        {
            for (int i = 0; i < 1000; i++)
            {
                string filePath = Path.Combine(dir, filePrefix + i.ToString() + fileSuffix);
                if (!File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return string.Empty;
        }

        public static string FileVersion
        {
            get
            {
                if (VersionString.Length == 0)
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    if (fvi.FileMinorPart == 0)
                        VersionString = fvi.FileMajorPart.ToString();
                    else
                    {
                        VersionString = string.Format("{0}.{1}", fvi.FileMajorPart, fvi.FileMinorPart);
                    }
                }

                return VersionString;
            }
        }

    }
}

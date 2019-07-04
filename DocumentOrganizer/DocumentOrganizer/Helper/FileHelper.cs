﻿using System.Collections.Generic;
using System.IO;


namespace DocumentOrganizer.Helper
{
    public class FileHelper
    {
		//Vininius
        public static List<FileInfo> LoadDocuments(string sourcePath)
        {
            DirectoryInfo directory = new DirectoryInfo(sourcePath);

            List<FileInfo> fileList = new List<FileInfo>();

            fileList.AddRange(directory.GetFiles());

            return fileList;
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
		//ABCBolinhas
    }
}

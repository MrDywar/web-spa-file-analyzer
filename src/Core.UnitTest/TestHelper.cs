using System;
using System.Collections.Generic;
using System.Text;

namespace Core.UnitTest
{
    public static class TestHelper
    {
        public static readonly string PATH_TO_TESTFILES = GetFullPathToFile("TestFiles");

        public static string GetFullPathToFile(string pathRelativeUnitTestingFile)
        {
            string folderProjectLevel = GetPathToCurrentUnitTestProject();
            string final = System.IO.Path.Combine(folderProjectLevel, pathRelativeUnitTestingFile);

            return final;
        }

        /// <summary>
        /// Get the path to the current unit testing project.
        /// </summary>
        private static string GetPathToCurrentUnitTestProject()
        {
            string pathAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string folderAssembly = System.IO.Path.GetDirectoryName(pathAssembly);
            if (folderAssembly.EndsWith("\\") == false) folderAssembly = folderAssembly + "\\";
            string folderProjectLevel = System.IO.Path.GetFullPath(folderAssembly + "..\\..\\..\\");

            return folderProjectLevel;
        }
    }
}

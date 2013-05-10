using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace ECSSS_Documenter
{
    class Documenter
    {
        public const string ecsss_root_source = @"C:\ECSSS\csss";
        public const string docs_root_destination = @"C:\ECSSS\csss\docs";

        static void Main(string[] args)
        {
            string[] ecsss_root_files = Directory.GetFiles(ecsss_root_source);
            string[] ecsss_root_dirs = Directory.GetDirectories(ecsss_root_source);
            string[] current_files;

            if (!Directory.Exists(docs_root_destination))
            {
                Directory.CreateDirectory(docs_root_destination);
            }

            foreach (string s in ecsss_root_files)
            {
                current_files = s.Split('\\');
                //Console.WriteLine(current_files[current_files.Length - 1]);
            }
            foreach (string s in ecsss_root_dirs)
            {
//                Console.WriteLine(s);
            }

            foreach (string s in ecsss_root_files)
            {
                //Console.WriteLine(docs_root_destination + s);
            }

            FileSystem fs = new FileSystem(ecsss_root_source);
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Class:  FileSystem
    /// Description:  Crawls the root directory and creates a list containing sublists
    /// which contain sublists (etc) representing the tree structure of the filesystem 
    /// starting at the root directory passed to the constructor.  Still debating on
    /// whether to list directories to exclude or (if there are too many to exclude)
    /// list directories to include instead.  Not sure which is less.
    /// </summary>
    class FileSystem
    {
        public static List<FileSystem> directories { get; set; }
        public static string[] restrictedDirectories = {
                                                          "CVS"
                                                          ,"reports"
                                                          ,"reports_2008"
                                                          ,"reports_2010"
                                                          ,"report_builder"
                                                          ,"logs"
                                                          ,"build"
                                                      };
        public static string[] targetDirectories = { 
                                                       "web"
                                                       ,"src"
                                                       ,"pages"
                                                   };
        public string root {get; private set; }

        public FileSystem(string rootPath)
        {
            root = rootPath;
            getDirStructure(rootPath);
        }

        public List<FileSystem> getDirStructure(string rootPath)
        {
            List<string> rootDirs = Directory.GetDirectories(rootPath).ToList();
            List<string> dirsTemp = new List<string>();
            //int rootDirDepth = calculateDirectoryDepth();

            foreach (string s in rootDirs)
            {
                dirsTemp.Add(s);
            }

            foreach (string s in dirsTemp)
            {
                string[] pathParts = s.Split('\\');
                if (!targetDirectories.Contains(pathParts[pathParts.Length - 1]))
                {
                    rootDirs.Remove(s);
                }
            }

            foreach (string s in rootDirs)
            {
                string foo = s;
                for (int i = 0; i < calculateDirectoryDepth(); i++)
                {
                    foo = " " + foo;
                }
                Console.WriteLine(foo);
                FileSystem fs = new FileSystem(s);
            }
            return directories;
        }


        public int calculateDirectoryDepth()
        {
            return root.Count(r => r == '\\') - 1;
        }
    }
}

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

            FileSystem fs = new FileSystem(ecsss_root_source, 0);
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// Class:  FileSystem
    /// Description:  Crawls the root directory and creates a list containing sublists
    /// which contain sublists (etc) representing the tree structure of the filesystem 
    /// starting at the root directory passed to the constructor.
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

        public FileSystem(string rootPath, int depth)
        {
            getDirStructure(rootPath, depth);
        }

        public static List<FileSystem> getDirStructure(string rootPath, int depth)
        {
            List<string> rootDirs = Directory.GetDirectories(rootPath).ToList();
            List<string> dirsTemp = new List<string>();
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
                for (int i = 0; i < depth; i++)
                {
                    foo = " " + foo;
                }
                Console.WriteLine(foo);
                FileSystem fs = new FileSystem(s, ++depth);
            }
            return directories;
        }
    }
}

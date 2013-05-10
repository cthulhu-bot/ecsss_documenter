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
            foreach (string s in rootDirs)
            {
                Console.WriteLine(s + "\nLast Index: " + s.LastIndexOf('\\') + "\nLength: " + s.Length);
                Console.WriteLine(s.Substring(s.LastIndexOf('\\') - 1, s.Length - 1));
            }
            //rootDirs.RemoveAll(r => !targetDirectories.Contains(r.Substring(r.LastIndexOf('\\') - 1, r.Length)));
            // Need a list to compare the original list to in order to remove directories you don't wish to scan
            //List<string> dirsTemp = new List<string>();

            // Need to manually add list elements from one list to the comparing list
            // Attempting to set one list = to another only makes both pointers point to the same list (i think)
            //foreach (string s in rootDirs)
            //{
            //    dirsTemp.Add(s);
            //}

            //foreach (string s in rootDirs)
            //{
            //    string[] pathParts = s.Split('\\');
            //    rootDirs.RemoveAll(r => !targetDirectories.Contains(pathParts[pathParts.Length - 1]));
            //    //if (!targetDirectories.Contains(pathParts[pathParts.Length - 1]))
            //    //{
            //    //    rootDirs.Remove(s);
            //    //}
            //}

            foreach (string s in rootDirs)
            {
                List<string> files = getCurrentPathFiles(s);
                string foo = s.PadLeft(calculateDirectoryDepth() + s.Length, ' ');
                //Console.WriteLine(foo);
                FileSystem fs = new FileSystem(s);
            }
            return directories;
        }

        public List<string> getRootPathFiles()
        {
            string[] root_files = Directory.GetFiles(root);
            List<string> files = root_files.ToList();
            return files;
        }

        public List<string> getCurrentPathFiles(string path)
        {
            string[] root_files = Directory.GetFiles(path);
            List<string> files = root_files.ToList();
            return files;
        }


        public int calculateDirectoryDepth()
        {
            return root.Count(r => r == '\\') - 1;
        }
    }
}

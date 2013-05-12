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
            if (!Directory.Exists(docs_root_destination))
            {
                Directory.CreateDirectory(docs_root_destination);
            }

            // Initialize filesystem below the ecsss_root_source directory to retrieve all files to be documented
            FileSystem fs = new FileSystem(ecsss_root_source);
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        public void runDocco(string fileName)
        {
            string cmd = "docco" + fileName;
            System.Diagnostics.Process.Start("CMD.exe", cmd);
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
        public static string[] targetDirectories = { 
                                                       "web"
                                                       ,"src"
                                                       ,"pages"
                                                   };
        public static string[] targetExtensions = {
                                                      ".js"
                                                      ,".cs"
                                                      ,".query"
                                                  };
        public List<string> rootFiles { get; set; }
        public string root {get; private set; }

        /// <summary>
        /// Public constructor initializes root directory structure
        /// </summary>
        /// <param name="rootPath"></param>
        public FileSystem(string rootPath)
        {
            root = rootPath;
            getDirStructure(root);
        }

        public List<FileSystem> getDirStructure(string rootPath)
        {
            List<string> rootDirs = Directory.GetDirectories(rootPath).ToList();
            
            // Remove all directories except those contained in the targetDirectories array
            rootDirs.RemoveAll(r => !targetDirectories.Any(r.Substring(r.LastIndexOf('\\') + 1, r.Length - r.LastIndexOf('\\') - 1).Contains));

            foreach (string s in rootDirs)
            {
                rootFiles = getCurrentPathFiles(s);
                
                //Remove all files not containing the targeted extensions
                rootFiles.RemoveAll(r => !targetExtensions.Any(r.Substring(r.LastIndexOf('.'), r.Length - r.LastIndexOf('.')).Contains)
                    || r.Substring(r.LastIndexOf('.'), r.Length - r.LastIndexOf('.')).Contains(".css"));

                Console.WriteLine(s.PadLeft(calculateDirectoryDepth() + s.Length, ' '));
                foreach (string f in rootFiles)
                {
                    Console.WriteLine(f.PadLeft(calculateDirectoryDepth() + f.Length, ' '));

                    //File.Copy(f, "dest");
                }

                // Recursively continue iterating through the filesystem by creating new instances of the FileSystem class for each
                // subdirectory beneath the root directory
                FileSystem fs = new FileSystem(s);
            }
            return directories;
        }

        /// <summary>
        /// Name:  getRootPathFiles
        /// Description:  Get all files in the class's root directory
        /// </summary>
        /// <returns></returns>
        public List<string> getRootPathFiles()
        {
            string[] root_files = Directory.GetFiles(root);
            List<string> files = root_files.ToList();
            return files;
        }

        /// <summary>
        /// Name:  getCurrentPathFiles
        /// Description:  Get all files contained in the path parameter
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace ECSSS_Documenter
{
    /// <summary>
    /// Name:  Documenter
    /// Description:  
    /// </summary>
    class Documenter
    {
        public const string ecsss_root_source = @"C:\ECSSS\csss";
        public const string docs_root_destination = @"C:\ECSSS\csss\docs\";
        public const string docs_root_destination_temp = @"C:\ECSSS\csss\docs\docs\";
        public const string batch_file = @"C:\ECSSS\csss\docs\test.bat";
        
        public static string[] targetDirectories = { 
                                                       "web"
                                                       ,"src"
                                                       ,"pages"
                                                       ,"app"
                                                       ,"script"
                                                   };
        public static string[] targetExtensions = {
                                                      ".js"
                                                      //,".cs"
                                                      //,".query"
                                                  };
        public static List<string> fileList = new List<string>();

        static void Main(string[] args)
        {
            if (!Directory.Exists(docs_root_destination) || !Directory.Exists(docs_root_destination_temp))
            {
                Directory.CreateDirectory(docs_root_destination);
                Directory.CreateDirectory(docs_root_destination_temp);
            }

            // Initialize filesystem below the ecsss_root_source directory to retrieve all files to be documented
            FileSystem fs = new FileSystem(ecsss_root_source, targetExtensions, targetDirectories);
            copyFilesToDocDirectory(fs);

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        /// <summary>
        /// Name:  copyFilesToDocDirectory
        /// Description: 
        /// </summary>
        /// <param name="fs"></param>
        public static void copyFilesToDocDirectory(FileSystem fs)
        {
            foreach (string s in fs.files)
            {
                string str = s.Replace('\\', '.');
                str = str.Remove(0, str.IndexOf("csss") + 5);
                if (File.Exists(docs_root_destination + str))
                    File.Delete(docs_root_destination + str);
                File.Copy(s, docs_root_destination + str);
                Console.WriteLine("{0} copied to {1}", s, docs_root_destination + str);
                fileList.Add(str);
            }
            runDocco(fileList);
        }

        /// <summary>
        /// Name:  runDocco
        /// Description:  For every file copied try running docco
        /// </summary>
        /// <param name="fileName"></param>
        public static void runDocco(List<string> fileList)
        {
            createBatchFile(fileList);
            ProcessStartInfo p = new ProcessStartInfo();
            p.UseShellExecute = false;
            p.RedirectStandardOutput = true;
            p.RedirectStandardError = true;
            p.Verb = "runas";
            p.WorkingDirectory = @"C:\ECSSS\csss\docs\";
            p.FileName = @"test.bat";
            string targetFile = Path.Combine(p.WorkingDirectory, p.FileName);
            var fileInfo = new FileInfo(targetFile);
            if (fileInfo.Exists)
            {
                try
                {
                    Process.Start(p);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Console execution error: " + e);
                }
            }
            else
            {
                throw new FileNotFoundException("The requested file was not found: " + fileInfo.FullName);
            }
        }

        /// <summary>
        /// Name: createBatchFile
        /// Description: Create batch file to execute docco commands
        /// </summary>
        /// <param name="file"></param>
        public static void createBatchFile(List<string> fileList)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in fileList)
            {
                sb.Append("docco " + s + "\n");
            }
            try
            {
                using (StreamWriter outfile = new StreamWriter(batch_file))
                {
                    outfile.Write(sb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Batch file creation error:  " + e.Message);
                Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());
            }
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
        private string[] targetDirectories { get; set; }
        private string[] targetExtensions { get; set; }
        public List<string> files { get; set; }
        public string root {get; private set; }

        /// <summary>
        /// Public constructor initializes root directory structure
        /// </summary>
        /// <param name="rootPath"></param>
        public FileSystem(string rootPath, string[] targetExtensions, string[] targetDirectories)
        {
            root = rootPath;
            this.targetDirectories = targetDirectories;
            this.targetExtensions = targetExtensions;
            this.files = getDirStructure();
        }

        public List<string> getDirStructure()
        {
            List<string> rootFiles = new List<string>();
            List<string> rootDirs = Directory.GetDirectories(root).ToList();
            
            // Remove all directories except those contained in the targetDirectories array
            rootDirs.RemoveAll(r => !targetDirectories.Any(r.Substring(r.LastIndexOf('\\') + 1, r.Length - r.LastIndexOf('\\') - 1).Contains));

            foreach (string s in rootDirs)
            {
                // Recursively continue iterating through the filesystem by creating new instances of the FileSystem class for each
                // subdirectory beneath the root directory
                FileSystem fs = new FileSystem(s, this.targetExtensions, this.targetDirectories);
                // retrieve all files from subdirectories
                rootFiles = rootFiles.Union(fs.files).ToList();
            }

            // Add all files in root directory
            foreach (string s in getRootPathFiles())
            {
                rootFiles.Add(s);
            }

            return rootFiles;
        }

        /// <summary>
        /// Name:  getRootPathFiles
        /// Description:  Get all files in the class's root directory
        /// </summary>
        /// <returns></returns>
        public List<string> getRootPathFiles()
        {
            List<string> files = Directory.GetFiles(root).ToList();
            //Remove all files not containing the targeted extensions
            files.RemoveAll(r => !r.Contains('.')
                || !targetExtensions.Any(r.Substring(r.LastIndexOf('.'), r.Length - r.LastIndexOf('.')).Contains)
                || r.Substring(r.LastIndexOf('.'), r.Length - r.LastIndexOf('.')).Contains(".css"));
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
            List<string> files = Directory.GetFiles(path).ToList();
            //Remove all files not containing the targeted extensions
            files.RemoveAll(r => !r.Contains('.')
                || !targetExtensions.Any(r.Substring(r.LastIndexOf('.'), r.Length - r.LastIndexOf('.')).Contains)
                || r.Substring(r.LastIndexOf('.'), r.Length - r.LastIndexOf('.')).Contains(".css"));
            return files;
        }


        public int calculateDirectoryDepth()
        {
            return root.Count(r => r == '\\') - 1;
        }
    }
}

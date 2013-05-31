using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;

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
                                                       ,"student"
                                                   };
        public static string[] targetExtensions = {
                                                      ".js"
                                                      //,".cs"
                                                      //,".query"
                                                  };

        static void Main(string[] args)
        {
            if (!Directory.Exists(docs_root_destination) || !Directory.Exists(docs_root_destination_temp))
            {
                Directory.CreateDirectory(docs_root_destination);
                Directory.CreateDirectory(docs_root_destination_temp);
            }

            // Initialize filesystem below the ecsss_root_source directory to retrieve all files to be documented
            FileSystem fs = new FileSystem(ecsss_root_source, targetExtensions, targetDirectories);
            Documenter doc = new Documenter();
            doc.copyFilesToDocDirectory(fs);

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        /// <summary>
        /// Name:  copyFilesToDocDirectory
        /// Description: 
        /// </summary>
        /// <param name="fs"></param>
        public void copyFilesToDocDirectory(FileSystem fs)
        {
            foreach (string currentFile in fs.files)
            {
                string str = currentFile.Replace('\\', '.');
                str = str.Remove(0, str.IndexOf("csss") + 5);
                if (File.Exists(docs_root_destination + str))
                    File.Delete(docs_root_destination + str);
                File.Copy(currentFile, docs_root_destination + str);
                Console.WriteLine("{0} copied to {1}", currentFile, docs_root_destination + str);
                this.runDocco(str);
            }
        }

        /// <summary>
        /// Name:  runDocco
        /// Description:  For every file copied try running docco
        /// </summary>
        /// <param name="fileName"></param>
        public void runDocco(string fileName)
        {
            this.createBatchFile(fileName);
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.WorkingDirectory = @"C:\ECSSS\csss\docs\";
            p.StartInfo.FileName = @"test.bat";
            string targetFile = Path.Combine(p.StartInfo.WorkingDirectory, p.StartInfo.FileName);
            var fileInfo = new FileInfo(targetFile);
            if (fileInfo.Exists)
            {
                if (this.isAdministrator)
                {
                    try
                    {
                        p.Start();
                        p.WaitForExit();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Console execution error: " + e);
                    }
                }
                else
                {
                    Console.WriteLine("You must be running in administrator for this to work\nPlease rerun with appropriate credentials");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadLine();
                    Environment.Exit(0);
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
        public void createBatchFile(string file)
        {
            StringBuilder sb = new StringBuilder("docco " + file);
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
                Console.WriteLine("Batch file for " + file + " may not have been created properly");
            }
        }

        private bool isAdministrator
        {
            get
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);
                return wp.IsInRole(WindowsBuiltInRole.Administrator);
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

            Parallel.ForEach(rootDirs, currentDirectory =>
            {
                // Recursively continue iterating through the filesystem by creating new instances of the FileSystem class for each
                // subdirectory beneath the root directory
                FileSystem fs = new FileSystem(currentDirectory, this.targetExtensions, this.targetDirectories);
                // retrieve all files from subdirectories
                rootFiles = rootFiles.Union(fs.files).ToList();
            });

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

        public void testParallelImplementation()
        {
        }
    }
}

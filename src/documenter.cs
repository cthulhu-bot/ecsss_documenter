using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ECSSS_Documenter
{
    class Documenter
    {
        public static string ecsss_root_source { get; private set; }
        public const string docs_root_destination = @"C:\ECSSS\csss\docs";

        static void Main(string[] args)
        {
            ecsss_root_source = @"C:\ECSSS\csss";
            string[] ecsss_root_files = Directory.GetFiles(ecsss_root_source);
            string[] ecsss_root_dirs = Directory.GetDirectories(ecsss_root_source);

            foreach (string s in ecsss_root_files)
            {
                Console.WriteLine(s);
            }
            foreach (string s in ecsss_root_dirs)
            {
                Console.WriteLine(s);
            }

            if (!Directory.Exists(docs_root_destination))
            {
                Directory.CreateDirectory(docs_root_destination);
            }




            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ECSSS_Documenter
{
    class Documenter
    {
        public static string ecsss_root_path { get; private set; }

        static void Main(string[] args)
        {
            ecsss_root_path = @"C:\ECSSS\csss";
            string[] ecsss_root_files = Directory.GetFiles(ecsss_root_path);
            string[] ecsss_root_dirs = Directory.GetDirectories(ecsss_root_path);

            foreach (string s in ecsss_root_files)
            {
                Console.WriteLine(s);
            }
            foreach (string s in ecsss_root_dirs)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}

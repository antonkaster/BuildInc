using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace BuildInc
{
    class Program
    {

        static int Main(string[] args)
        {
            //args = new string[] { "debug", @"D:\Dropbox\Programming\VCShPr\RefSyncIRB\RefSyncLib\" };

            if (args.Length == 0)
            {
                ConsoleLogLine("use:\r\n\t BuildInc.exe [ /r | debug | release ] project_dir\r\nexample:\r\n\t $(SolutionDir).\\BuildInc.exe $(ConfigurationName) $(ProjectDir)");
                return -1;
            }

            bool isRelease = false;
            string projectDir = "";
            int dirArgIndex = 0;

            switch(args[0].ToLower())
            {
                case "/r":
                case "release":
                    isRelease = true;
                    dirArgIndex = 1;
                    break;  
                case "debug":
                    isRelease = false;
                    dirArgIndex = 1;
                    break; 
            }

            if (args.Length < dirArgIndex + 1)
            {
                ConsoleLogLine("need project directory in arguments!");
                return -1;
            }

            projectDir = (args[dirArgIndex] + @"\").Replace(@"\\", @"\");

            ConsoleLogLine("project: " + projectDir);

            if(isRelease)
                ConsoleLogLine("release and build increment");
            else
                ConsoleLogLine("build only increment");

            List<IVersionChanger> versionChangers = new List<IVersionChanger>();
            versionChangers.Add(new AssemblyVersionChanger(projectDir));
            versionChangers.Add(new CoreProjectVersionChanger(projectDir));

            try
            {
                foreach(var changer in versionChangers)
                {
                    if (changer.CanChange)
                    {
                        if (isRelease)
                            changer.ReleaseIncrement();
                        else
                            changer.BuildIncrement();

                        changer.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleLogLine("Error: " + ex.Message);
                return -1;
            }
            return 0;
        }



        private static void ConsoleLogLine(string text)
        {
            Console.WriteLine("BuildInc: " + text);
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuildInc
{
    public class AssemblyVersionChanger : IVersionChanger
    {
        public bool CanChange  => File.Exists(assemblyFile);

        private readonly string strPattern_1 = @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]";
        private readonly string strPattern_2 = @"\[assembly\: AssemblyFileVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]";

        private readonly string assemblyFile;
        
        private string fileContent;
        

        public AssemblyVersionChanger(string projectDir)
        {
            if(string.IsNullOrEmpty(projectDir))
                throw new ArgumentException("Project dir can't be null or empty!");

            assemblyFile = Path.Combine(projectDir, @"Properties\AssemblyInfo.cs");

            if (CanChange)
                fileContent = File.ReadAllText(assemblyFile);
        }

        public void BuildIncrement()
        {
            if (!CanChange)
                return;

            fileContent = GetResult(strPattern_1, fileContent, "AssemblyVersion", false);
            fileContent = GetResult(strPattern_2, fileContent, "AssemblyFileVersion", false);
        }

        public void ReleaseIncrement()
        {
            if (!CanChange)
                return;

            fileContent = GetResult(strPattern_1, fileContent, "AssemblyVersion", true);
            fileContent = GetResult(strPattern_2, fileContent, "AssemblyFileVersion", true);
        }

        public void Save()
        {
            if (!CanChange)
                return;
            
            File.WriteAllText(assemblyFile, fileContent);
        }

        private string GetResult(string pattern, string str, string app, bool isRelease)
        {
            Console.Write("[AssemblyInfo] " + app);
            int buildVersion, releaseVersion;

            Regex r = new Regex(pattern);

            Match m = r.Match(str);
            Console.Write($": {m.Groups[1]}.{m.Groups[2]}.{m.Groups[3]}.{m.Groups[4]} -> ");

            releaseVersion = Convert.ToInt32(m.Groups[3].Value);
            buildVersion = Convert.ToInt32(m.Groups[4].Value);

            if (isRelease)
                releaseVersion++;
            buildVersion++;

            string newVersion = $"{m.Groups[1]}.{m.Groups[2]}.{releaseVersion}.{buildVersion}";
            Console.WriteLine(newVersion);

            string rz = $"[assembly: {app}(\"{newVersion}\")]";
            return r.Replace(str, rz);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BuildInc
{
    public class CoreProjectVersionChanger : IVersionChanger
    {
        public bool CanChange { get; private set; } = true;

        private readonly string projectFile = "";
        private readonly XDocument xml;
        private readonly XElement assemblyVersion;
        private readonly XElement fileVersion;

        public CoreProjectVersionChanger(string projectDir)
        {
            if (string.IsNullOrEmpty(projectDir))
                throw new ArgumentException("Project dir can't be null or empty!");

            if(!Directory.Exists(projectDir))
            {
                CanChange = false;
                return;
            }

            projectFile = Directory.GetFiles(projectDir, "*.csproj", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

            if(string.IsNullOrEmpty(projectFile))
            {
                CanChange = false;
                return;
            }
            
            xml = XDocument.Load(projectFile);

            XElement versionRoot = xml
                ?.Descendants("Project")?.FirstOrDefault()
                ?.Descendants("PropertyGroup")?.FirstOrDefault();

            if(versionRoot == null)
            {
                CanChange = false;
                return;
            }

            assemblyVersion = versionRoot?.Descendants("AssemblyVersion")?.FirstOrDefault();
            fileVersion = versionRoot?.Descendants("FileVersion")?.FirstOrDefault();

        }


        public void BuildIncrement()
        {
            if (!CanChange)
                return;
            Increment(false);
        }

        public void ReleaseIncrement()
        {
            if (!CanChange)
                return;
            Increment(true);
        }

        private void Increment(bool isRelease)
        {
            if (assemblyVersion != null)
            {
                Console.Write($"[ProjectFile] AssemblyVersion: {assemblyVersion.Value} -> ");

                string[] split = assemblyVersion.Value.Split('.');

                if (split.Length > 0 && int.TryParse(split[split.Length - 1], out int build))
                    split[split.Length - 1] = (++build).ToString();

                if (split.Length > 1 && isRelease && int.TryParse(split[split.Length - 2], out int release))
                    split[split.Length - 2] = (++release).ToString();

                assemblyVersion.Value = string.Join(".", split);

                Console.WriteLine($"{assemblyVersion.Value}");
            }

            if (fileVersion != null)
            {
                Console.Write($"[ProjectFile] FileVersion: {fileVersion.Value} -> ");

                string[] split = fileVersion.Value.Split('.');

                if (split.Length > 0 && int.TryParse(split[split.Length - 1], out int build))
                    split[split.Length - 1] = (++build).ToString();

                if (split.Length > 1 && isRelease && int.TryParse(split[split.Length - 2], out int release))
                    split[split.Length - 2] = (++release).ToString();

                fileVersion.Value = string.Join(".", split);

                Console.WriteLine($"{fileVersion.Value}");
            }
        }

        public void Save()
        {
            if (!CanChange)
                return;
            xml.Save(projectFile);
        }
    }
}

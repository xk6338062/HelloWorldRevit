using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDirectory = Environment.CurrentDirectory;
            var programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var directoryInfo = new DirectoryInfo(Path.Combine(programdata, @"Autodesk\Revit\Addins"));
            var files = new DirectoryInfo(currentDirectory).GetFiles("*.addin");
            if (files.Length != 0 && directoryInfo.Exists)
            {
                var directories = directoryInfo.GetDirectories();
                try
                {
                    foreach (var directoryInfo0 in directories)
                    {
                        if (directoryInfo0.Name.StartsWith("20") && double.Parse(directoryInfo0.Name) >= 2017.0)
                        {
                            using(StreamReader streamReader=new StreamReader(files[0].FullName))
                            {
                                string path = Path.Combine(directoryInfo0.FullName, files[0].Name);
                                if (File.Exists(path)) File.Delete(path);
                                using(StreamWriter streamWriter=new StreamWriter(path))
                                {
                                    while(streamReader.Peek()!=-1)
                                    {
                                        var text = streamReader.ReadLine();
                                        var value = string.Empty;
                                        if (text != null && text.Contains("{0}"))
                                        {
                                            value = string.Format(text, currentDirectory);
                                        }
                                        else if (text != null)
                                            value = text;
                                        streamWriter.WriteLine(value);
                                    }
                                    streamWriter.Flush();
                                    streamWriter.Close();
                                }
                                streamReader.Close();
                            }
                        }
                    }

                }
                catch (Exception e)
                {

                }
            }
        }
    }
}

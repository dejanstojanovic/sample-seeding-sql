using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Sample.Seeding.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmd = new CommandLineApplication();
            var argName = cmd.Option("-n | --name <value>", "Script name", CommandOptionType.SingleValue);
            var argProject = cmd.Option("-p | --project <value>", "Project file path or project name", CommandOptionType.SingleValue);
            var argOutput = cmd.Option("-o | --output <value>", "Output folder path", CommandOptionType.SingleValue);

            cmd.OnExecute(() =>
            {
                //String currentExecutionFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                String currentExecutionFolder = System.Environment.CurrentDirectory;
                String projectFile = null;
                String outputFolder = currentExecutionFolder;

                // Check name param
                if (!argName.HasValue())
                    throw new ArgumentNullException(paramName: "name", message: "Argument name required!");


                // Calculate output path
                if (!argOutput.HasValue())
                    outputFolder = currentExecutionFolder;
                else if (!Path.IsPathRooted(argOutput.Value()))
                    outputFolder = Path.Combine(currentExecutionFolder, argOutput.Value());
                else
                    outputFolder = argOutput.Value();

                // Create output folder if missing
                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                // Calculate absolute output file path
                var outputFile = Path.Combine(outputFolder, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{argName.Value()}.sql");

                // Find project file
                if (!argProject.HasValue())
                    projectFile = FindProject(currentExecutionFolder);

                string FindProject(string folder)
                {
                    var files = Directory.GetFiles(folder, "*.csproj");
                    if (files == null || !files.Any())
                        if (Directory.GetParent(folder) == null)
                            throw new ArgumentException(paramName: "project", message: "Project file not found!");
                        else
                            return FindProject(Directory.GetParent(folder).FullName);
                    else if (files.Length > 1)
                        throw new ArgumentException(paramName: "project", message: "More than one project found!");
                    else
                        return files.Single();
                }



                UpdateProject(projectFile, outputFile);

                Console.WriteLine(outputFolder);
                Console.WriteLine(projectFile);

                return 0;
            });

            cmd.HelpOption("-? | -h | --help");
            cmd.Execute(args);

            if (System.Diagnostics.Debugger.IsAttached)
                Console.ReadLine();
        }


        static void UpdateProject(String projectFilename, String seedingScriptFilename)
        {
            if (Path.GetDirectoryName(seedingScriptFilename.ToLower()).StartsWith(Path.GetDirectoryName(projectFilename.ToLower())))
                seedingScriptFilename = seedingScriptFilename.Replace(Path.GetDirectoryName(projectFilename), string.Empty, StringComparison.InvariantCultureIgnoreCase);

            if (seedingScriptFilename.StartsWith(Path.DirectorySeparatorChar))
                seedingScriptFilename = seedingScriptFilename.Substring(1);

            XmlDocument doc = new XmlDocument();
            doc.Load(projectFilename);
            var node = doc.SelectSingleNode("/Project/ItemGroup[@Label=seeding]");
            if (node == null)
            {
                node = doc.CreateElement("ItemGroup");
                var attribute = doc.CreateAttribute("Label");
                attribute.Value = "seeding";
                node.Attributes.Append(attribute);
                doc.DocumentElement.AppendChild(node);
            }

            var noneElement = doc.CreateElement("None");
            var removeAttribute = doc.CreateAttribute("Remove");
            removeAttribute.Value = seedingScriptFilename;
            noneElement.Attributes.Append(removeAttribute);
            node.AppendChild(noneElement);

            var embeddedResourceElement = doc.CreateElement("EmbeddedResource");
            var includeAttribute = doc.CreateAttribute("Include");
            includeAttribute.Value = seedingScriptFilename;
            embeddedResourceElement.Attributes.Append(includeAttribute);
            node.AppendChild(embeddedResourceElement);


            doc.Save(@"C:\temp\project.csproj");

        }

    }
}

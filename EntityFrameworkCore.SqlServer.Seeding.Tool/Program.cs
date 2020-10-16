using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace EntityFrameworkCore.SqlServer.Seeding.Tool
{
    /// <summary>
    /// CLI tool entry point method
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var cmd = new CommandLineApplication();
            cmd.HelpOption("-? | -h | --help");

            cmd.Command("add", (cmd) =>
            {
                var argName = cmd.Argument("name", "Script name", false);
                var optName = cmd.Option("-n | --name <value>", "Script name", CommandOptionType.SingleValue);
                var optProject = cmd.Option("-p | --project <value>", "Project file path or project name", CommandOptionType.SingleValue);
                var optOutput = cmd.Option("-o | --output <value>", "Output folder path", CommandOptionType.SingleValue);
                cmd.HelpOption("-? | -h | --help");
                cmd.OnExecute(() =>
                {
                    String currentExecutionFolder = Directory.GetCurrentDirectory();
                    String projectFile = optProject.HasValue() ? optProject.Value() : null;
                    String outputFolder = currentExecutionFolder;
                    String name = optName.HasValue() ? optName.Value() : argName.Value;


                    // Check name param
                    if (String.IsNullOrWhiteSpace(name))
                        throw new ArgumentNullException(paramName: "name", message: "Argument name required!");

                    #region Seeding script file

                    // Calculate output path
                    if (!optOutput.HasValue())
                        outputFolder = currentExecutionFolder;
                    else if (!Path.IsPathRooted(optOutput.Value()))
                        outputFolder = Path.Combine(currentExecutionFolder, optOutput.Value());
                    else
                        outputFolder = optOutput.Value();

                    // Create output folder if missing
                    if (!Directory.Exists(outputFolder))
                        Directory.CreateDirectory(outputFolder);

                    // Calculate absolute output file path
                    var outputFilePath = Path.Combine(outputFolder, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{name}.sql");

                    // Create 
                    var outputContentTemplateStream = typeof(Program).Assembly.GetManifestResourceStream($"{typeof(Program).Namespace}.Template.sql");
                    using (StreamReader reader = new StreamReader(outputContentTemplateStream))
                    {
                        var contentText = reader.ReadToEnd();
                        File.WriteAllText(outputFilePath, contentText);
                    }

                    #endregion

                    #region Project file update
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

                    void UpdateProject(String projectFilename, String seedingScriptFilename)
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

                        doc.Save(projectFilename);
                    }


                    if (!optProject.HasValue())
                        projectFile = FindProject(currentExecutionFolder);

                    UpdateProject(projectFile, outputFilePath);
                    #endregion

                    Console.WriteLine($"Created seeding script file: {outputFilePath}");

                    return 0;
                });


            });
            cmd.Execute(args);

        }
    }
}

namespace Skyline.DataMiner.CICD.Parsers.DIS.Macro.VisualStudio
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.FileSystem;
    using Skyline.DataMiner.CICD.Loggers;
    using Skyline.DataMiner.CICD.Parsers.DIS.Macro.Xml;
    using Skyline.DataMiner.CICD.Parsers.Common.Exceptions;
    using Skyline.DataMiner.CICD.Parsers.Common.VisualStudio;

    /// <summary>
    /// Represents a DIS Macro solution.
    /// </summary>
    public class DisMacroSolution : Solution
    {
        private readonly ICollection<(Macro Script, SolutionFolder Folder)> _macros = new List<(Macro, SolutionFolder)>();
        private readonly IFileSystem _fileSystem = FileSystem.Instance;
        private readonly ILogCollector logCollector;

        private DisMacroSolution(string solutionPath, ILogCollector logCollector) : base(solutionPath)
        {
            if (!_fileSystem.File.Exists(solutionPath))
            {
                throw new System.IO.FileNotFoundException("Could not find solution file: " + solutionPath);
            }

            this.logCollector = logCollector;
            LoadMacros();
        }

        /// <summary>
        /// Gets the scripts of the DIS Macro solution.
        /// </summary>
        /// <value>The scripts of the DIS Macro solution.</value>
        public IEnumerable<(Macro Script, SolutionFolder Folder)> Macros => _macros;

        /// <summary>
        /// Loads the specified DIS Macro solution.
        /// </summary>
        /// <param name="solutionPath">The DIS Macro solution file path.</param>
        /// <param name="logCollector">Log collector.</param>
        /// <returns>The loaded DIS Macro solution.</returns>
        /// <exception cref="ParserException">Could not find 'Macros' folder in root of solution.</exception>
        public static DisMacroSolution Load(string solutionPath, ILogCollector logCollector = null)
        {
            return new DisMacroSolution(solutionPath, logCollector);
        }

        private void LoadMacros()
        {
            var macrosFolder = GetSubFolder("Macros");
            if (macrosFolder == null)
            {
                throw new ParserException("Could not find 'Macros' folder in root of solution.");
            }

            foreach (var folder in macrosFolder.GetDescendantFolders())
            {
                logCollector?.ReportStatus("Loading Macros from Descendant Folder: " + folder.Name);

                foreach (var file in folder.Files)
                {
                    if (!String.Equals(_fileSystem.Path.GetExtension(file.FileName), ".xml", StringComparison.OrdinalIgnoreCase)
                        || !Macro.IsMacroFile(file.AbsolutePath, logCollector))
                    {
                        continue;
                    }

                    try
                    {
                        var macro = Macro.Load(file.AbsolutePath);
                        _macros.Add((macro, folder));
                    }
                    catch (Exception e)
                    {
                        logCollector?.ReportError("Exception Loading Macros Checking File: " + e);
                    }
                }
            }
        }
    }
}

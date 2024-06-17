namespace Skyline.DataMiner.CICD.Parsers.DIS.Macro.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;

    using Skyline.DataMiner.CICD.FileSystem;
    using Skyline.DataMiner.CICD.Parsers.Common.Xml;
    using Skyline.DataMiner.CICD.Loggers;

    /// <summary>
    /// Represents a script of a DIS Macro solution.
    /// </summary>
    public class Macro
    {
        private readonly IList<string> _dllImports;
        private MacroCode _macroCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Macro"/> class.
        /// </summary>
        /// <param name="document">The Automation script document.</param>
        /// <exception cref="ArgumentNullException"><paramref name="document"/> is <see langword="null"/>.</exception>
        public Macro(XmlDocument document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            _dllImports = new List<string>();

            LoadMacro();
        }

        /// <summary>
        /// Gets the macro document.
        /// </summary>
        /// <value>The script document.</value>
        public XmlDocument Document { get; }

        /// <summary>
        /// Gets the description of the macro.
        /// </summary>
        /// <value>The name of the script.</value>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the author of the macro.
        /// </summary>
        public string Author { get; private set; }

        /// <summary>
        /// Gets the Code block of the DIS Macro.
        /// </summary>
        /// <vamie>The Exe blocks of the Automation script.</vamie>
        public MacroCode MacroCode => _macroCode;

        /// <summary>
        /// Gets the DllImports blocks of the DIS Macro.
        /// </summary>
        public IEnumerable<string> dllImports => _dllImports;

        /// <summary>
        /// Loads the specified Macro.
        /// </summary>
        /// <param name="path">The file path of the Macro.</param>
        /// <returns>The loaded script.</returns>
        /// <exception cref="System.IO.FileNotFoundException">The specified Macro was not found.</exception>
        public static Macro Load(string path)
        {
            if (!FileSystem.Instance.File.Exists(path))
            {
                throw new System.IO.FileNotFoundException("The Macro file '" + path + "' could not be found.", path);
            }

            var document = XmlDocument.Load(path);
            return new Macro(document);
        }

        /// <summary>
        /// Determines whether the specified file is an Macro.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="logCollector">The log collector.</param>
        /// <returns><c>true</c> if the specified file is an Macro; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException"><paramref name="path"/> is <see langword="null"/> or whitespace.</exception>
        public static bool IsMacroFile(string path, ILogCollector logCollector = null)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' cannot be null or whitespace", nameof(path));
            }

            try
            {
                var xmlContent = FileSystem.Instance.File.ReadAllText(path);
                var doc = XDocument.Parse(xmlContent);
                return String.Equals(doc.Root?.Name?.LocalName, "DisMacro", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception e)
            {
                logCollector?.ReportError("XDocument failed to Load with exception: " + e);
                return false;
            }
        }

        private void LoadMacro()
        {
            var disMacro = Document?.Element["DisMacro"];
            if (disMacro == null)
            {
                return;
            }

            Description = disMacro.Element["Description"]?.InnerText?.Trim();
            Author = disMacro.Element["Author"]?.InnerText?.Trim();

            var script = disMacro.Element["Script"];
            if (script == null)
            {
                return;
            }

            var code = script?.Element["Code"];
            if (code == null)
            {
                return;
            }

            _macroCode = new MacroCode(code);

            var dlls = script?.Element["DllImports"]?.Elements["DllImport"];
            if (dlls == null)
            {
                return;
            }

            foreach(var dll in dlls)
            {
                _dllImports.Add(dll?.InnerText?.Trim());
            }
        }
    }
}

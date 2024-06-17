namespace Skyline.DataMiner.CICD.Parsers.DIS.Macro.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.CICD.Parsers.Common.Xml;

    /// <summary>
    /// Represents a code block of a DIS Macro.
    /// </summary>
    public class MacroCode
    {
        private IList<string> _codeLines = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MacroCode"/> class.
        /// </summary>
        /// <param name="node">The node that represents the Code block.</param>
        public MacroCode(XmlElement node)
        {
            Node = node;
            ParseValue(node);
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; private set; }

        /// <summary>
        /// Gets the code lines.
        /// </summary>
        /// <value>The code lines.</value>
        public IEnumerable<string> CodeLines => _codeLines;

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <value>The node.</value>
        public XmlElement Node { get; }

        /// <summary>
        /// Gets the node CDATA.
        /// </summary>
        /// <value>The node CDATA.</value>
        public XmlCDATA NodeCDATA { get; private set; }

        private void ParseValue(XmlElement node)
        {
            var valueNode = node;
            if (valueNode == null)
            {
                return;
            }

            NodeCDATA = valueNode
                        .Children.OfType<XmlCDATA>()
                        .FirstOrDefault();

            Code = NodeCDATA != null ? NodeCDATA.InnerText : valueNode.InnerText;
            _codeLines = Code.Split(new[] { "\r\n" }, StringSplitOptions.None);
            for (int i = 0; i < _codeLines.Count; i++)
            {
                _codeLines[i] = _codeLines[i].Replace("\t", "    ");
            }
        }
    }
}

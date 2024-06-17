namespace Skyline.DataMiner.CICD.Parsers.DIS.Macro.VisualStudio.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.CICD.Parsers.DIS.Macro.VisualStudio;

    [TestClass]
	public class DisMacroSolutionTests
	{
        [TestMethod]
        public void DISMacroCompiler_Solution1_Load()
        {
            var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dir = Path.GetFullPath(Path.Combine(baseDir, @"VisualStudio\TestFiles\Solution1"));
            var path = Path.Combine(dir, "DisMacro.sln");

            var solution = DisMacroSolution.Load(path);

            Assert.IsInstanceOfType(solution, typeof(DisMacroSolution));

            Assert.AreEqual(path, solution.SolutionPath);
            Assert.AreEqual(Path.GetDirectoryName(path), solution.SolutionDirectory);

            Assert.AreEqual(3, solution.Projects.Count());
            Assert.AreEqual(2, solution.Macros.Count());

            var script1 = solution.Macros.FirstOrDefault(s => s.Script.Description == "Macro_1").Script;
            Assert.IsNotNull(script1);
            Assert.IsNull(script1.MacroCode);

            var script2 = solution.Macros.FirstOrDefault(s => s.Script.Description == "Macro_2").Script;
            Assert.IsNotNull(script2);
            Assert.IsNotNull(script2.MacroCode);
            Assert.AreEqual("[Project:Script_2]", script2.MacroCode.Code);
        }
    }
}
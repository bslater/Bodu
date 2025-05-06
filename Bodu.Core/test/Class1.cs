using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu
{
	[TestClass]
	public class NetStandardTests
	{
#if NETSTANDARD2_0

    /// <summary>
    /// Verifies that this test only runs in .NET Standard 2.0 target builds.
    /// </summary>
    [TestMethod]
    public void NetStandard20_ConditionalCode_ShouldCompileAndRun()
    {
        var message = "Running under NETSTANDARD2_0";
        Console.WriteLine(message);
        Assert.AreEqual("Running under NETSTANDARD2_0", message);
    }
#else

		[TestMethod]
		public void NetStandard20_ConditionalCode_ShouldBeExcluded()
		{
			Assert.Inconclusive("This test does not run under NETSTANDARD2_0 target.");
		}

#endif
	}
}
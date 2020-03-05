using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MAU.Test
{
	[TestClass]
	public class MauTest
	{
		[TestMethod]
		public async Task ConnectTest()
		{
			using var ws = new MyAngularUi(3000);
			bool serverState = await ws.Start();
			Assert.AreEqual(true, serverState);
			ws.Wait();
		}
	}
}

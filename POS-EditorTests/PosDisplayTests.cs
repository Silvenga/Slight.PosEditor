using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace POS_Editor.Tests {

    [TestClass]
    public class PosDisplayTests {

        [TestMethod]
        public void TryParseTest() {

            PosDisplay display;
            bool result;

            {
                result = PosDisplay.TryParse("", out display);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("Hello", out display);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHello", out display);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHello111", out display);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("Hello Hello Hello Hello Hello Hello", out display);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello", out display);
                Assert.IsFalse(result);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHello", out display);
                Assert.IsFalse(result);
            }
        }
    }
}

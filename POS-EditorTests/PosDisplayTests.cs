using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace POS_Editor.Tests {

    [TestClass]
    public class PosDisplayTests {

        [TestMethod]
        public void TryParseTest() {

            PosDisplay display;
            bool result;

            {
                result = PosDisplay.TryParse("", out display, 20, 4);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("Hello", out display, 20, 4);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHello", out display, 20, 4);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHello111", out display, 20, 4);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("Hello Hello Hello Hello Hello Hello", out display, 20, 4);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello", out display, 20, 4);
                Assert.IsFalse(result);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHello", out display, 20, 4);
                Assert.IsFalse(result);
            }
        }
    }
}

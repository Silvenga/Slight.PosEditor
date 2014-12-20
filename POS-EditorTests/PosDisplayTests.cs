using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace POS_Editor.Tests {

    [TestClass]
    public class PosDisplayTests {

        [TestMethod]
        public void TryParseTest() {

            PosDisplay display;
            bool result;

            {
                result = PosDisplay.TryParse("", out display, 4, 20);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("Hello", out display, 4, 20);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHello", out display, 4, 20);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHello111", out display, 4, 20);
                Assert.IsTrue(result);
            }
            {
                result = PosDisplay.TryParse("Hello Hello Hello Hello Hello Hello", out display, 4, 20);
                Assert.IsTrue(result);
            }
            {
                result =
                    PosDisplay.TryParse(
                        "Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello",
                        out display,
                        4,
                        20);
                Assert.IsFalse(result);
            }
            {
                result =
                    PosDisplay.TryParse(
                        "HelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHello",
                        out display,
                        4,
                        20);
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void SendTest() {

            var manager = new PosManager("COM3");
            PosDisplay display;
            bool result;

            {
                result = PosDisplay.TryParse("", out display, 4, 20);
                manager.Send(display);
            }
            {
                result = PosDisplay.TryParse("Hello", out display, 4, 20);
                manager.Send(display);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHello", out display, 4, 20);
                manager.Send(display);
            }
            {
                result = PosDisplay.TryParse("HelloHelloHelloHello111", out display, 4, 20);
                manager.Send(display);
            }
            {
                result = PosDisplay.TryParse("Hello Hello Hello Hello Hello Hello", out display, 4, 20);
                manager.Send(display);
            }
            {
                result =
                    PosDisplay.TryParse(
                        "Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello Hello",
                        out display,
                        4,
                        20);
                Assert.IsFalse(result);
            }
            {
                result =
                    PosDisplay.TryParse(
                        "HelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHelloHello",
                        out display,
                        4,
                        20);
                Assert.IsFalse(result);
            }
        }

    }

}
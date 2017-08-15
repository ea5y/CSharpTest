using NUnit.Framework;
using Should;

namespace Test.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Should_be_true()
        {
            true.ShouldBeTrue();
        }
    }
}

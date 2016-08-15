using cqrs.Messaging;
using Xunit;

namespace cqrs_test
{
    public class Class1
    {
        [Fact]
        public void Go()
        {
            int i = 5;
            var e = new Envelope<int>(i);

            Assert.Equal(e.Body, i);

        }
    }
}

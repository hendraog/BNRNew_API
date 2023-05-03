using Castle.Core.Logging;
using Moq;
using my_api.Controllers;

namespace my_api_test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Mock<ILogger> l = new Mock<ILogger>();

            Mock<idao> m = new Mock<idao>();
            m.Setup(x => x.generateRadom()).Returns("dfd");

            var s = new WeatherForecastController(m.Object);

            var res = s.Get();
            

            Assert.True(res !=null);
        }
    }
}
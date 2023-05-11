using Castle.Core.Logging;
using Moq;
using my_api.Controllers;

namespace my_api.tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Mock<ILogger> l = new Mock<ILogger>();


        Idao d = new Dao();
        d.generateRadom();


        Mock<Idao> m = new Mock<Idao>();
        m.Setup(x => x.generateRadom()).Returns("dfd");

        var s = new WeatherForecastController(m.Object);

        var res = s.Get();


        Assert.True(res != null);
    }
}
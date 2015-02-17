using System;
using Xunit;
using WeatherApp;

namespace WeatherAppTestProject
{
    public class TestClass
    {

        [Fact]
        public void MyTest()
        {
            Assert.Equal(4, 2 + 2);
        }

        [Fact]
        public void ZipCodeForCharlottesvilleReturnsCorrectCoordinates()
        {
            var sut = new CoordinateRequest();
            sut.GetCoordinates();
            Assert.Equal(sut.coordinateValues, "lat=38.0374&lon=-78.4857");
        }
    }
}

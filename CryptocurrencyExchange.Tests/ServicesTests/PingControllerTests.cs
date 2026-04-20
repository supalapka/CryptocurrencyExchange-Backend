using CryptocurrencyExchange.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class PingControllerTests
    {
        [Test]
        public void Get_ReturnsPongPlainText()
        {
            var controller = new PingController();

            var result = controller.Get();

            var contentResult = (ContentResult)result;
            Assert.That(contentResult.Content, Is.EqualTo("PONG"));
            Assert.That(contentResult.ContentType, Is.EqualTo("text/plain"));
        }
    }
}

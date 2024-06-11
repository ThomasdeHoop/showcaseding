using Xunit;
using Moq;
using authenticatie.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace TestLibrary
{
    public class Tests
    {
        public interface IClientContract
        {
            void broadcastMessage(string name, string message);
        }

        [Fact]
        public async Task HubsAreMockableViaType()
        {
            // Arrange
            var hub = new ChatHub();
            var mockClients = new Mock<IHubCallerClients>();
            var all = new Mock<IClientProxy>();
            hub.Clients = mockClients.Object;
            all.Setup(m => m.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default)).Verifiable();
            mockClients.Setup(m => m.All).Returns(all.Object);

            // Act
            await hub.SendMessage("TestUser", "TestMessage");

            // Assert
            all.VerifyAll();
        }

        [Fact]
        public void HubsAreMockableViaDynamic()
        {
            // Arrange
            bool sendCalled = false;
            var hub = new ChatHub();
            var mockClients = new Mock<IHubCallerClients>();
            hub.Clients = mockClients.Object;

            var all = new Mock<IClientProxy>();
            all.Setup(m => m.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default)).Callback(() => sendCalled = true).Returns(Task.CompletedTask);

            mockClients.Setup(m => m.All).Returns(all.Object);

            // Act
            hub.SendMessage("TestUser", "TestMessage");

            // Assert
            Assert.True(sendCalled);
        }
    }
}

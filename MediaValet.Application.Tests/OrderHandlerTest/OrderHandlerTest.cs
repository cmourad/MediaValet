using System.Threading.Tasks;
using FluentAssertions;
using MediaValet.Application.Handlers.OrdersHandler;
using MediaValet.Application.Repositories;
using MediaValet.Infrastructure.Cloud.Queue;
using MediaValet.Module;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MediaValet.Application.Tests.OrderHandlerTest
{
  public class OrderHandlerTest
  {
    private readonly Mock<ILogger<OrderHandler>> _loggerMock;
    private readonly Mock<IOrdersRepository> _repositoryMock;
    private readonly Mock<IMessagingService> _messagingMock;


    public OrderHandlerTest()
    {
      _loggerMock = new Mock<ILogger<OrderHandler>>();
      _repositoryMock = new Mock<IOrdersRepository>();
      _messagingMock = new Mock<IMessagingService>();
    }

    [Fact] public async Task GivenANewOrder_shouldEnqueueIt()
    {
      var expectedResult = "Send order 0 with random number 2";
      var newOrder = new Order
      {
        OrderId = 0,
        OrderText = "test",
        RandomNumber = 2
      };

      _repositoryMock.Setup(e => e.GetOrderId()).Returns(0);
      _repositoryMock.Setup(e => e.GetRandomId()).Returns(2);
      _messagingMock.Setup(e => e.Enqueue(newOrder.ToString())).ReturnsAsync(true);

      var request = new OrderCommand("test");
      var handler = new OrderHandler(_repositoryMock.Object, _messagingMock.Object, _loggerMock.Object);
      var result = await handler.Handle(request, default);

      result.Should().Be(expectedResult);


      _messagingMock.Verify(e=>e.Enqueue(It.Is<string>(s=>s.Contains("test"))));
      _repositoryMock.VerifyAll();
      _messagingMock.VerifyAll();

    }
  }
}

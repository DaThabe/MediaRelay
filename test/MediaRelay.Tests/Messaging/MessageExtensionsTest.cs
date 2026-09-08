using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaRelay.Messaging;


[TestClass]
public sealed class MessageExtensionsTest
{
    [TestMethod]
    public async Task AddMessageHandlers_Should_Register_Sender_And_Receivers()
    {
        //var receiverMock = new Mock<IMessageReceiver<string>>();
        //receiverMock
        //    .Setup(x => x.OnReceivedAsync(
        //       It.IsAny<string>(),
        //       It.IsAny<Func<ValueTask>?>(),
        //       It.IsAny<CancellationToken>()))
        //    .Returns(ValueTask.CompletedTask);


        //var descriptors = new ServiceCollection();
        //descriptors.AddSingleton(typeof(IMessageSender<>), typeof(MessageSender<>));
        //descriptors.AddSingleton<IMessageOrchestrator, MessageOrchestrator>();

        //descriptors.AddMessageReceiver(receiverMock.Object);
        //descriptors.AddLogging(x => x.AddConsole());

        //var services = descriptors.BuildServiceProvider();


        //var stringSender = services.GetService<IMessageSender<string>>();
        //Assert.IsNotNull(stringSender);

        //var numberSender = services.GetService<IMessageSender<double>>();
        //Assert.IsNotNull(numberSender);

        //var timeSender = services.GetService<IMessageSender<DateTime>>();
        //Assert.IsNotNull(timeSender);


        //var orchestrator = services.GetRequiredService<IMessageOrchestrator>();
        //await orchestrator.SendAsnc("strinMessage", TestContext.CancellationToken);
    }


    public TestContext TestContext { get; set; }
}

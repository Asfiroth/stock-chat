using Mediator;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockChat.Api.Controllers;
using StockChat.Api.Handlers.Queries;
using StockChat.Api.Models;

namespace StockChat.Tests;

public class ChatInputControllerTest
{
    // tests for ChatInputController
    
    // tests for Get
    [Fact]
    public async Task GivenAValidChatGroupId_WhenICallTheGet_ThenItReturnsOkWithResponse()
    {
        //Arrange
        var theMediator = new Mock<IMediator>();
        
        theMediator
            .Setup(mediator => mediator.Send(It.IsAny<GetChatGroupByMembersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Func<string>(() => "id"));

        theMediator.Setup(mediator =>
                mediator.Send(It.IsAny<GetChatMessagesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChatMessage>());


        var sut = new ChatInputController(theMediator.Object);

        //Act
        var contentResult = await sut.Get("id");

        //Assert
        Assert.NotNull(contentResult);
        Assert.IsAssignableFrom<OkObjectResult>(contentResult);

        var result = ((OkObjectResult)contentResult).Value;
        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<ChatMessage>>(result);
    }
    
    [Fact]
    public async Task GivenAInvalidChatGroupId_WhenICallTheGet_ThenItReturnsA500Status()
    {
        //Arrange
        var theMediator = new Mock<IMediator>();
        
        theMediator
            .Setup(mediator => mediator.Send(It.IsAny<GetChatGroupByMembersQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        theMediator.Setup(mediator =>
                mediator.Send(It.IsAny<GetChatMessagesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ChatMessage>());
        
        var sut = new ChatInputController(theMediator.Object);
        
        //Act
        var result = await sut.Get("id");
        
        //Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }
        
    // tests for GetLobby
    [Fact]
    public async Task GivenAValidCall_WhenICallTheGetLobby_ThenItReturnsOkWithResponse()
    {
        //Arrange
        var theMediator = new Mock<IMediator>();
        
        theMediator
            .Setup(mediator => mediator.Send(It.IsAny<GetPeopleAtLobbyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserConnectedMessage>());

        var sut = new ChatInputController(theMediator.Object);

        //Act
        var contentResult = await sut.GetLobby();

        //Assert
        Assert.NotNull(contentResult);
        Assert.IsAssignableFrom<OkObjectResult>(contentResult);

        var result = ((OkObjectResult)contentResult).Value;
        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<UserConnectedMessage>>(result);
    }
    
    [Fact]
    public async Task GivenAInvalidCall_WhenICallTheGetLobby_ThenItReturnsA500Status()
    {
        //Arrange
        var theMediator = new Mock<IMediator>();
        
        theMediator
            .Setup(mediator => mediator.Send(It.IsAny<GetPeopleAtLobbyQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var sut = new ChatInputController(theMediator.Object);
        
        //Act
        var result = await sut.GetLobby();
        
        //Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }
}
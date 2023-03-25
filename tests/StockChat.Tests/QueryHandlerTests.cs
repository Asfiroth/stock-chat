using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Moq;
using StockChat.Api.Data;
using StockChat.Api.Handlers.Queries;
using StockChat.Api.Models;

namespace StockChat.Tests;

public class QueryHandlerTests
{
    // tests for GetChatGroupByMembersQuery
    
    [Fact]
    public async Task GivenAValidChatGroupId_WhenICallGetCharGroupByMembersHandle_ThenItReturnsTheChatGroupId()
    {
        //Arrange
        var query = new GetChatGroupByMembersQuery
        {
            FakeGroupId = "id1|id2"
        };
        
        var sut = new GetCharGroupByMembers();
        
        //Act
        var result =await sut.Handle(query, CancellationToken.None);

        //Assert
        Assert.Equal("id1-id2", result);
    }
    
    [Fact]
    public async Task GivenAInvalidChatGroupId_WhenICallGetCharGroupByMembersHandle_ThenItThrowsAnException()
    {
        //Arrange
        var query = new GetChatGroupByMembersQuery
        {
            FakeGroupId = "invalid-id"
        };
        
        var sut = new GetCharGroupByMembers();
        
        //Act
        
        //Assert
        await Assert.ThrowsAsync<Exception>(async () => await sut.Handle(query, CancellationToken.None));
    }
    
    [Fact]
    public async Task GivenAInvalidChatGroupId_WhenICallGetCharGroupByMembersHandle_ThenItThrowsAnArgumentNullException()
    {
        //Arrange
        var query = new GetChatGroupByMembersQuery
        {
            FakeGroupId = null
        };
        
        var sut = new GetCharGroupByMembers();
        
        //Act
        
        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Handle(query, CancellationToken.None));
    }
    
    [Fact]
    public async Task GivenAInvalidQuery_WhenICallGetCharGroupByMembersHandle_ThenItThrowsAnArgumentNullException()
    {
        //Arrange
        GetChatGroupByMembersQuery query = null;
        
        var sut = new GetCharGroupByMembers();
        
        //Act
        
        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Handle(query, CancellationToken.None));
    }
    
    // tests for GetChatMessagesQuery
    
    [Fact]
    public async Task GivenAValidChatGroupId_WhenICallGetChatMessagesHandler_ThenItReturnsTheChatMessages()
    {
        //Arrange
        var repositoryMock = new Mock<IRepository<ChatMessage>>();
        repositoryMock.Setup(repository => repository.GetFiltered(It.IsAny<Expression<Func<ChatMessage, bool>>>()))
            .ReturnsAsync(new List<ChatMessage>());
        
        var loggerMock = new Mock<ILogger<GetChatMessages>>();
        loggerMock.Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

        var query = new GetChatMessagesQuery
        {
            ChatGroupId = "id1-id2"
        };
        
        var sut = new GetChatMessages(repositoryMock.Object, loggerMock.Object);
        
        //Act
        var result = await sut.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<ChatMessage>>(result);
    }

    [Fact]
    public async Task GiveAValidChatGroupId_WhenICallGetChatMessagesHandler_ThenItReturnsThe50FirstMessagesOrderedBySentTime()
    {
        //Arrange
        var fakeList = new List<ChatMessage>(); 
        
        for (var i = 0; i < 100; i++)
        {
            fakeList.Add(new ChatMessage
            {
                ChatGroupId = "id1-id2",
                SentTime = DateTime.Now.AddSeconds(i)
            });
        }
        
        
        var repositoryMock = new Mock<IRepository<ChatMessage>>();
        repositoryMock.Setup(repository => repository.GetFiltered(It.IsAny<Expression<Func<ChatMessage, bool>>>()))
            .ReturnsAsync(fakeList);
        
        var loggerMock = new Mock<ILogger<GetChatMessages>>();
        loggerMock.Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

        var query = new GetChatMessagesQuery
        {
            ChatGroupId = "id1-id2"
        };
        
        var sut = new GetChatMessages(repositoryMock.Object, loggerMock.Object);
        
        //Act
        var result = await sut.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<ChatMessage>>(result);
        Assert.Equal(50, result.Count);
    }
    
    [Fact]
    public async Task GivenAInvalidChatGroupId_WhenICallGetChatMessagesHandler_ThenItThrowsAnArgumentNullException()
    {
        //Arrange
        var repositoryMock = new Mock<IRepository<ChatMessage>>();
        repositoryMock.Setup(repository => repository.GetFiltered(It.IsAny<Expression<Func<ChatMessage, bool>>>()))
            .ReturnsAsync(new List<ChatMessage>());
        
        var loggerMock = new Mock<ILogger<GetChatMessages>>();
        loggerMock.Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

        var query = new GetChatMessagesQuery
        {
            ChatGroupId = null
        };
        
        var sut = new GetChatMessages(repositoryMock.Object, loggerMock.Object);
        
        //Act
        
        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Handle(query, CancellationToken.None));
    }
    
    [Fact]
    public async Task GivenAInvalidQuery_WhenICallGetChatMessagesHandler_ThenItThrowsAnArgumentNullException()
    {
        //Arrange
        var repositoryMock = new Mock<IRepository<ChatMessage>>();
        repositoryMock.Setup(repository => repository.GetFiltered(It.IsAny<Expression<Func<ChatMessage, bool>>>()))
            .ReturnsAsync(new List<ChatMessage>());
        
        var loggerMock = new Mock<ILogger<GetChatMessages>>();
        loggerMock.Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

        GetChatMessagesQuery query = null;
        
        var sut = new GetChatMessages(repositoryMock.Object, loggerMock.Object);
        
        //Act
        
        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Handle(query, CancellationToken.None));
    }

    // tests for GetPeopleAtLobbyQuery
    
    [Fact]
    public async Task GivenAValidLobbyInstance_WhenICallGetPeopleAtLobbyHandler_ThenItReturnsThePeopleAtLobby()
    {
        //Arrange
        var repositoryMock = new Mock<IRepository<UserConnectedMessage>>();
        repositoryMock.Setup(repository => repository.GetAll())
            .ReturnsAsync(new List<UserConnectedMessage>());
        
        var loggerMock = new Mock<ILogger<GetPeopleAtLobby>>();
        loggerMock.Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

        var query = GetPeopleAtLobbyQuery.Instance;
        
        var sut = new GetPeopleAtLobby(repositoryMock.Object, loggerMock.Object);
        
        //Act
        var result = await sut.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<List<UserConnectedMessage>>(result);
    }
    
    [Fact]
    public async Task GivenAnInValidLobbyInstance_WhenICallGetPeopleAtLobbyHandler_ThenItThrowsAnArgumentNullException()
    {
        //Arrange
        var repositoryMock = new Mock<IRepository<UserConnectedMessage>>();
        repositoryMock.Setup(repository => repository.GetAll())
            .ReturnsAsync(new List<UserConnectedMessage>());
        
        var loggerMock = new Mock<ILogger<GetPeopleAtLobby>>();
        loggerMock.Setup(logger => logger.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

        GetPeopleAtLobbyQuery query = null;
        
        var sut = new GetPeopleAtLobby(repositoryMock.Object, loggerMock.Object);
        
        //Act

        //Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.Handle(query, CancellationToken.None));
    }
}
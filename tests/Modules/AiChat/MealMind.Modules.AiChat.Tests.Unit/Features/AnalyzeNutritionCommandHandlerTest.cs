using MealMind.Modules.AiChat.Application.Abstractions.Database;
using MealMind.Modules.AiChat.Application.Abstractions.Services;
using MealMind.Modules.AiChat.Application.Features.Commands.AnalyzeNutritionCommand;
using MealMind.Modules.AiChat.Domain.AiChatUser;
using MealMind.Modules.AiChat.Domain.Conversation;
using MealMind.Modules.AiChat.Domain.Rag;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;
using MealMind.Shared.Abstractions.Services;
using MealMind.Shared.Contracts.Dto.AiChat;
using MealMind.Shared.Contracts.Result;
using Microsoft.SemanticKernel;
using Moq;
using Pgvector;

namespace MealMind.Modules.AiChat.Tests.Unit.Features;

public class AnalyzeNutritionCommandHandlerTest
{
    private readonly Mock<INutritionSummaryService> _nutritionSummaryServiceMock;
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly Mock<IDocumentRepository> _documentRepositoryMock;
    private readonly Mock<IAiChatUserRepository> _aiChatUserRepositoryMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IEmbeddingService> _embeddingServiceMock;
    private readonly Mock<IAiChatService> _aiChatServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AnalyzeNutritionCommand.Handler _handler;

    public AnalyzeNutritionCommandHandlerTest()
    {
        _nutritionSummaryServiceMock = new Mock<INutritionSummaryService>();
        _conversationRepositoryMock = new Mock<IConversationRepository>();
        _documentRepositoryMock = new Mock<IDocumentRepository>();
        _aiChatUserRepositoryMock = new Mock<IAiChatUserRepository>();
        _userServiceMock = new Mock<IUserService>();
        _embeddingServiceMock = new Mock<IEmbeddingService>();
        _aiChatServiceMock = new Mock<IAiChatService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AnalyzeNutritionCommand.Handler(
            _nutritionSummaryServiceMock.Object,
            _conversationRepositoryMock.Object,
            _documentRepositoryMock.Object,
            _aiChatUserRepositoryMock.Object,
            _userServiceMock.Object,
            _embeddingServiceMock.Object,
            _aiChatServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    [Test]
    public async Task Handle_WhenAiChatUserNotFound_ShouldReturnNotFound()
    {
        var userId = UserId.New();
        _userServiceMock.Setup(x => x.UserId).Returns(userId);
        _aiChatUserRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AiChatUser)null!);

        var command = new AnalyzeNutritionCommand(Guid.NewGuid(), "What should I eat?");

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.StatusCode).IsEqualTo(404);
        await Assert.That(result.Message).IsEqualTo("AI Chat user not found.");
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenConversationNotFound_ShouldReturnNotFound()
    {
        var userId = UserId.New();
        var aiUser = AiChatUser.Create(userId);
        var conversationId = Guid.NewGuid();

        _userServiceMock.Setup(x => x.UserId).Returns(userId);
        _aiChatUserRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiUser);
        _conversationRepositoryMock
            .Setup(x => x.GetByIdAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation)null!);

        var command = new AnalyzeNutritionCommand(conversationId, "What should I eat?");

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.StatusCode).IsEqualTo(404);
        await Assert.That(result.Message).IsEqualTo("Conversation not found.");
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenDailyPromptsLimitExceeded_ShouldReturnBadRequest()
    {
        var userId = UserId.New();
        var aiUser = AiChatUser.Create(userId);
        // Free tier has DailyPromptsLimit = 10; simulate count at limit
        var conversation = Conversation.Create(userId);

        _userServiceMock.Setup(x => x.UserId).Returns(userId);
        _aiChatUserRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiUser);
        _conversationRepositoryMock
            .Setup(x => x.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepositoryMock
            .Setup(x => x.GetUserDailyConversationPromptsCountAsync(aiUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10); // equals DailyPromptsLimit (10) for Free tier

        var command = new AnalyzeNutritionCommand(conversation.Id, "What should I eat?");

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.StatusCode).IsEqualTo(400);
        await Assert.That(result.Message).IsEqualTo("Daily prompts limit exceeded.");
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenPromptExceedsTokenLimit_ShouldReturnBadRequest()
    {
        var userId = UserId.New();
        var aiUser = AiChatUser.Create(userId); // PromptTokensLimit = 200 on Free tier
        var conversation = Conversation.Create(userId);

        // Build a prompt that exceeds 200 tokens (each word is roughly 1 token)
        var longPrompt = string.Join(" ", Enumerable.Repeat("nutrition", 300));

        _userServiceMock.Setup(x => x.UserId).Returns(userId);
        _aiChatUserRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiUser);
        _conversationRepositoryMock
            .Setup(x => x.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepositoryMock
            .Setup(x => x.GetUserDailyConversationPromptsCountAsync(aiUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var command = new AnalyzeNutritionCommand(conversation.Id, longPrompt);

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.StatusCode).IsEqualTo(400);
        await Assert.That(result.Message).IsEqualTo("Prompt exceeds the token limit.");
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenNoRelevantDocumentsFound_ShouldReturnBadRequest()
    {
        var userId = UserId.New();
        var aiUser = AiChatUser.Create(userId);
        var conversation = BuildConversationWithAssistantMessage(userId);
        var embedding = new Vector(new float[768]);

        _userServiceMock.Setup(x => x.UserId).Returns(userId);
        _aiChatUserRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiUser);
        _conversationRepositoryMock
            .Setup(x => x.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepositoryMock
            .Setup(x => x.GetUserDailyConversationPromptsCountAsync(aiUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _nutritionSummaryServiceMock
            .Setup(x => x.BuildSummaryAsync(aiUser.Id, It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("summary");
        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(embedding);
        _documentRepositoryMock
            .Setup(x => x.GetRelevantDocumentsAsync(It.IsAny<IEnumerable<float>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RagDocument>());

        var command = new AnalyzeNutritionCommand(conversation.Id, "Hi");

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsFalse();
        await Assert.That(result.StatusCode).IsEqualTo(400);
        await Assert.That(result.Message).IsEqualTo("No relevant documents found.");
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_HappyPath_ShouldCallAiServiceWithNutritionSummaryAndCommitAndReturnOk()
    {
        var userId = UserId.New();
        var aiUser = AiChatUser.Create(userId);
        var conversation = BuildConversationWithAssistantMessage(userId);
        var embedding = new Vector(new float[768]);
        var ragDoc = RagDocument.Create("title", "some content", embedding, 0, Guid.NewGuid());
        var aiResponse = new StructuredResponse
        {
            Title = "Analysis",
            Paragraphs = ["You eat well."],
            KeyPoints = ["Good protein intake."]
        };
        const string expectedSummary = "week 1 summary";
        const int requestedWeeks = 2;
        const string prompt = "Analyze my nutrition";

        _userServiceMock.Setup(x => x.UserId).Returns(userId);
        _aiChatUserRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiUser);
        _conversationRepositoryMock
            .Setup(x => x.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepositoryMock
            .Setup(x => x.GetUserDailyConversationPromptsCountAsync(aiUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _nutritionSummaryServiceMock
            .Setup(x => x.BuildSummaryAsync(aiUser.Id, requestedWeeks, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSummary);
        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(prompt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(embedding);
        _documentRepositoryMock
            .Setup(x => x.GetRelevantDocumentsAsync(It.IsAny<IEnumerable<float>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RagDocument> { ragDoc });
        _aiChatServiceMock
            .Setup(x => x.GenerateStructuredResponseWithNutritionSummaryAsync(
                prompt,
                It.IsAny<string>(),
                expectedSummary,
                It.IsAny<List<ChatMessageContent>>(),
                aiUser.ResponseTokensLimit,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResponse);
        _unitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var command = new AnalyzeNutritionCommand(conversation.Id, prompt, requestedWeeks);

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.StatusCode).IsEqualTo(200);
        await Assert.That(result.Value).IsNotNull();
        await Assert.That(result.Value!.ConversationId).IsEqualTo(conversation.Id.Value);

        _nutritionSummaryServiceMock.Verify(
            x => x.BuildSummaryAsync(aiUser.Id, requestedWeeks, It.IsAny<CancellationToken>()),
            Times.Once);
        _aiChatServiceMock.Verify(
            x => x.GenerateStructuredResponseWithNutritionSummaryAsync(
                prompt,
                It.IsAny<string>(),
                expectedSummary,
                It.IsAny<List<ChatMessageContent>>(),
                aiUser.ResponseTokensLimit,
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WhenDailyPromptsLimitIsMinusOne_ShouldNotEnforceDailyLimit()
    {
        // Premium tier has DailyPromptsLimit = -1, meaning unlimited
        var userId = UserId.New();
        var aiUser = AiChatUser.Create(userId);
        aiUser.ChangeTier(MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums.SubscriptionTier.Premium);
        var conversation = BuildConversationWithAssistantMessage(userId);
        var embedding = new Vector(new float[768]);
        var ragDoc = RagDocument.Create("title", "content", embedding, 0, Guid.NewGuid());
        var aiResponse = new StructuredResponse { Title = "Result" };

        _userServiceMock.Setup(x => x.UserId).Returns(userId);
        _aiChatUserRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiUser);
        _conversationRepositoryMock
            .Setup(x => x.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _conversationRepositoryMock
            .Setup(x => x.GetUserDailyConversationPromptsCountAsync(aiUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(9999); // would trip the limit if DailyPromptsLimit != -1
        _nutritionSummaryServiceMock
            .Setup(x => x.BuildSummaryAsync(aiUser.Id, It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("summary");
        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(embedding);
        _documentRepositoryMock
            .Setup(x => x.GetRelevantDocumentsAsync(It.IsAny<IEnumerable<float>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RagDocument> { ragDoc });
        _aiChatServiceMock
            .Setup(x => x.GenerateStructuredResponseWithNutritionSummaryAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<List<ChatMessageContent>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(aiResponse);
        _unitOfWorkMock
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        var command = new AnalyzeNutritionCommand(conversation.Id, "Hi");

        var result = await _handler.Handle(command, CancellationToken.None);

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.StatusCode).IsEqualTo(200);
    }

    // Builds a Conversation that already has one Assistant message so that
    // GetRecentMessage() — called by the handler when creating the User message — succeeds.
    private static Conversation BuildConversationWithAssistantMessage(UserId userId)
    {
        var conversation = Conversation.Create(userId);
        var seedMessage = AiChatMessage.Create(
            conversation.Id,
            AiChatRole.Assistant,
            "Hello! How can I help you?",
            Guid.Empty);
        conversation.AddMessage(seedMessage);
        return conversation;
    }
}

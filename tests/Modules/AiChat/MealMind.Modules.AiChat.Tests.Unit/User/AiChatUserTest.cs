using MealMind.Modules.AiChat.Domain.AiChatUser;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Enums;
using MealMind.Shared.Abstractions.Kernel.ValueObjects.Ids;

namespace MealMind.Modules.AiChat.Tests.Unit.User;

public class AiChatUserTest
{
    [Test]
    public async Task ChangeTier_ShouldUpdateToFreeTier()
    {
        var user = AiChatUser.Create(UserId.New());

        user.ChangeTier(SubscriptionTier.Free);

        await Assert.That(user.Tier).IsEqualTo(SubscriptionTier.Free);
        await Assert.That(user.ConversationsLimit).IsEqualTo(2);
        await Assert.That(user.ConversationsMessagesHistoryDaysLimit).IsEqualTo(7);
        await Assert.That(user.DocumentsLimit).IsEqualTo(1);
        await Assert.That(user.PromptTokensLimit).IsEqualTo(200);
        await Assert.That(user.ResponseTokensLimit).IsEqualTo(200);
        await Assert.That(user.DailyPromptsLimit).IsEqualTo(10);
        await Assert.That(user.CanExportData).IsFalse();
        await Assert.That(user.CanUseAdvancedPrompts).IsFalse();
        await Assert.That(user.DailyImageAnalysisLimit).IsEqualTo(0);
        await Assert.That(user.ImageAnalysisCorrectionPromptLimit).IsEqualTo(0);
    }

    [Test]
    public async Task ChangeTier_ShouldUpdateToStandardTier()
    {
        var user = AiChatUser.Create(UserId.New());

        user.ChangeTier(SubscriptionTier.Standard);

        await Assert.That(user.Tier).IsEqualTo(SubscriptionTier.Standard);
        await Assert.That(user.ConversationsLimit).IsEqualTo(5);
        await Assert.That(user.ConversationsMessagesHistoryDaysLimit).IsEqualTo(30);
        await Assert.That(user.DocumentsLimit).IsEqualTo(5);
        await Assert.That(user.PromptTokensLimit).IsEqualTo(500);
        await Assert.That(user.ResponseTokensLimit).IsEqualTo(500);
        await Assert.That(user.DailyPromptsLimit).IsEqualTo(50);
        await Assert.That(user.CanExportData).IsTrue();
        await Assert.That(user.CanUseAdvancedPrompts).IsFalse();
        await Assert.That(user.DailyImageAnalysisLimit).IsEqualTo(3);
        await Assert.That(user.ImageAnalysisCorrectionPromptLimit).IsEqualTo(3);
    }

    [Test]
    public async Task ChangeTier_ShouldUpdateToPremiumTier()
    {
        var user = AiChatUser.Create(UserId.New());

        user.ChangeTier(SubscriptionTier.Premium);

        await Assert.That(user.Tier).IsEqualTo(SubscriptionTier.Premium);
        await Assert.That(user.ConversationsLimit).IsEqualTo(20);
        await Assert.That(user.ConversationsMessagesHistoryDaysLimit).IsEqualTo(90);
        await Assert.That(user.DocumentsLimit).IsEqualTo(20);
        await Assert.That(user.PromptTokensLimit).IsEqualTo(1000);
        await Assert.That(user.ResponseTokensLimit).IsEqualTo(1000);
        await Assert.That(user.DailyPromptsLimit).IsEqualTo(-1);
        await Assert.That(user.CanExportData).IsTrue();
        await Assert.That(user.CanUseAdvancedPrompts).IsTrue();
        await Assert.That(user.DailyImageAnalysisLimit).IsEqualTo(10);
        await Assert.That(user.ImageAnalysisCorrectionPromptLimit).IsEqualTo(10);
    }

    [Test]
    public async Task IncrementActiveConversations_ValidData_ShouldIncrementActiveConversations()
    {
        var user = AiChatUser.Create(UserId.New());

        user.IncrementActiveConversations();

        await Assert.That(user.ActiveConversations).IsEqualTo(1);
    }

    [Test]
    public async Task IncrementActiveConversations_ExceedLimit_ShouldThrow()
    {
        var user = AiChatUser.Create(UserId.New());

        user.IncrementActiveConversations();
        user.IncrementActiveConversations();

        await Assert.That(user.IncrementActiveConversations)
            .Throws<InvalidOperationException>()
            .WithMessage($"User has reached the maximum number of active conversations ({user.ConversationsLimit}).");
    }
}
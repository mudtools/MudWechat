using FluentAssertions;
using Mud.AI.Agent.Tools.Errors;
using Mud.Wechat.AI.Agent.Results;
using Xunit;

namespace Mud.Wechat.AI.Tests.Results;

/// <summary>
/// 企微业务码 → 中立错误读数的码表守卫（<b>冻结码集</b>，增删即需同批改本文件）。
/// </summary>
/// <remarks>
/// 码表是模型自我纠错的唯一依据：把「限流」误分类成「参数错」会让模型反复改参数，
/// 把「无权限」误分类成「可重试」会让它无限重试。故码集与映射都用用例钉死。
/// </remarks>
public class WechatApiErrorCatalogTests
{
    [Fact]
    public void Codes_ShouldMatchFrozenSet()
    {
        WechatApiErrorCatalog.Codes.Should().Equal(
            [
                -1,
                40003,
                40014,
                42001,
                45009,
                45033,
                48002,
                60011,
                60111,
                81013,
            ],
            "码表是有实证的冻结集合，新增须先给官方依据");
    }

    [Theory]
    [InlineData(45009, ToolErrorCategory.Retryable, ToolErrorSubtype.RateLimited)]
    [InlineData(45033, ToolErrorCategory.Retryable, ToolErrorSubtype.RateLimited)]
    [InlineData(-1, ToolErrorCategory.Retryable, ToolErrorSubtype.Transient)]
    [InlineData(42001, ToolErrorCategory.Retryable, ToolErrorSubtype.Transient)]
    [InlineData(60011, ToolErrorCategory.Authorization, ToolErrorSubtype.AuthorizationDenied)]
    [InlineData(48002, ToolErrorCategory.Authorization, ToolErrorSubtype.AuthorizationDenied)]
    [InlineData(40003, ToolErrorCategory.Validation, ToolErrorSubtype.InvalidArgs)]
    [InlineData(60111, ToolErrorCategory.Validation, ToolErrorSubtype.InvalidArgs)]
    public void Classify_ShouldMapToExpectedSegment(int code, ToolErrorCategory category, string subtype)
    {
        var failure = WechatApiErrorCatalog.Classify(code);

        failure.Should().NotBeNull();
        failure!.Category.Should().Be(category);
        failure.Subtype.Should().Be(subtype);
        failure.Suggestions.Should().NotBeNullOrEmpty("分类必须给出恢复候选，否则模型只能重试");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(40056)]
    [InlineData(999999)]
    public void Classify_ForUncataloguedCode_ShouldReturnNull(int code)
    {
        WechatApiErrorCatalog.Classify(code).Should().BeNull("未收录的码交由中立层回落 upstream_error，不猜语义");
    }

    [Fact]
    public void HintOf_ForCataloguedCode_ShouldRenderSuggestions()
    {
        var hint = WechatApiErrorCatalog.HintOf(45009);

        hint.Should().StartWith("建议：");
        hint.Should().Contain("缩小查询范围");
    }

    [Fact]
    public void HintOf_ForUncataloguedCode_ShouldBeEmpty()
    {
        WechatApiErrorCatalog.HintOf(40056).Should().BeEmpty();
    }

    [Fact]
    public void HintOf_ForNullCode_ShouldBeEmpty()
    {
        WechatApiErrorCatalog.HintOf(null).Should().BeEmpty();
    }
}

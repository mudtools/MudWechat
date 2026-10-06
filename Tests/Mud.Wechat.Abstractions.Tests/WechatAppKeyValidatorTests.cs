// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Tests;

/// <summary>
/// AppKey 形状校验器下沉前后的行为等价性（合法/非法边界 + 只读判定与抛异常判定同源）。
/// </summary>
public class WechatAppKeyValidatorTests
{
    /// <summary>V1：合法键通过校验（字母/数字/'.'/'_'/'-'，首字符为字母或数字）。</summary>
    [Theory]
    [InlineData("default")]
    [InlineData("mp-main")]
    [InlineData("a.b_c-d")]
    [InlineData("0abc")]
    public void Validate_ShouldAcceptLegalKeys(string appKey)
    {
        var act = () => WechatAppKeyValidator.Validate(appKey);
        act.Should().NotThrow();
        WechatAppKeyValidator.IsValid(appKey).Should().BeTrue();
    }

    /// <summary>
    /// V2：非法键拒绝——重点是 ':'（会造成令牌持久化键别名 ⇒ 跨应用串号）与 '/'、'@'、空格、首字符分隔符。
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("a:b")]
    [InlineData("a/b")]
    [InlineData("a@b")]
    [InlineData("a b")]
    [InlineData(".abc")]
    [InlineData("-abc")]
    [InlineData("_abc")]
    [InlineData("中")]
    public void Validate_ShouldRejectIllegalKeys(string appKey)
    {
        var act = () => WechatAppKeyValidator.Validate(appKey);
        act.Should().Throw<InvalidOperationException>();
        WechatAppKeyValidator.IsValid(appKey).Should().BeFalse();
    }

    /// <summary>V3：超长拒绝（> 128）。</summary>
    [Fact]
    public void Validate_ShouldRejectOverlongKey()
    {
        var key = new string('a', WechatAppKeyValidator.MaxLength + 1);

        var act = () => WechatAppKeyValidator.Validate(key);
        act.Should().Throw<InvalidOperationException>().WithMessage("*长度不得超过*");
        WechatAppKeyValidator.IsValid(key).Should().BeFalse();

        WechatAppKeyValidator.IsValid(new string('a', WechatAppKeyValidator.MaxLength)).Should().BeTrue();
    }

    /// <summary>V4：只读判定与抛异常判定必须同源（只读变体是调用点避免「以异常做流程控制」的入口）。</summary>
    [Theory]
    [InlineData("ok-key")]
    [InlineData("bad:key")]
    [InlineData("")]
    public void IsValid_ShouldAgreeWithValidate(string appKey)
    {
        var isValid = WechatAppKeyValidator.IsValid(appKey);

        var throws = false;
        try
        {
            WechatAppKeyValidator.Validate(appKey);
        }
        catch (InvalidOperationException)
        {
            throws = true;
        }

        isValid.Should().Be(!throws, "两个入口必须同源，否则形状规则会出现两套事实");
    }

    /// <summary>V5：null 安全（只读判定不抛）。</summary>
    [Fact]
    public void IsValid_ShouldReturnFalseForNull()
        => WechatAppKeyValidator.IsValid(null).Should().BeFalse();
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels;

namespace Mud.Wechat.Work.Abstractions.Tests.Exceptions;

/// <summary>
/// P2-4：<see cref="WechatWorkException.RequestUri"/> 构造期脱敏（剥离 query 与 userinfo）——
/// 企业微信的凭据均以 Query 承载（<c>corpsecret</c> / <c>suite_access_token</c> / <c>provider_access_token</c>）。
/// </summary>
public class WechatWorkExceptionTests
{
    [Fact]
    public void RequestUri_ShouldStripQuery_WhenCredentialsInQuery()
    {
        var ex = new WechatWorkException(40014,
            "invalid access_token",
            "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=ww-corp&corpsecret=SUPER-SECRET");

        ex.RequestUri.Should().Be("https://qyapi.weixin.qq.com/cgi-bin/gettoken");
        ex.RequestUri.Should().NotContain("SUPER-SECRET");
    }

    [Fact]
    public void RequestUri_ShouldStripUserInfo()
    {
        var ex = new WechatWorkException(-1, "boom", "https://user:pass@qyapi.weixin.qq.com/cgi-bin/gettoken");

        ex.RequestUri.Should().Be("https://***@qyapi.weixin.qq.com/cgi-bin/gettoken");
        ex.RequestUri.Should().NotContain("pass");
    }

    [Fact]
    public void RequestUri_ShouldBeNull_WhenNotProvided()
    {
        new WechatWorkException(-1, "boom").RequestUri.Should().BeNull();
    }

    [Fact]
    public void ThrowIfFailed_ShouldCarryErrorCodeAndRedactedUri()
    {
        var ex = Assert.Throws<WechatWorkException>(() => WechatWorkException.ThrowIfFailed(
            new WechatWorkResponse { ErrorCode = 42001, ErrorMessage = "expired" },
            "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpsecret=SUPER-SECRET"));

        ex.ErrorCode.Should().Be(42001);
        ex.RequestUri.Should().NotContain("SUPER-SECRET");
        ex.RequestUri.Should().Be("https://qyapi.weixin.qq.com/cgi-bin/gettoken");
    }

    [Fact]
    public void ThrowIfFailed_ShouldThrow_WhenResponseIsNull()
    {
        var ex = Assert.Throws<WechatWorkException>(
            () => WechatWorkException.ThrowIfFailed<WechatWorkResponse>(null));

        ex.ErrorCode.Should().Be(-1);
    }
}

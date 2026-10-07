// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Tests.Exceptions;

/// <summary>
/// 响应判错守卫与异常脱敏（「成功 / 失败 / 缺省」三态 + Query 凭据不外泄）。
/// </summary>
public class WechatApiResponseGuardTests
{
    private sealed class FakeResponse : IWechatApiResponse
    {
        public int ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

        public bool IsSuccess => ErrorCode == 0;
    }

    private sealed class FakeException : WechatApiException
    {
        public FakeException(int errorCode, string message, string? requestUri = null)
            : base(errorCode, message, requestUri)
        {
        }
    }

    private static readonly Func<int, string, string?, WechatApiException> Factory =
        static (code, message, uri) => new FakeException(code, message, uri);

    /// <summary>G1：响应为 null ⇒ 抛异常且 errcode = -1（与内存实现约定一致）。</summary>
    [Fact]
    public void ThrowIfFailed_ShouldThrowOnNullResponse()
    {
        var act = () => WechatApiResponseGuard.ThrowIfFailed<FakeResponse>(null, Factory, "https://api.weixin.qq.com/cgi-bin/token?secret=x");

        act.Should().Throw<FakeException>().Which.ErrorCode.Should().Be(-1);
    }

    /// <summary>G2：缺省 errcode（0）⇒ 视为成功（覆盖「成功响应不带 errcode」的官方形态）。</summary>
    [Fact]
    public void ThrowIfFailed_ShouldPassWhenErrorCodeDefaultsToZero()
    {
        var response = new FakeResponse { ErrorCode = 0 };
        var act = () => WechatApiResponseGuard.ThrowIfFailed(response, Factory);
        act.Should().NotThrow("成功响应体可能整体缺省 errcode，反序列化后 ErrorCode 保持默认 0");
    }

    /// <summary>G3：非零 errcode ⇒ 抛出且错误码透传。</summary>
    [Fact]
    public void ThrowIfFailed_ShouldThrowOnNonZeroErrorCode()
    {
        var response = new FakeResponse { ErrorCode = 40125, ErrorMessage = "invalid appsecret" };
        var act = () => WechatApiResponseGuard.ThrowIfFailed(response, Factory);

        act.Should().Throw<FakeException>()
            .Which.ErrorCode.Should().Be(40125);
    }

    /// <summary>G4：异常消息不回显凭据（仅 errcode/errmsg），请求地址剥离 query 与 userinfo。</summary>
    [Fact]
    public void Exception_ShouldRedactQueryAndUserInfo()
    {
        var uri = "https://user:pass@api.weixin.qq.com/cgi-bin/token?appid=wx1&secret=TOP_SECRET";
        var exception = new FakeException(40013, "invalid appid", uri);

        exception.RequestUri.Should().NotContain("TOP_SECRET", "凭据以 Query 承载，构造期必须剥离");
        exception.RequestUri.Should().NotContain("wx1");
        exception.RequestUri.Should().NotContain("pass", "userinfo 必须整体掩码");
        exception.RequestUri.Should().Be("https://***@api.weixin.qq.com/cgi-bin/token");
    }

    /// <summary>G5：请求地址为空时原样返回（不做无谓构造）。</summary>
    [Fact]
    public void Exception_ShouldKeepNullRequestUri()
        => new FakeException(0, "ok").RequestUri.Should().BeNull();
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Transport;

namespace Mud.Wechat.Pay.Tests.Download;

/// <summary>
/// 账单下载通道（<see cref="WechatPayBillDownloadService"/>）的输入闸与失败分类测试。
/// </summary>
/// <remarks>
/// <b>为何先测输入闸</b>：<c>download_url</c> 是官方返回的<b>字符串</b>，但它会被配置 / 中间件 / 日志重放
/// 等外部因素替换。若不加主机白名单就发送，等于把「请求去哪」交给上游字符串 ——
/// 故本组在<b>不触达网络</b>的前提下锁定 fail-fast 行为。
/// </remarks>
public class WechatPayBillDownloadServiceTests
{
    /// <summary>空地址必须拒绝（参数错误，而非发送空请求）。</summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task DownloadBillAsync_ShouldReject_WhenUrlBlank(string url)
    {
        var sut = CreateSut();

        var act = async () => await sut.DownloadBillAsync(url);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    /// <summary>非官方主机必须 fail-fast（SSRF 纵深防御，不得把请求发到第三方域）。</summary>
    [Theory]
    [InlineData("https://evil.example.com/v3/billdownload/file?token=abc")]
    [InlineData("https://api.mch.weixin.qq.com.evil.com/v3/billdownload/file")]
    [InlineData("https://sub.api.mch.weixin.qq.com/v3/billdownload/file")]
    public async Task DownloadBillAsync_ShouldReject_WhenHostNotOfficial(string url)
    {
        var sut = CreateSut();

        var act = async () => await sut.DownloadBillAsync(url);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*白名单*");
    }

    /// <summary>非绝对 URL / 非 HTTP(S) 方案亦须拒绝。</summary>
    [Theory]
    [InlineData("/v3/billdownload/file?token=abc")]
    [InlineData("ftp://api.mch.weixin.qq.com/v3/billdownload/file")]
    public async Task DownloadBillAsync_ShouldReject_WhenUrlNotHttpAbsolute(string url)
    {
        var sut = CreateSut();

        var act = async () => await sut.DownloadBillAsync(url);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    /// <summary>非法输入不得触达 HTTP 客户端（逐条断言「拒绝发生在发送之前」）。</summary>
    [Fact]
    public async Task DownloadBillAsync_ShouldNotSendRequest_WhenInputRejected()
    {
        var client = new Mock<IWechatPayHttpClient>(MockBehavior.Strict);
        var sut = new WechatPayBillDownloadService(client.Object, NullLogger<WechatPayBillDownloadService>.Instance);

        var act = async () => await sut.DownloadBillAsync("https://evil.example.com/x");

        await act.Should().ThrowAsync<InvalidOperationException>();
        client.VerifyNoOtherCalls();
    }

    private static WechatPayBillDownloadService CreateSut()
        => new(new Mock<IWechatPayHttpClient>(MockBehavior.Loose).Object,
               NullLogger<WechatPayBillDownloadService>.Instance);
}

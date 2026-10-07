// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Moq;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Web;

namespace Mud.Wechat.OfficialAccount.Tests.Web;

/// <summary>
/// JS-SDK 签名用例（V7 已按官方页面原文核验；含**官方样例向量**）。
/// </summary>
public class MpJsApiSignatureTests
{
    // —— 官方「JS-SDK」页附录1 给出的样例参数与期望签名（原文照录）——
    private const string OfficialTicket =
        "sM4AOVdWfPE4DxkXGEs8VMCPGGVi4C3VM0P37wVUCFvkVAy_90u5h9nbSlYy3-Sl-HhTdfl2fzFy1AOcHKP7qg";

    private const string OfficialNonceStr = "Wm3WZYTPz0wzccnW";
    private const long OfficialTimestamp = 1414587457;
    private const string OfficialUrl = "http://mp.weixin.qq.com?params=value";
    private const string OfficialSignature = "0f9de62fce790f9a083d5c99e95740ceb90c27ed";

    /// <summary>官方样例向量：算法实现必须与该向量**逐位一致**（算法正确性的唯一权威判据）。</summary>
    [Fact]
    public void OfficialSample_ShouldMatchOfficialSignature()
    {
        var actual = MpJsApiSignature.Compute(
            OfficialTicket, OfficialNonceStr, OfficialTimestamp, OfficialUrl);

        actual.Should().Be(OfficialSignature);
    }

    /// <summary>待签名串的小写字段名与字典序（官方明文：jsapi_ticket → noncestr → timestamp → url）。</summary>
    [Fact]
    public void BuildStringToSign_ShouldUseLowercaseFieldsInAsciiOrder()
    {
        var string1 = MpJsApiSignature.BuildStringToSign(
            OfficialTicket, OfficialNonceStr, OfficialTimestamp, OfficialUrl);

        string1.Should().Be(
            "jsapi_ticket=" + OfficialTicket +
            "&noncestr=" + OfficialNonceStr +
            "&timestamp=" + OfficialTimestamp.ToString(System.Globalization.CultureInfo.InvariantCulture) +
            "&url=" + OfficialUrl);
    }

    /// <summary>url 必须去除 <c>#</c> 及其后片段（官方明确要求）；<c>?</c> 查询串保留。</summary>
    [Fact]
    public void NormalizeUrl_ShouldStripFragmentAndKeepQuery()
    {
        MpJsApiSignature.NormalizeUrl("https://a.com/p?x=1#/route")
            .Should().Be("https://a.com/p?x=1");

        MpJsApiSignature.NormalizeUrl("https://a.com/#frag").Should().Be("https://a.com/");

        // 签名入参带 fragment 时，等价于对去 fragment 后的 URL 签名。
        var withFragment = MpJsApiSignature.Compute(OfficialTicket, OfficialNonceStr, OfficialTimestamp,
            OfficialUrl + "#frag");
        withFragment.Should().Be(OfficialSignature);

        ((Action)(() => MpJsApiSignature.NormalizeUrl(" "))).Should().Throw<ArgumentException>();
    }

    /// <summary>「字段名和字段值都采用原始值，不进行 URL 转义」——含元字符时也必须原样参与。</summary>
    [Fact]
    public void Compute_ShouldNotUrlEscapeValues()
    {
        var string1 = MpJsApiSignature.BuildStringToSign("t&x=1", "n&y=2", 1, "http://a.com/?q=1&r=2");

        string1.Should().Be("jsapi_ticket=t&x=1&noncestr=n&y=2&timestamp=1&url=http://a.com/?q=1&r=2");
    }

    /// <summary>随机串：长度 16、两次不同、仅字母数字（官方样例形态）。</summary>
    [Fact]
    public void CreateNonceStr_ShouldBeRandomAlphanumeric16()
    {
        var first = MpJsApiSignature.CreateNonceStr();
        var second = MpJsApiSignature.CreateNonceStr();

        first.Should().HaveLength(16);
        first.Should().MatchRegex("^[A-Za-z0-9]{16}$");
        first.Should().NotBe(second);
    }

    /// <summary>签名服务：票据来自 <c>IMpJsApiTicketManager</c>（不重复实现票据缓存），结果自洽且 URL 已规范化。</summary>
    [Fact]
    public async Task SignAsync_ShouldUseTicketManagerAndReturnConsistentResult()
    {
        var ticketManager = new Mock<IMpJsApiTicketManager>();
        ticketManager.Setup(m => m.GetTicketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(OfficialTicket);

        var fixedNow = DateTimeOffset.FromUnixTimeSeconds(OfficialTimestamp);
        var service = new MpJsApiSignatureService(
            ticketManager.Object,
            Microsoft.Extensions.Options.Options.Create(
                new MpAppConfig { AppKey = "mp1", AppId = "wxTestAppId" }),
            () => fixedNow);

        var result = await service.SignAsync("https://a.com/p#/route");

        result.AppId.Should().Be("wxTestAppId", "前端 wx.config 需要 appId（四份官方样例均回传）");
        result.Url.Should().Be("https://a.com/p", "签名 URL 不含 # 片段");
        result.TimeStamp.Should().Be(OfficialTimestamp);
        result.NonceStr.Should().HaveLength(16);
        result.Signature.Should().Be(
            MpJsApiSignature.Compute(OfficialTicket, result.NonceStr, result.TimeStamp, result.Url));
        ticketManager.Verify(m => m.GetTicketAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>票据缺失 ⇒ 明确失败（不回退到空票据签名，避免产生必然被前端判为 invalid signature 的脏签名）。</summary>
    [Fact]
    public async Task SignAsync_ShouldFailFastWhenTicketMissing()
    {
        var ticketManager = new Mock<IMpJsApiTicketManager>();
        ticketManager.Setup(m => m.GetTicketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        var service = new MpJsApiSignatureService(
            ticketManager.Object,
            Microsoft.Extensions.Options.Options.Create(
                new MpAppConfig { AppKey = "mp1", AppId = "wxTestAppId" }));

        await service.Invoking(s => s.SignAsync("https://a.com/p"))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    /// <summary>应用配置缺 AppId ⇒ 构造即失败（不回包让前端拿不到 appId 的签名结果）。</summary>
    [Fact]
    public void Constructor_ShouldRequireAppId()
    {
        var ticketManager = new Mock<IMpJsApiTicketManager>();

        ((Action)(() => new MpJsApiSignatureService(
                ticketManager.Object,
                Microsoft.Extensions.Options.Options.Create(new MpAppConfig { AppKey = "mp1" }))))
            .Should().Throw<ArgumentException>();
    }
}

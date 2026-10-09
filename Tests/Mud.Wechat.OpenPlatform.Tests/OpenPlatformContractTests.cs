// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// 第三方平台官方契约常量锁定（照官方原文，<b>不得「纠正」</b>）。
/// </summary>
/// <remarks>
/// <b>官方来源</b>：《令牌（component_access_token）》
/// <c>developers.weixin.qq.com/doc/oplatform/Third-party_Platforms/2.0/api/ThirdParty/token/component_access_token.html</c>
/// （2026-10-09 逐字核验）。
/// </remarks>
public class OpenPlatformContractTests
{
    /// <summary>路径与字段名逐字锁定。</summary>
    [Fact]
    public void Contract_ShouldMatchOfficialValues()
    {
        OpenPlatformContract.ApiBaseUrl.Should().Be("https://api.weixin.qq.com");
        OpenPlatformContract.ComponentTokenPath.Should().Be("/cgi-bin/component/api_component_token");

        // 请求体三字段（官方原文，全必填）。
        OpenPlatformContract.ComponentAppIdField.Should().Be("component_appid");
        OpenPlatformContract.ComponentAppSecretField.Should().Be("component_appsecret");
        OpenPlatformContract.ComponentVerifyTicketField.Should().Be("component_verify_ticket");

        // 应答两字段。
        OpenPlatformContract.ComponentAccessTokenField.Should().Be("component_access_token");
        OpenPlatformContract.ExpiresInField.Should().Be("expires_in");
    }

    /// <summary>有效期与提前刷新窗口的算术自洽性。</summary>
    /// <remarks>
    /// <b>为何要锁这条不变量</b>：提前窗口一旦 ≥ 有效期，就等价于「每次调用都刷新」——
    /// 会把官方接口打成高频轮询（官方对令牌获取本身有频次限制）。这个错误在代码里
    /// <b>完全看不出来</b>（两个常量各自都「合理」），只有把关系写死才能拦住。
    /// </remarks>
    [Fact]
    public void RefreshLead_ShouldBeShorterThanTokenLifetime()
    {
        OpenPlatformContract.ComponentTokenLifetimeSeconds.Should().Be(7200, "官方原文：有效期为 2 小时");
        OpenPlatformContract.RecommendedRefreshLeadSeconds.Should().Be(600,
            "官方建议「1 小时 50 分」刷新 ⇒ 提前 10 分钟");

        OpenPlatformContract.RecommendedRefreshLeadSeconds
            .Should().BeLessThan(OpenPlatformContract.ComponentTokenLifetimeSeconds,
                "提前窗口必须严格小于有效期，否则等价于「每次调用都刷新」");
    }
}

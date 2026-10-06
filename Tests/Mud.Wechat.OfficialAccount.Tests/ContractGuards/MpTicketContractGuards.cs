// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.DataModels.WebDev;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// JS-SDK / 卡券票据域契约守卫：官方取值、凭证形态、缓存钥匙隔离与「禁止绕过缓存」约束。
/// </summary>
public class MpTicketContractGuards
{
    /// <summary>TK1：官方票据类型取值与有效期锁定。</summary>
    [Fact]
    public void TicketTypes_ShouldMatchOfficialValues()
    {
        MpTicketTypes.JsApi.Should().Be("jsapi");
        MpTicketTypes.WxCard.Should().Be("wx_card");
        MpTicketTypes.ExpireSeconds.Should().Be(7200, "官方：Api_ticket 有效期为 7200 秒");

        // 键前缀必须互不相同，否则两类票据共用键空间 ⇒ 卡券签名会拿到 JS-SDK 票据。
        MpTicketTypes.JsApiChannelKeyPrefix.Should().NotBe(MpTicketTypes.WxCardChannelKeyPrefix);
        MpTicketTypes.JsApiChannelKeyPrefix.Should().Be("Wechat.Mp.JsApiTicket");
        MpTicketTypes.WxCardChannelKeyPrefix.Should().Be("Wechat.Mp.WxCardTicket");
    }

    /// <summary>
    /// TK2（**关键**）：票据签发接口必须「不带 <c>[Token]</c>」且令牌显式传参。
    /// </summary>
    /// <remarks>
    /// 若带上 <c>[Token]</c>，票据刷新会进入 errcode 恢复链路，而恢复链路又依赖令牌管理器
    /// ⇒ 形成「令牌刷新 → 取令牌 → 票据请求 → 触发令牌恢复 → 再取令牌」的递归。
    /// 本守卫与 <c>IMpAuthentication</c> 的同类断言同源。
    /// </remarks>
    [Fact]
    public void TicketService_ShouldNotUseTokenInjection()
    {
        typeof(IMpTicketService).GetCustomAttribute<TokenAttribute>()
            .Should().BeNull("票据签发接口不得带 [Token]（否则与令牌恢复链路互相递归）");

        var method = typeof(IMpTicketService).GetMethod(nameof(IMpTicketService.GetTicketAsync))!;
        var route = method.GetCustomAttribute<GetAttribute>();
        route.Should().NotBeNull("官方契约：GET");
        route!.RequestUri.Should().Be("/cgi-bin/ticket/getticket");

        var queryNames = method.GetParameters()
            .SelectMany(p => p.GetCustomAttributes<QueryAttribute>())
            .Select(a => a.Name!)
            .ToList();

        queryNames.Should().BeEquivalentTo(new[] { "access_token", "type" },
            "官方两个业务 Query 参数：access_token 与 type（令牌显式传参，不经框架注入）");
    }

    /// <summary>TK3：票据响应 DTO 的官方字段名与 AOT 登记。</summary>
    [Fact]
    public void TicketResponse_ShouldLockOfficialFieldNamesAndRegisterContext()
    {
        var jsonNames = typeof(MpGetTicketResponse).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .ToList();

        jsonNames.Should().BeEquivalentTo(new[] { "ticket", "expires_in", "errcode", "errmsg" });

        WebDevJsonContext.Default.GetTypeInfo(typeof(MpGetTicketResponse))
            .Should().NotBeNull("票据响应必须登记进 WebDevJsonContext（AOT）");
        typeof(MpGetTicketResponse).GetCustomAttribute<HttpJsonSerializableAttribute>()!
            .SerializerClassName.Should().Be("WebDev");
    }

    /// <summary>TK4：管理器契约的定位——票据类型由接口固定，且两类票据各持一份管理器。</summary>
    [Fact]
    public void TicketManagers_ShouldBeSplitByTicketType()
    {
        typeof(IMpJsApiTicketManager).Should().Implement<IMpTicketManager>();
        typeof(IMpWxCardTicketManager).Should().Implement<IMpTicketManager>();
        typeof(IMpJsApiTicketManager).Should().NotBe(typeof(IMpWxCardTicketManager),
            "官方仅两个合法票据取值 ⇒ 两类票据不得共用同一管理器（键空间必须隔离）");

        // 票据管理器**不是**注入型令牌：不得被登记进令牌管理器注册表（ITokenManagerRegistry）。
        typeof(IMpJsApiTicketManager).Should().NotImplement<Mud.HttpUtils.ITokenManager>(
            "票据由宿主取走自行使用（前端签名 / 卡券），不参与请求注入与 errcode 恢复链路");
    }

    /// <summary>
    /// TK5：**禁止暴露默认票据客户端**——源码文本守卫，防止后续为「方便」注册可直接解析的
    /// <c>IMpTicketService</c> 而让宿主绕过缓存逐次调用（官方频次受限）。
    /// </summary>
    [Fact]
    public void TicketService_ShouldNotBeRegisteredAsResolvableClient()
    {
        var root = GetRepositoryRoot();
        var extensions = File.ReadAllText(Path.Combine(
            root, "Mud.Wechat.OfficialAccount.Abstractions", "Extensions", "MpMultiAppExtensions.cs"));

        extensions.Should().NotContain("AddTicketWebApiHttpClient",
            "票据必须经管理器缓存取用；暴露默认客户端会让宿主绕过缓存触发官方频次限制");

        extensions.Should().Contain("IMpTicketFactory",
            "per-app 票据客户端工厂必须注册（票据管理器据此装配）");
    }

    /// <summary>TK6：官方频次约束必须留在源码文档中（不得静默删注释）。</summary>
    [Fact]
    public void TicketFrequencyConstraint_ShouldBeDocumented()
    {
        var root = GetRepositoryRoot();

        var dto = File.ReadAllText(Path.Combine(
            root, "Mud.Wechat.OfficialAccount.DataModels", "WebDev", "MpGetTicketResponse.cs"));
        dto.Should().Contain("调用次数非常有限", "官方「注意事项」原文必须留在 DTO 文档中");

        var manager = File.ReadAllText(Path.Combine(
            root, "Mud.Wechat.OfficialAccount.Abstractions", "Authentication", "Tickets", "IMpTicketManager.cs"));
        manager.Should().Contain("调用次数非常有限");
        manager.Should().Contain("不得", "必须显式声明「宿主不得绕过管理器逐次直调」");
    }

    /// <summary>TK7：票据端点不支持第三方平台调用（官方明文）必须记录在案。</summary>
    [Fact]
    public void TicketThirdPartyExclusion_ShouldBeDocumented()
    {
        var root = GetRepositoryRoot();
        var service = File.ReadAllText(Path.Combine(
            root, "Mud.Wechat.OfficialAccount.Abstractions", "Authentication", "Interfaces", "IMpTicketService.cs"));

        service.Should().Contain("不支持第三方平台调用");
    }

    private static string GetRepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }
}

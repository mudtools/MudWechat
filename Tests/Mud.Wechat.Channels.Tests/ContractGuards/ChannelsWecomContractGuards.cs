// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Wecom;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「企业微信关联」域契约守卫（CWC 系列：Wecom 域 1 端点路由 + 独立适用范围措辞 +
/// 令牌绑定 + DTO 字段 + JSON 上下文登记；官方无枚举值 ⇒ 无 CWC6）。
/// </summary>
/// <remarks>
/// <para>
/// <b>适用范围措辞锁定（官方契约，勿套用其他域口径）</b>：本端点仅「微信小店带货助手」账号类型可调用，
/// <b>第三方平台调用不支持</b>（无权限集 id）、云调用不支持——与其他域「129/131/192 权限集代调用」
/// 口径不同，接口 XML 文档必须保留该差异。
/// </para>
/// <para>
/// <b>企微归因链路（与 Shop 域的衔接）</b>：本端点换取的 <c>wecom_corp_id</c>/<c>wecom_user_id</c>
/// 即店铺传播物料三端点（H5 链接 / 二维码 / 口令）的选填企微参数来源；两域各持 DTO、不做跨域共用
/// （字段语义在本域为「查询入参」、在 Shop 域为「归因透传」）。
/// </para>
/// </remarks>
public class ChannelsWecomContractGuards
{
    private const string WecomRegistryGroupName = "Wecom";

    /// <summary>Wecom 域官方路由表（1 端点：POST + JSON 请求体）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] WecomRoutes =
    {
        (typeof(IChannelsWecomService), nameof(IChannelsWecomService.GetWecomIdAsync), typeof(PostAttribute), "/channels/ec/wecom/get_wecom_id"),
    };

    /// <summary>契约守卫 CWC1：Wecom 域 1 端点路由必须与官方契约一致。</summary>
    [Fact]
    public void WecomEndpoints_ShouldMatchOfficialRoutes()
    {
        WecomRoutes.Should().HaveCount(1, "企业微信关联域仅获取关联账号企微 id 1 端点");
        WecomRoutes.Select(r => r.Route).Distinct().Should().HaveCount(1);

        WecomRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/wecom/", StringComparison.Ordinal))
            .Should().Be(1, "本域端点走 /channels/ec/wecom/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in WecomRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CWC2：官方契约 POST + JSON 请求体——必须带 [Body]。</summary>
    [Fact]
    public void WecomEndpoints_ShouldBePost_WithJsonBody()
    {
        foreach (var (iface, method, _, _) in WecomRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST");
        }

        typeof(IChannelsWecomService).GetMethod(nameof(IChannelsWecomService.GetWecomIdAsync))!
            .GetParameters().Should().Contain(
                static p => p.GetCustomAttribute<BodyAttribute>() != null,
                "GetWecomIdAsync 官方契约为 POST JSON 请求体（corp_id / user_id）——必须声明 [Body]");
    }

    /// <summary>契约守卫 CWC3：令牌绑定与注册形态。</summary>
    [Fact]
    public void WecomInterface_ShouldBindChannelsToken_AndRegisterUnderWecomGroup()
    {
        var token = typeof(IChannelsWecomService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Wecom 域端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsWecomService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(WecomRegistryGroupName, "必须挂 Wecom 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Wecom");
        typeof(ChannelsServiceBuilder).GetMethod("AddWecomApi").Should().NotBeNull("必须提供 AddWecomApi 链式注册方法");
    }

    /// <summary>契约守卫 CWC4：Wecom 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void WecomDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = WecomJsonContext.Default;

        var domainTypes = typeof(ChannelsWecomIdResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Wecom"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t)
                        && !t.IsAbstract)
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(3,
            "Wecom 域 1 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（请求 1 + 响应 1 + data 嵌套 1 = 3）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Wecom 域命名空间，必须登记进 WecomJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(WecomRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Wecom");
        }

        // 常量类断言计数且未标序列化特性：官方响应无枚举值 ⇒ 本域无常量类。
        var constantTypes = domainTypes.Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null).ToList();
        constantTypes.Should().BeEmpty("Wecom 域官方无枚举值——不得私造常量类");
    }

    /// <summary>契约守卫 CWC5：官方字段名锁定（corp_id/user_id 入参 + wecom_corp_id/wecom_user_id 回参，照抄官方原文）。</summary>
    [Fact]
    public void WecomDataModels_ShouldLockOfficialFields()
    {
        AssertJsonProperty<ChannelsWecomIdRequest>("corp_id", "企业 corp_id（必填）");
        AssertJsonProperty<ChannelsWecomIdRequest>("user_id", "成员 user_id（选填）");
        AssertJsonProperty<ChannelsWecomIdResponse>("data", "回包信息");
        AssertJsonProperty<ChannelsWecomId>("wecom_corp_id", "企业微信 id");
        AssertJsonProperty<ChannelsWecomId>("wecom_user_id", "企业微信账号 id");

        // 共用裁决（负向）：corp_id 官方必填 ⇒ 非可空（与 Shop 域选填 wecom_corp_id 的可空性区分，不做跨域共用 DTO）。
        typeof(ChannelsWecomIdRequest).GetProperty(nameof(ChannelsWecomIdRequest.CorpId))!
            .PropertyType.Should().Be(typeof(string), "corp_id 官方必填——非可空");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}

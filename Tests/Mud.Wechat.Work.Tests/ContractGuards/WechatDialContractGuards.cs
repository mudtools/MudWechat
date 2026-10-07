// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.Dial;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 公费电话模块（Dial 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （官方仅自建应用开放——代开发与第三方权限表均标注暂不支持；
/// 与「紧急通知应用」域的 pstncc 路由族分属官方两棵章节树，路由 /cgi-bin/dial/ 勿与 /cgi-bin/pstncc/ 混淆）。
/// </summary>
public class WechatDialContractGuards
{
    private const string DialRegistryGroupName = "Dial";

    /// <summary>
    /// 契约守卫 DL1：公费电话域端点路由必须与官方契约一致
    /// （获取公费电话拨打记录 93662，官方即 POST；请求字段官方原文为 start_time / end_time 下划线形态）。
    /// </summary>
    [Fact]
    public void DialEndpoints_ShouldMatchOfficialRoutes()
    {
        var target = typeof(IWechatWorkInternalDialService).GetMethod(
            nameof(IWechatWorkInternalDialService.GetDialRecordAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        target.Should().NotBeNull("获取公费电话拨打记录必须声明于自建子接口");

        var attr = target!.GetCustomAttribute<PostAttribute>();
        attr.Should().NotBeNull("get_dial_record 官方即 POST，勿改成 GET");
        attr!.RequestUri.Should().Be("/cgi-bin/dial/get_dial_record", "路由必须与官方契约一致");

        // 请求字段名照官方原文（下划线形态），防止驼峰化漂移。
        typeof(GetDialRecordRequest).GetProperty(nameof(GetDialRecordRequest.StartTime))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("start_time", "官方原文即 start_time");
        typeof(GetDialRecordRequest).GetProperty(nameof(GetDialRecordRequest.EndTime))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("end_time", "官方原文即 end_time");
    }

    /// <summary>
    /// 契约守卫 DL2：仅单类应用开放域的接口层级——公费电话域官方仅自建应用开放，
    /// 为零端点父接口 + 仅自建子接口承载端点，继承链上不得出现第三方 / 代开发子接口。
    /// </summary>
    [Fact]
    public void DialSingleAppTypeFamily_ShouldHaveExactlyOneChildAndAbstractParent()
    {
        var parent = typeof(IWechatWorkDialService);
        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { typeof(IWechatWorkInternalDialService) },
            "继承链上不得出现自建之外的子接口：官方仅向自建应用开放（代开发/第三方暂不支持）");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("零端点父接口：端点全部声明于唯一的子接口");

        var childApi = typeof(IWechatWorkInternalDialService).GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull("子接口必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(DialRegistryGroupName,
            $"{nameof(IWechatWorkInternalDialService)} 必须挂 {DialRegistryGroupName} 注册组");
    }

    /// <summary>
    /// 契约守卫 DL3：令牌绑定——Dial 模块两个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；自建子接口必须声明 InternalAccessToken 归属域键）。
    /// </summary>
    [Fact]
    public void DialTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        foreach (var iface in new[] { typeof(IWechatWorkDialService), typeof(IWechatWorkInternalDialService) })
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 DL4：公费电话模块的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void DialDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = DialJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(GetDialRecordRequest), typeof(GetDialRecordResponse),
            typeof(DialRecord), typeof(DialCaller), typeof(DialCallee),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是公费电话模块契约面类型，必须登记进 DialJsonContext（AOT 源生成）");
        }
    }
}

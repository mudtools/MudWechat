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
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 智能机器人模块（Aibot 模块）HTTP 面契约守卫：路由表、接口层级、<b>无令牌</b>面与 DTO 上下文登记锁定
/// （官方仅企业自建开放，零端点父接口 + 唯一自建子接口承载端点）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：<see cref="IWechatWorkInternalEmergencyService"/>（官方仅自建开放，
/// 零端点父接口 + 唯一自建子接口承载端点）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：主动回复端点 <c>/cgi-bin/aibot/response</c> 以 URL 上的
/// <c>response_code</c> 为<b>一次性凭据</b>（每个 <c>response_url</c> 仅可调用一次、有效期 1 小时），
/// <b>不使用任何令牌链路</b> ⇒ 本域父子接口均不得声明 <c>[Token]</c>（第二个既存无令牌例外，
/// 例外清单见 <see cref="WechatTokenOwnerContractGuards"/> TO1）；
/// 官方 101138 支持面仅 <c>markdown</c> / <c>template_card</c>（<c>stream</c> 与媒体消息属长连接能力）；
/// 官方 101138 文档页未列出响应字段，响应按统一基底 <c>WechatWorkResponse</c> 承载（含 errcode/errmsg）。
/// </para>
/// </remarks>
public class WechatAibotContractGuards
{
    /// <summary>父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落 <c>Mud.Wechat.Work.Interfaces.Internal</c>）。</summary>
    private const string AibotParentImplementationClassName = "WechatWorkAibotService";

    private const string AibotRegistryGroupName = "Aibot";

    private const string AibotNamespace = "Mud.Wechat.Work.DataModels.Aibot";

    /// <summary>智能机器人域官方路由表（官方仅自建应用开放，端点在唯一自建子接口）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AibotRoutes =
    {
        // 主动回复消息（官方 101138；HTTP 方法即 POST）。
        (typeof(IWechatWorkInternalAibotService),
            nameof(IWechatWorkInternalAibotService.ReplyAsync),
            typeof(PostAttribute), "/cgi-bin/aibot/response"),
    };

    /// <summary>
    /// 契约守卫 AI1：智能机器人域端点路由与查询凭据必须与官方契约一致。
    /// </summary>
    [Fact]
    public void AibotEndpoints_ShouldMatchOfficialRoutes()
    {
        AibotRoutes.Should().HaveCount(1,
            "智能机器人 HTTP 面官方仅「主动回复消息」1 个端点（接收/被动回复为回调通道，不是 HTTP 端点）");

        foreach (var (iface, method, httpAttribute, route) in AibotRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");

            // 官方 101138：一次性凭据 response_code 走 Query。
            var queryParam = target.GetParameters()
                .Select(p => (Parameter: p, Attribute: p.GetCustomAttribute<QueryAttribute>()))
                .FirstOrDefault(x => x.Attribute != null);
            queryParam.Attribute.Should().NotBeNull(
                "官方 101138 以 response_code 查询参数承载一次性应答凭据");
            queryParam.Attribute!.Name.Should().Be("response_code",
                "参数名照抄官方原文（response_code，非 responseCode）");
        }
    }

    /// <summary>
    /// 契约守卫 AI2：接口层级与生成器注册形态——官方文档树整体位于「企业自建应用开发」分类下、
    /// 正文零提及第三方 / 服务商代开发，故继承链上必须恰好只有唯一自建子接口。
    /// </summary>
    [Fact]
    public void AibotInterfaceHierarchy_ShouldConvergeOnSelfBuildOnlyChild()
    {
        var assignable = typeof(IWechatWorkAibotService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t != typeof(IWechatWorkAibotService)
                        && typeof(IWechatWorkAibotService).IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { typeof(IWechatWorkInternalAibotService) },
            "智能机器人官方仅自建开放（机器人与其凭证均在企业管理后台配置），继承链上不得出现其它子接口");

        var parent = typeof(IWechatWorkAibotService);
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.TokenManage.Should().Be(nameof(IWechatAppManager),
            "无令牌 ≠ 不参与多应用切换：与全仓各域父接口形态一致（TokenManage 恒为 IWechatAppManager，"
            + "仅配置应用切换管理器、与令牌注入无关），且父子两级必须统一——否则生成器以 new 隐藏基类切换成员"
            + "（组件分析器 HTTPCLIENT028），基类调用会静默走默认模式");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("官方仅自建开放：父接口零端点，端点全部由唯一自建子接口承载");

        var child = typeof(IWechatWorkInternalAibotService);
        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(AibotRegistryGroupName,
            $"{child.Name} 必须挂 {AibotRegistryGroupName} 注册组");
        childApi.InheritedFrom.Should().Be(AibotParentImplementationClassName,
            $"{child.Name} 必须继承父接口生成实现类");
        childApi.TokenManage.Should().Be(nameof(IWechatAppManager),
            $"{child.Name} 的 TokenManage 须与父接口一致（两级统一，防基类切换成员被 new 隐藏）");
        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(1, $"{child.Name} 承载本域全部 1 条官方端点");
    }

    /// <summary>
    /// 契约守卫 AI3：<b>无令牌面</b>——父子接口均不得声明 <c>[Token]</c>。
    /// </summary>
    /// <remarks>
    /// 官方 101138 以 URL 一次性凭据 <c>response_code</c> 鉴权，与 <c>access_token</c> /
    /// <c>suite_access_token</c> / <c>provider_access_token</c> 三条链路均无关；
    /// 声明任一 <c>[Token]</c> 都会让生成器注入本不该存在的令牌参数（且可能改变注入参数名）。
    /// 子接口的无 <c>[Token]</c> 例外已同步登记在 TO1 的精确清单中。
    /// </remarks>
    [Fact]
    public void AibotInterfaces_ShouldNotDeclareTokenAttribute()
    {
        foreach (var iface in new[] { typeof(IWechatWorkAibotService), typeof(IWechatWorkInternalAibotService) })
        {
            iface.GetCustomAttribute<TokenAttribute>().Should().BeNull(
                $"{iface.Name} 不得声明 [Token]：官方 101138 以 response_code 一次性凭据鉴权，" +
                "不属任何令牌链路（G5 白名单与令牌归属域守卫均不覆盖本域）");
        }
    }

    /// <summary>
    /// 契约守卫 AI4：Aibot 域全部顶层 DTO 必须登记进 <c>AibotJsonContext</c>，
    /// 且 <c>SerializerClassName</c> 统一为域段 <c>Aibot</c>。
    /// </summary>
    [Fact]
    public void AibotDataModels_ShouldBeRegisteredInJsonContext()
    {
        var aibotContext = AibotJsonContext.Default;

        // 端点契约面（守卫是权威描述，新增/删减端点须同批更新）。
        var endpointContractTypes = new Type[]
        {
            typeof(AibotMessage), typeof(AibotReplyResponse),
            typeof(AibotTextBody), typeof(AibotMarkdownBody), typeof(AibotStreamBody),
            typeof(AibotStreamItem), typeof(AibotStreamImageItem),
        };

        foreach (var type in endpointContractTypes)
        {
            aibotContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是智能机器人端点契约面类型，必须登记进 AibotJsonContext（AOT 源生成）");
        }

        var domainTypes = typeof(AibotMessage).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == AibotNamespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(23,
            "智能机器人域契约面类型数漂移须先核对官方文档（101031/101138/100719/101027）再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            aibotContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于智能机器人域命名空间，必须登记进 AibotJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Aibot",
                $"{type.Name} 的 SerializerClassName 必须为智能机器人域段 Aibot");
        }
    }

    /// <summary>
    /// 契约守卫 AI5：官方契约陷阱锁定——
    /// ① 应答外壳字段名为 <c>msgsignature</c>（无下划线），不得「顺手统一」为 <c>msg_signature</c>；
    /// ② <c>create_time</c> 为 <c>long?</c>（官方为数值型时间戳，非字符串）；
    /// ③ 智能机器人模板卡片选择项为三层嵌套 <c>selected_items.selected_item[].option_ids.option_id[]</c>。
    /// </summary>
    [Fact]
    public void AibotDataModels_ShouldLockOfficialContractTraps()
    {
        JsonNameShouldBe(typeof(AibotEncryptedEnvelope), nameof(AibotEncryptedEnvelope.Msgsignature), "msgsignature");
        typeof(AibotEncryptedEnvelope).GetProperty(nameof(AibotEncryptedEnvelope.Msgsignature))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().NotBe("msg_signature",
                "官方 101033：应答侧签名字段名为 msgsignature（无下划线），与请求侧 msg_signature 不同");

        typeof(AibotEventCallback).GetProperty(nameof(AibotEventCallback.CreateTime))!
            .PropertyType.Should().Be(typeof(long?), "官方 101027 create_time 为数值型 Unix 秒时间戳");

        typeof(AibotTemplateCardEvent).GetProperty(nameof(AibotTemplateCardEvent.EventKey))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("event_key",
                "官方 101027 字段名为 event_key（下划线），非 eventkey");
        typeof(AibotTemplateCardEvent).GetProperty(nameof(AibotTemplateCardEvent.TaskId))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("task_id",
                "官方 101027 字段名为 task_id（下划线），非 taskid");

        // 三层嵌套：selected_items → selected_item[] → option_ids → option_id[]。
        typeof(AibotTemplateCardEvent).GetProperty(nameof(AibotTemplateCardEvent.SelectedItems))!
            .PropertyType.Should().Be(typeof(AibotSelectedItems));
        typeof(AibotSelectedItems).GetProperty(nameof(AibotSelectedItems.SelectedItem))!
            .PropertyType.Should().Be(typeof(List<AibotSelectedItem>));
        typeof(AibotSelectedItem).GetProperty(nameof(AibotSelectedItem.OptionIds))!
            .PropertyType.Should().Be(typeof(AibotOptionIds));
        typeof(AibotOptionIds).GetProperty(nameof(AibotOptionIds.OptionId))!
            .PropertyType.Should().Be(typeof(List<string>));

        // 回调媒体结构体：url + aeskey（长连接每链接唯一；回调地址模式不返回 aeskey）。
        typeof(AibotMediaContent).GetProperty(nameof(AibotMediaContent.AesKey))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("aeskey");
        typeof(AibotFrom).GetProperty(nameof(AibotFrom.CorpId))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("corpid");

        // 机器人事件触发者必填 userid；群聊 chatid 为可空（仅群聊返回）。
        typeof(WechatBotEventTypes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Should().HaveCount(11,
                "智能机器人键集固定为 4 事件键（enter_chat/template_card_event/feedback_event/disconnected_event）" +
                "+ 7 消息键（text/image/mixed/voice/file/video/stream）");
    }

    /// <summary>JSON 字段名断言（属性映射的官方字段名必须与官方原文一致）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}

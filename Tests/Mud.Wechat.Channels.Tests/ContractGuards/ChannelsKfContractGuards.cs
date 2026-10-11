// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Kf;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「商家客服」域契约守卫（CK 系列：Kf 域 2 端点路由 + multipart 上传通路 +
/// 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量 + 与企微微信客服的概念辨析）。
/// </summary>
/// <remarks>
/// <para>
/// <b>概念辨析（守卫措辞锁定）</b>：本域是微信小店的商家客服（<c>/channels/ec/commkf/*</c>，
/// 小店 <c>access_token</c>），<b>不是</b>企微线的微信客服（Work 线 <c>Interfaces/KF/</c>，
/// 企微 <c>Wechat.AccessToken</c>）。路由面由 CH-R2 锁定（<c>/channels/ec/commkf/*</c> 与
/// Work 线 <c>/cgi-bin/kf/*</c> 天然无交叠）。
/// </para>
/// <para>
/// <b>客服消息时序契约</b>：48 小时回复窗口 + 最多回复 5 条（超出分别报 10190009 / 10190008）。
/// </para>
/// </remarks>
public class ChannelsKfContractGuards
{
    private const string KfRegistryGroupName = "Kf";

    /// <summary>Kf 域官方路由表（2 端点：/channels/ec/commkf/*，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] KfRoutes =
    {
        (typeof(IChannelsKfService), nameof(IChannelsKfService.CosUploadAsync), typeof(PostAttribute), "/channels/ec/commkf/cosupload"),
        (typeof(IChannelsKfService), nameof(IChannelsKfService.SendMessageAsync), typeof(PostAttribute), "/channels/ec/commkf/sendmsg"),
    };

    /// <summary>契约守卫 CK1：Kf 域 2 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void KfEndpoints_ShouldMatchOfficialRoutes()
    {
        // 2 端点 = 多媒体资源上传 1 + 发送消息 1（全部走 /channels/ec/commkf/* 主干前缀）。
        KfRoutes.Should().HaveCount(2, "商家客服域仅开放多媒体资源上传与发送消息两个端点");
        KfRoutes.Select(r => r.Route).Distinct().Should().HaveCount(2);

        KfRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/commkf/", StringComparison.Ordinal))
            .Should().Be(2, "2 端点全部走 /channels/ec/commkf/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in KfRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CK2：Kf 域端点全部为 POST；sendmsg 走 JSON [Body]，cosupload 官方为 multipart/form-data，须走 [MultipartForm] 通路且不得声明 [Body]。</summary>
    [Fact]
    public void KfEndpoints_ShouldAllBePost_WithMultipartUploadRules()
    {
        foreach (var (iface, method, _, _) in KfRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST");
        }

        // sendmsg：JSON 请求体（open_id / msg_type + 类型化内容对象）。
        var sendMsg = typeof(IChannelsKfService).GetMethod(nameof(IChannelsKfService.SendMessageAsync))!;
        sendMsg.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().ContainSingle("sendmsg 官方契约走 JSON 请求体");

        // cosupload：官方请求体为 multipart/form-data（msg_type / open_id / file 均为表单字段）——
        // 走库内既有声明式上传通路 [MultipartForm] IFormContent（与 Work 线媒体上传同形态），不得造 JSON [Body]。
        var cosUpload = typeof(IChannelsKfService).GetMethod(nameof(IChannelsKfService.CosUploadAsync))!;
        var formParam = cosUpload.GetParameters().Should().Contain(
                static p => p.ParameterType == typeof(IFormContent),
                "cosupload 官方为 multipart/form-data 上传，参数类型必须为组件 IFormContent（AOT 安全的声明式上传通路）")
            .Subject;
        formParam.GetCustomAttribute<MultipartFormAttribute>().Should().NotBeNull(
            "multipart 表单参数必须标 [MultipartForm]：才走 IFormContent.ToHttpContentAsync 通路");
        cosUpload.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("cosupload 官方请求体为 multipart/form-data——不得声明 [Body]");
    }

    /// <summary>契约守卫 CK3：令牌绑定与注册形态。</summary>
    [Fact]
    public void KfInterface_ShouldBindChannelsToken_AndRegisterUnderKfGroup()
    {
        var token = typeof(IChannelsKfService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Kf 域端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsKfService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(KfRegistryGroupName, "必须挂 Kf 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Kf");
        typeof(ChannelsServiceBuilder).GetMethod("AddKfApi").Should().NotBeNull("必须提供 AddKfApi 链式注册方法");
    }

    /// <summary>契约守卫 CK4：Kf 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void KfDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = KfJsonContext.Default;

        var domainTypes = typeof(ChannelsKfSendMsgRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Kf"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(7,
            "Kf 域 2 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（sendmsg 请求 1 + 文本内容 1 + 媒体内容 1 + 商品卡片 1 + 订单卡片 1 + sendmsg 响应 1 + cosupload 响应 1 = 7）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Kf 域命名空间，必须登记进 KfJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(KfRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Kf");
        }

        // 常量类（静态常量类不参与序列化）：Kf 域 2 个，均不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().HaveCount(2,
            "Kf 域常量类 = 消息类型（sendmsg）+ 媒体资源类型（cosupload）");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CK5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 共用裁决）。</summary>
    [Fact]
    public void KfDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 发送消息。
        AssertJsonProperty<ChannelsKfSendMsgRequest>("request_id", "唯一任务 id（去重）");
        AssertJsonProperty<ChannelsKfSendMsgRequest>("open_id", "用户 openid");
        AssertJsonProperty<ChannelsKfSendMsgRequest>("msg_type", "消息类型");
        AssertJsonProperty<ChannelsKfSendMsgRequest>("text", "文本消息内容");
        AssertJsonProperty<ChannelsKfSendMsgRequest>("product_share", "商品卡片消息内容");
        AssertJsonProperty<ChannelsKfSendMsgRequest>("order_share", "订单卡片消息内容");
        AssertJsonProperty<ChannelsKfTextContent>("content", "文本内容");
        AssertJsonProperty<ChannelsKfProductShareContent>("product_id", "商品 id");
        AssertJsonProperty<ChannelsKfOrderShareContent>("order_id", "订单 id");
        AssertJsonProperty<ChannelsKfSendMsgResponse>("msg_id", "消息单调自增 id");

        // 上传多媒体资源。
        AssertJsonProperty<ChannelsKfCosUploadResponse>("cos_url", "多媒体资源地址");

        // 共用 DTO 裁决：sendmsg 的 image / video / file 三个内容对象字段集完全一致（仅 cos_url）⇒ 共用 ChannelsKfMediaContent。
        AssertJsonProperty<ChannelsKfMediaContent>("cos_url", "上传多媒体 cos_url");
        typeof(ChannelsKfSendMsgRequest).GetProperty(nameof(ChannelsKfSendMsgRequest.Image))!
            .PropertyType.Should().Be(typeof(ChannelsKfMediaContent));
        typeof(ChannelsKfSendMsgRequest).GetProperty(nameof(ChannelsKfSendMsgRequest.Video))!
            .PropertyType.Should().Be(typeof(ChannelsKfMediaContent));
        typeof(ChannelsKfSendMsgRequest).GetProperty(nameof(ChannelsKfSendMsgRequest.File))!
            .PropertyType.Should().Be(typeof(ChannelsKfMediaContent));
    }

    /// <summary>契约守卫 CK6：枚举常量字面量锁定。</summary>
    [Fact]
    public void KfEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsKfMsgTypes.Text.Should().Be("text");
        ChannelsKfMsgTypes.Image.Should().Be("image");
        ChannelsKfMsgTypes.Video.Should().Be("video");
        ChannelsKfMsgTypes.ProductShare.Should().Be("product_share");
        ChannelsKfMsgTypes.OrderShare.Should().Be("order_share");
        ChannelsKfMsgTypes.File.Should().Be("file");

        ChannelsKfMediaTypes.Video.Should().Be("video");
        ChannelsKfMediaTypes.File.Should().Be("file");
        ChannelsKfMediaTypes.Image.Should().Be("image");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}

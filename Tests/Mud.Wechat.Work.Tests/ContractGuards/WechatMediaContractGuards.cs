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
using Mud.Wechat.Work.DataModels.Media;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 素材管理模块（Media 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （临时素材上传/获取域 + 上传图片域 + 高清语音素材域 + 异步上传临时素材域：三类应用公共面收敛父接口；
/// 服务商上传临时素材域：官方仅第三方应用开放，走 provider_access_token 独立成族）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：素材管理公共面（上传临时素材 / 获取临时素材 / 上传图片 / 获取高清语音素材 / 异步上传临时素材）为
/// 三类应用公共面收敛父接口（自建 / 第三方 / 代开发子接口均为零差异端点空标记）；
/// 服务商上传临时素材域（<c>/cgi-bin/service/media/upload</c>）官方仅第三方应用开放
///（零端点父接口 + 仅第三方子接口承载，形态对齐微信客服组件域 / 获客助手组件代支付流水族）。
/// </para>
/// </remarks>
public class WechatMediaContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string MediaParentImplementationClassName = "WechatWorkMediaService";

    private const string ServiceMediaParentImplementationClassName = "WechatWorkServiceMediaService";

    private const string MediaRegistryGroupName = "Media";

    /// <summary>
    /// 素材管理公共面官方路由表（父接口 6 条公共端点：上传临时素材 / 上传图片 / 异步上传两族为 POST，
    /// 获取临时素材 / 获取高清语音素材为 GET 且返回二进制文件流）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] MediaRoutes =
    {
        // 上传临时素材（自建 90253、第三方 90389、代开发 96484），type 走 Query、文件走 multipart。
        (typeof(IWechatWorkMediaService),
            nameof(IWechatWorkMediaService.UploadTemporaryMediaAsync),
            typeof(PostAttribute), "/cgi-bin/media/upload"),
        // 获取临时素材（自建 90256、第三方 90390、代开发 96486），media_id 走 Query，成功返回二进制。
        (typeof(IWechatWorkMediaService),
            nameof(IWechatWorkMediaService.GetTemporaryMediaAsync),
            typeof(GetAttribute), "/cgi-bin/media/get"),
        // 上传图片（自建 90254、第三方 90392、代开发 96485），文件走 multipart。
        (typeof(IWechatWorkMediaService),
            nameof(IWechatWorkMediaService.UploadImageAsync),
            typeof(PostAttribute), "/cgi-bin/media/uploadimg"),
        // 获取高清语音素材（自建 90255、第三方 90391、代开发 96487），media_id 走 Query，成功返回二进制。
        (typeof(IWechatWorkMediaService),
            nameof(IWechatWorkMediaService.GetHdVoiceMediaAsync),
            typeof(GetAttribute), "/cgi-bin/media/get/jssdk"),
        // 生成异步上传任务（自建 96219、第三方 97126、代开发 96488）。
        (typeof(IWechatWorkMediaService),
            nameof(IWechatWorkMediaService.UploadMediaByUrlAsync),
            typeof(PostAttribute), "/cgi-bin/media/upload_by_url"),
        // 查询异步上传任务结果（自建 96219、第三方 97126、代开发 96488）。
        (typeof(IWechatWorkMediaService),
            nameof(IWechatWorkMediaService.GetUploadMediaByUrlResultAsync),
            typeof(PostAttribute), "/cgi-bin/media/get_upload_by_url_result"),
    };

    /// <summary>
    /// 服务商上传临时素材官方路由表（仅第三方应用开放，1 条端点声明于第三方子接口，
    /// 走 provider_access_token，路由在 <c>/cgi-bin/service/media/</c> 下）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ServiceMediaRoutes =
    {
        // 服务商上传临时素材（第三方 99310），type / attachment_type 走 Query、文件走 multipart。
        (typeof(IWechatWorkThirdPartyServiceMediaService),
            nameof(IWechatWorkThirdPartyServiceMediaService.UploadServiceMediaAsync),
            typeof(PostAttribute), "/cgi-bin/service/media/upload"),
    };

    /// <summary>
    /// 契约守卫 MD1a：素材管理公共面全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 获取临时素材与获取高清语音素材官方即 GET 且成功响应为二进制文件流（勿改 POST；
    /// 失败时官方返回 JSON，由 errcode 判定器统一抛 <see cref="WechatWorkException"/>）。
    /// </summary>
    [Fact]
    public void MediaEndpoints_ShouldMatchOfficialRoutes()
    {
        MediaRoutes.Should().HaveCount(6,
            "素材管理公共面 6 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = MediaRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(6, "素材管理公共面各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in MediaRoutes)
        {
            // DeclaredOnly：公共端点必须落在父接口自身声明，不得下沉到子接口重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 获取临时素材 / 获取高清语音素材为二进制下载端点：返回 byte[]（文件流），
        // 且必须支持可选 Range 请求头（异步上传 media_id 超 20M 时官方强制分块下载，否则 830002）。
        foreach (var downloadMethod in new[]
                 {
                     nameof(IWechatWorkMediaService.GetTemporaryMediaAsync),
                     nameof(IWechatWorkMediaService.GetHdVoiceMediaAsync),
                 })
        {
            var target = typeof(IWechatWorkMediaService).GetMethod(
                downloadMethod, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull(downloadMethod);

            target!.ReturnType.Should().Be(typeof(Task<byte[]?>),
                $"{downloadMethod} 官方成功响应为二进制文件流（Content-Type / Content-disposition），必须以 byte[] 承载");
            var rangeParam = target.GetParameters().SingleOrDefault(p =>
                p.GetCustomAttribute<HeaderAttribute>() is { } h && h.Name == "Range");
            rangeParam.Should().NotBeNull($"{downloadMethod} 必须支持可选 Range 请求头（分块下载）");
            rangeParam!.ParameterType.Should().Be(typeof(string),
                "Range 请求头为可选参数（string，可空）");
            rangeParam.HasDefaultValue.Should().BeTrue("Range 请求头不得强制必填（普通 media_id 无需分块）");
        }
    }

    /// <summary>
    /// 契约守卫 MD1b：服务商上传临时素材路由必须与官方契约一致（99310，仅第三方应用开放；
    /// attachment_type 为官方选填 Query 参数，不得改为必填）。
    /// </summary>
    [Fact]
    public void ServiceMediaEndpoints_ShouldMatchOfficialRoutes()
    {
        ServiceMediaRoutes.Should().HaveCount(1,
            "服务商上传临时素材域仅 1 个端点（官方仅第三方应用开放，走 provider_access_token）");

        foreach (var (iface, method, httpAttribute, route) in ServiceMediaRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // attachment_type 为官方选填参数（如 3 - 收银台），须以 [Query("attachment_type")] 显式承载且可缺省。
        var uploadMethod = typeof(IWechatWorkThirdPartyServiceMediaService).GetMethod(
            nameof(IWechatWorkThirdPartyServiceMediaService.UploadServiceMediaAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        uploadMethod.Should().NotBeNull();
        var attachmentTypeParam = uploadMethod!.GetParameters().SingleOrDefault(p =>
            p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "attachment_type");
        attachmentTypeParam.Should().NotBeNull("attachment_type 必须以 [Query(\"attachment_type\")] 显式承载");
        attachmentTypeParam!.HasDefaultValue.Should().BeTrue("attachment_type 为官方选填参数，不得强制必填");
    }

    /// <summary>
    /// 契约守卫 MD2：素材管理域接口层级与生成器注册形态——公共端点收敛于 IsAbstract 父接口，
    /// 自建 / 第三方 / 代开发子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void MediaInterfaceHierarchy_ShouldConvergeOnAbstractParentWithMediaRegistry()
    {
        var children = new[]
        {
            typeof(IWechatWorkInternalMediaService),
            typeof(IWechatWorkThirdPartyMediaService),
            typeof(IWechatWorkProviderMediaService),
        };

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(typeof(IWechatWorkMediaService),
                $"{child.Name} 必须继承公共父接口 IWechatWorkMediaService");
        }

        var parentApi = typeof(IWechatWorkMediaService).GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var child in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(MediaRegistryGroupName,
                $"{child.Name} 必须挂 {MediaRegistryGroupName} 注册组" +
                $"（Media 模块共用 Add{MediaRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(MediaParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            // 三类应用公共面开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 MD1/MD2。
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 MD2b：服务商上传临时素材域仅第三方应用开放——零端点父接口 + 仅一个子接口承载端点，
    /// 继承链上不得出现其它应用类型子接口（形态对齐微信客服组件域 / 获客助手组件代支付流水族守卫）。
    /// </summary>
    [Fact]
    public void ServiceMediaFamily_ShouldHaveExactlyThirdPartyChildAndAbstractParent()
    {
        var parent = typeof(IWechatWorkServiceMediaService);

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { typeof(IWechatWorkThirdPartyServiceMediaService) },
            "继承链上不得出现第三方之外的子接口：服务商上传临时素材域官方仅向第三方应用开放" +
            "（provider_access_token 鉴权，与三类应用公共面 access_token 路由键不同，独立成族）");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("零端点父接口：端点全部声明于唯一的子接口");

        var childApi = typeof(IWechatWorkThirdPartyServiceMediaService).GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull("子接口必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(MediaRegistryGroupName,
            $"子接口必须挂 {MediaRegistryGroupName} 注册组（与公共面共用 Add{MediaRegistryGroupName}WebApiHttpClient()）");
        childApi.InheritedFrom.Should().Be(ServiceMediaParentImplementationClassName,
            "子接口必须继承父接口生成实现类");
        typeof(IWechatWorkThirdPartyServiceMediaService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().NotBeEmpty("子接口为唯一端点承载接口");
    }

    /// <summary>
    /// 契约守卫 MD3：令牌绑定——素材管理公共面 6 个接口统一消费 AccessToken 路由键并以 Query 注入
    ///（官方契约 access_token）；服务商上传临时素材族 2 个接口消费 ProviderAccessToken 路由键
    ///（官方契约 provider_access_token，Query 注入）。
    /// </summary>
    [Fact]
    public void MediaTokenBinding_ShouldBeAccessAndProviderTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkMediaService),
            typeof(IWechatWorkInternalMediaService),
            typeof(IWechatWorkThirdPartyMediaService),
            typeof(IWechatWorkProviderMediaService),
        };

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }

        var providerTokenInterfaces = new[]
        {
            typeof(IWechatWorkServiceMediaService),
            typeof(IWechatWorkThirdPartyServiceMediaService),
        };

        foreach (var iface in providerTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.ProviderAccessToken,
                $"{iface.Name} 令牌路由键必须为 ProviderAccessToken（服务商级凭证）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("provider_access_token",
                $"{iface.Name} Query 注入参数名必须为官方契约的 provider_access_token");
        }
    }

    /// <summary>
    /// 契约守卫 MD4：素材管理模块的请求/响应 DTO 必须登记进 AOT JSON 上下文（全量 7 个契约面类型；
    /// 获取临时素材 / 获取高清语音素材为二进制端点、无响应 DTO，不入登记面）。
    /// </summary>
    [Fact]
    public void MediaDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = MediaJsonContext.Default;

        var requiredTypes = new[]
        {
            // 上传临时素材（media/upload 与 service/media/upload 共用响应体）。
            typeof(UploadMediaResponse),
            // 上传图片（uploadimg）。
            typeof(UploadImageResponse),
            // 异步上传临时素材（upload_by_url + get_upload_by_url_result）。
            typeof(UploadMediaByUrlRequest), typeof(UploadMediaByUrlResponse),
            typeof(GetUploadMediaByUrlResultRequest), typeof(GetUploadMediaByUrlResultResponse),
            typeof(MediaUploadJobDetail),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是素材管理模块契约面类型，必须登记进 MediaJsonContext（AOT 源生成）");
        }
    }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Media;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 素材管理域契约守卫：路由表、双通道分工（JSON 管线 / 下载独立请求形态）、注册形态、令牌绑定、
/// DTO 字段与 JSON 上下文登记、下载结果信封形态、官方上限常量、错误码常量。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07）：服务端 API 索引 → 素材管理（临时 3 页 + 永久 6 页，其中
/// get_material 双形态落下载通道）。域级约束：临时素材 3 天有效且可复用；素材库上限图文/图片 100000、
/// 其他 1000；batchget count ≤ 20；uploadimg 仅 jpg/png 且 ≤ 1MB。
/// </remarks>
public class MpMediaContractGuards
{
    private const string MediaRegistryGroupName = "Media";

    /// <summary>素材域 JSON 通道（生成管线）官方路由表（临时上传 1 + 永久 4 + uploadimg 1 = 6 端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] MediaRoutes =
    {
        (typeof(IMpMediaService), nameof(IMpMediaService.UploadTempMediaAsync), typeof(PostAttribute), "/cgi-bin/media/upload"),
        (typeof(IMpMediaService), nameof(IMpMediaService.AddMaterialAsync), typeof(PostAttribute), "/cgi-bin/material/add_material"),
        (typeof(IMpMediaService), nameof(IMpMediaService.GetMaterialCountAsync), typeof(GetAttribute), "/cgi-bin/material/get_materialcount"),
        (typeof(IMpMediaService), nameof(IMpMediaService.BatchGetMaterialAsync), typeof(PostAttribute), "/cgi-bin/material/batchget_material"),
        (typeof(IMpMediaService), nameof(IMpMediaService.DelMaterialAsync), typeof(PostAttribute), "/cgi-bin/material/del_material"),
        (typeof(IMpMediaService), nameof(IMpMediaService.UploadImageAsync), typeof(PostAttribute), "/cgi-bin/media/uploadimg"),
        (typeof(IMpMediaService), nameof(IMpMediaService.UploadNewsAsync), typeof(PostAttribute), "/cgi-bin/media/uploadnews"),
        (typeof(IMpMediaService), nameof(IMpMediaService.UploadVideoAsync), typeof(PostAttribute), "/cgi-bin/media/uploadvideo"),
    };

    /// <summary>
    /// 契约守卫 MD1：JSON 通道路由与官方契约一致；下载通道路由经常量锁定（下载不在生成管线上）。
    /// </summary>
    [Fact]
    public void MediaEndpoints_ShouldMatchOfficialRoutes()
    {
        MediaRoutes.Should().HaveCount(8, "临时上传 1 + 永久上传/计数/列表/删除 4 + uploadimg 1 + 群发前置 uploadnews/uploadvideo 2；get_material 在下载通道");
        MediaRoutes.Select(r => r.Route).Distinct().Should().HaveCount(8, "各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in MediaRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方反直觉点锁定：get_materialcount 为 GET（唯一无请求体的查询端点），其余 7 个为 POST。
        MediaRoutes.Count(r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(1,
            "仅 get_materialcount 为 GET（官方原文）");

        // uploadimg 官方同时列于「群发消息」与「永久素材」分组 ⇒ 只建模一次（本域，群发域交叉引用）。
        MediaRoutes.Count(r => r.Route == "/cgi-bin/media/uploadimg").Should().Be(1,
            "uploadimg 只建模一次，不得在群发域重复声明");

        // 下载通道路由锁定：media/get、media/get/jssdk 均为 GET，get_material 为 POST（官方契约）。
        MpMediaDownloadService.TemporaryMediaPath.Should().Be("/cgi-bin/media/get", "官方获取临时素材路径");
        MpMediaDownloadService.JssdkVoicePath.Should().Be("/cgi-bin/media/get/jssdk", "官方获取高清语音素材路径");
        MpMediaDownloadService.PermanentMaterialPath.Should().Be("/cgi-bin/material/get_material", "官方获取永久素材路径");
    }

    /// <summary>契约守卫 MD2：双通道分工——下载端点不得进入生成管线（返回形态必须为下载信封）。</summary>
    [Fact]
    public void DownloadChannel_ShouldStayOutOfGeneratedPipeline()
    {
        // 生成管线接口不得出现返回 Stream / 下载信封的端点（三个下载端点都在下载服务上）。
        var generatedMethods = typeof(IMpMediaService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        generatedMethods.Select(m => m.ReturnType)
            .Should().NotContain(t => t == typeof(Stream) || t == typeof(MpMediaDownloadResult) || t == typeof(MpPermanentMaterialResult),
                "下载端点走 IMpMediaDownloadService 独立请求形态（I3），生成管线仅承载 JSON 响应端点");

        var download = typeof(IMpMediaDownloadService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        download.Should().HaveCount(3, "下载通道 = media/get + media/get/jssdk + material/get_material 三端点");
        download.Where(m => m.Name.StartsWith("Download", StringComparison.Ordinal))
            .Should().OnlyContain(m => m.ReturnType == typeof(Task<MpMediaDownloadResult>),
                "临时素材下载端点统一返回下载结果信封（文件流 / video_url 双形态）");
        download.Where(m => m.Name == nameof(IMpMediaDownloadService.GetPermanentMaterialAsync))
            .Should().OnlyContain(m => m.ReturnType == typeof(Task<MpPermanentMaterialResult>),
                "永久素材端点返回三形态信封（图文 news_item / 视频 down_url / 文件流）");

        // 下载结果信封形态字段。
        typeof(MpMediaDownloadResult).GetProperty(nameof(MpMediaDownloadResult.Content)).Should().NotBeNull();
        typeof(MpMediaDownloadResult).GetProperty(nameof(MpMediaDownloadResult.VideoUrl)).Should().NotBeNull();
        typeof(MpMediaDownloadResult).GetProperty(nameof(MpMediaDownloadResult.ContentType)).Should().NotBeNull();
        typeof(MpMediaDownloadResult).GetProperty(nameof(MpMediaDownloadResult.FileName)).Should().NotBeNull();
        typeof(MpMediaDownloadResult).Should().BeAssignableTo<IDisposable>(
            "内容流绑定底层响应报文，调用方用毕须释放（SDK 不做本地落盘）");

        typeof(MpPermanentMaterialResult).GetProperty(nameof(MpPermanentMaterialResult.NewsItems)).Should().NotBeNull();
        typeof(MpPermanentMaterialResult).GetProperty(nameof(MpPermanentMaterialResult.VideoDownUrl)).Should().NotBeNull();
        typeof(MpPermanentMaterialResult).GetProperty(nameof(MpPermanentMaterialResult.Content)).Should().NotBeNull();
        typeof(MpPermanentMaterialResult).Should().BeAssignableTo<IDisposable>();
    }

    /// <summary>契约守卫 MD3：注册形态——上传接口自身即注册接口，无应用类型子接口。</summary>
    [Fact]
    public void MediaInterfaceHierarchy_ShouldRegisterDirectly()
    {
        var iface = typeof(IMpMediaService);
        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();

        api.Should().NotBeNull("素材域上传接口必须声明 [HttpClientApi]");
        api!.IsAbstract.Should().BeFalse("公众号无应用类型分化 ⇒ 本接口直接作注册接口");
        api.RegistryGroupName.Should().Be(MediaRegistryGroupName, "必须挂 Media 注册组（AddMediaWebApiHttpClient()）");
        api.TokenManage.Should().Be(nameof(IMpAppManager));

        iface.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != iface && iface.IsAssignableFrom(t))
            .Should().BeEmpty("素材域不得出现应用类型子接口");
    }

    /// <summary>契约守卫 MD4：令牌绑定——统一 AccessToken 路由键 + Query 注入（含下载通道的显式注入）。</summary>
    [Fact]
    public void MediaTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var token = typeof(IMpMediaService).GetCustomAttribute<TokenAttribute>();

        token.Should().NotBeNull();
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        // 下载服务无 [Token] 特性（手工注入），但请求 URL 同样以 access_token Query 承载——
        // 由 MpMediaDownloadService.BuildRequestUri 实现锁定（MUD005 同源风险，脱敏词表已覆盖）。
        typeof(IMpMediaDownloadService).GetCustomAttribute<TokenAttribute>().Should().BeNull(
            "下载通道走独立请求形态，不进声明式 [Token] 管线（注入在实现内显式完成）");
    }

    /// <summary>契约守卫 MD5：素材域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void MediaDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = MediaJsonContext.Default;

        var domainTypes = typeof(MpUploadTempMediaResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Media"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 15;
        domainTypes.Should().HaveCount(expectedCount,
            "素材域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（1 上传临时 + 1 永久上传响应 + 1 视频描述 + 1 永久素材响应 + 1 图文条目 + 1 计数响应 +" +
            " 2 列表请求/响应 + 1 列表条目 + 1 图文容器 + 1 media_id 共用请求 + 1 uploadimg 响应 +" +
            " B2a：1 uploadnews 请求 + 1 群发图文条目 + 1 uploadvideo 请求；响应共用上传响应）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于素材域命名空间，必须登记进 MediaJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(MediaRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Media");
        }
    }

    /// <summary>契约守卫 MD6：官方字段名与共用 DTO 裁决锁定。</summary>
    [Fact]
    public void MediaDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        AssertJsonProperty<MpUploadTempMediaResponse>("type", "媒体文件类型（image/voice/video/thumb）");
        AssertJsonProperty<MpUploadTempMediaResponse>("media_id", "媒体文件唯一标识（3 天内有效且可复用）");
        AssertJsonProperty<MpUploadTempMediaResponse>("created_at", "上传时间戳（秒级 Unix 时间）");

        AssertJsonProperty<MpAddMaterialResponse>("media_id", "新增的永久素材 media_id");
        AssertJsonProperty<MpAddMaterialResponse>("url", "图片素材 URL（仅图片返回；腾讯系域名内可用）");

        AssertJsonProperty<MpMaterialDescription>("title", "视频描述标题");
        AssertJsonProperty<MpMaterialDescription>("introduction", "视频描述简介");

        // 永久素材响应 = 图文/视频两形态字段超集（image/voice 为二进制，不在此 DTO）。
        AssertJsonProperty<MpPermanentMaterialResponse>("news_item", "图文形态字段");
        AssertJsonProperty<MpPermanentMaterialResponse>("title", "视频形态字段");
        AssertJsonProperty<MpPermanentMaterialResponse>("description", "视频形态字段");
        AssertJsonProperty<MpPermanentMaterialResponse>("down_url", "视频形态字段");

        // get_material 与 batchget_material 两页 news_item 字段表主体一致 ⇒ 共用条目 DTO。
        AssertJsonProperty<MpMaterialNewsItem>("title", "图文标题");
        AssertJsonProperty<MpMaterialNewsItem>("thumb_media_id", "封面素材 id（必须永久 media_id）");
        AssertJsonProperty<MpMaterialNewsItem>("show_cover_pic", "是否显示封面");
        AssertJsonProperty<MpMaterialNewsItem>("author", "作者");
        AssertJsonProperty<MpMaterialNewsItem>("digest", "摘要（仅单图文有）");
        AssertJsonProperty<MpMaterialNewsItem>("content", "正文（<2 万字符、<1M、去除 JS）");
        AssertJsonProperty<MpMaterialNewsItem>("content_source_url", "原文地址");
        AssertJsonProperty<MpMaterialNewsItem>("url", "图文页 URL");
        AssertJsonProperty<MpMaterialNewsItem>("thumb_url", "仅列表页出现；官方描述原文疑有误（照录）");

        AssertJsonProperty<MpBatchGetMaterialRequest>("type", "素材类型");
        AssertJsonProperty<MpBatchGetMaterialRequest>("offset", "偏移位置（0 起）");
        AssertJsonProperty<MpBatchGetMaterialRequest>("count", "返回数量（1~20）");
        AssertJsonProperty<MpBatchGetMaterialResponse>("total_count", "该类型素材总数");
        AssertJsonProperty<MpBatchGetMaterialResponse>("item_count", "本次获取数量");
        AssertJsonProperty<MpBatchGetMaterialResponse>("item", "素材条目列表");

        // 列表条目 = news 与 image/voice/video 两形态字段超集（官方同字段表声明两形态）。
        AssertJsonProperty<MpMaterialListItem>("media_id", "素材 id");
        AssertJsonProperty<MpMaterialListItem>("update_time", "更新时间");
        AssertJsonProperty<MpMaterialListItem>("name", "仅 image/voice/video 形态");
        AssertJsonProperty<MpMaterialListItem>("url", "仅 image/voice/video 形态");
        AssertJsonProperty<MpMaterialListItem>("content", "仅 news 形态");
        AssertJsonProperty<MpMaterialNewsContent>("news_item", "图文容器字段");

        AssertJsonProperty<MpGetMaterialCountResponse>("voice_count", "语音总数（上限 1000）");
        AssertJsonProperty<MpGetMaterialCountResponse>("video_count", "视频总数（上限 1000）");
        AssertJsonProperty<MpGetMaterialCountResponse>("image_count", "图片总数（上限 100000）");
        AssertJsonProperty<MpGetMaterialCountResponse>("news_count", "图文总数（上限 100000）");

        // get_material 与 del_material 请求体字段表一致 ⇒ 必须共用（两份声明会漂移）。
        AssertJsonProperty<MpMediaIdRequest>("media_id", "按 media_id 寻址（获取/删除共用）");
        typeof(IMpMediaService).GetMethod(nameof(IMpMediaService.DelMaterialAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(MpMediaIdRequest));

        AssertJsonProperty<MpUploadImageResponse>("url", "图片 URL（非 media_id）");

        // 媒体类型词表（官方 type 参数取值）。
        MpMediaTypes.Image.Should().Be("image");
        MpMediaTypes.Voice.Should().Be("voice");
        MpMediaTypes.Video.Should().Be("video");
        MpMediaTypes.Thumb.Should().Be("thumb");
        MpMediaTypes.News.Should().Be("news");
    }

    /// <summary>契约守卫 MD7：上传端点参数位置——type 在 Query（官方契约），文件在 multipart 表单。</summary>
    [Fact]
    public void UploadEndpoints_ShouldTakeTypeViaQueryAndFileViaMultipart()
    {
        var uploadTemp = typeof(IMpMediaService).GetMethod(nameof(IMpMediaService.UploadTempMediaAsync))!;
        uploadTemp.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name)
            .Should().BeEquivalentTo(new[] { "type" }, "官方契约：type 为 Query 参数（access_token 由 [Token] 注入）");
        uploadTemp.GetParameters().Should().Contain(p => p.GetCustomAttribute<MultipartFormAttribute>() != null,
            "文件经 multipart 表单（字段名 media）上传");

        var addMaterial = typeof(IMpMediaService).GetMethod(nameof(IMpMediaService.AddMaterialAsync))!;
        addMaterial.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name)
            .Should().BeEquivalentTo(new[] { "type" }, "永久上传同样以 Query 传 type");
        var descriptionParam = addMaterial.GetParameters()
            .SingleOrDefault(p => p.GetCustomAttribute<FormAttribute>() != null);
        descriptionParam.Should().NotBeNull("视频素材的 description 为 multipart 表单字段");
        descriptionParam!.GetCustomAttribute<FormAttribute>()!.FieldName.Should().Be("description",
            "官方表单字段名为 description（JSON 字符串形态）");

        var uploadImage = typeof(IMpMediaService).GetMethod(nameof(IMpMediaService.UploadImageAsync))!;
        uploadImage.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>())
            .Should().BeEmpty("官方 uploadimg 契约无 type 参数（仅 access_token Query）");
        uploadImage.GetParameters().Should().Contain(p => p.GetCustomAttribute<MultipartFormAttribute>() != null);
    }

    /// <summary>契约守卫 MD8：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void MediaModule_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("Media");
        typeof(MpServiceBuilder).GetMethod("AddMediaApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpMediaService),
            "上传通道必须在内；全量白名单由 MpQueryTokenWhitelistGuard（QT1）单点持有");
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");
    }

    /// <summary>契约守卫 MD9：素材域已核验错误码常量锁定。</summary>
    [Fact]
    public void MediaErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.InvalidMediaType.Should().Be(40004);
        MpErrorCodes.InvalidMediaId.Should().Be(40007);
        MpErrorCodes.InvalidImageSize.Should().Be(40009);
    }

    /// <summary>
    /// 契约守卫 MD10：下载通道的 JSON 解释必须走 <b>源生成 JsonTypeInfo 快车道</b>
    /// （AOT 净零锚点）——<c>MediaJsonContext.Default</c> 必须为永久素材响应与 media_id 请求体
    /// 暴露强类型元数据；缺失即意味着 <c>MpMediaDownloadService</c> 退回反射路径
    /// （AGENTS §3 红线：禁反射版 Serialize/Deserialize&lt;T&gt;）。
    /// </summary>
    /// <remarks>
    /// 快车道形态（<c>IAotJsonContentSerializer</c> + <c>MediaJsonContext</c>）与 AOT/裁剪分析同属
    /// <b>net8.0+</b>：低 TFM（netstandard2.0 / net6.0）不启用裁剪/AOT 分析器，SDK 走 options 路径。
    /// 本守卫在 <c>net8.0</c> 单 TFM 测试工程内执行 ⇒ 断言的正是启用 AOT 门禁的那条路径。
    /// </remarks>
    [Fact]
    public void DownloadChannel_ShouldUseSourceGeneratedJsonTypeInfo()
    {
        MediaJsonContext.Default.MpPermanentMaterialResponse.Should().NotBeNull(
            "get_material 的图文 news_item / 视频 down_url 统一响应体必须登记进 MediaJsonContext（下载通道 AOT 快车道锚点）");
        MediaJsonContext.Default.MpMediaIdRequest.Should().NotBeNull(
            "get_material 的 {\"media_id\":…} 请求体必须登记进 MediaJsonContext（下载通道 AOT 快车道锚点）");

        // 默认组件序列化器实现 IAotJsonContentSerializer ⇒ 快车道在标准装配下即可生效（非仅理论路径）。
        HttpContentSerializerFactory.CreateDefault().Should().BeAssignableTo<IAotJsonContentSerializer>(
            "组件默认序列化器须实现 IAotJsonContentSerializer，否则下载通道永远退回 options 解析路径");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Resource;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「资源管理」域契约守卫（CR 系列：Resource 域 7 端点 / 8 方法路由 + 同路由双传输形态 +
/// multipart 与二进制响应通道 + 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>端点 / 方法计数口径</b>：官方 8 端点，其中图片上传（<c>img_upload</c>）与图片上传 V2
/// （<c>img_uploadv2</c>）各给出按 <c>upload_type</c> 互斥的两种请求体传输形态（<c>upload_type=1</c> JSON 包 <c>img_url</c> /
/// <c>upload_type=0</c> multipart 文件 <c>media</c>）⇒ SDK 各以两个方法承载（对齐 Work 线
/// message/send「同路由多方法」先例），故方法数 10 &gt; 路由数 8。
/// </para>
/// <para>
/// <b>路由前缀并存（官方历史前缀，照抄原文禁止改写）</b>：<c>/shop/ec/basics/*</c>（7 路由）与
/// <c>/channels/ec/basics/*</c>（1 路由：media/get）。
/// </para>
/// <para>
/// <b>非 JSON 通道</b>：媒体取数（media/get）成功响应为二进制（<c>byte[]?</c> 直读，对齐 Work 线
/// media/get 形态）；资质图片 / 图片二进制 / 视频分块为 <c>[MultipartForm]</c> 通道（对齐公众号线素材域形态）。
/// </para>
/// </remarks>
public class ChannelsResourceContractGuards
{
    private const string ResourceRegistryGroupName = "Resource";

    /// <summary>Resource 域官方路由表（8 端点 / 10 方法：/shop/ec/basics/* 9 方法 + /channels/ec/basics/* 1 方法）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ResourceRoutes =
    {
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.UploadImageAsync), typeof(PostAttribute), "/shop/ec/basics/img/upload"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.UploadImageByBinaryAsync), typeof(PostAttribute), "/shop/ec/basics/img/upload"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.UploadImageV2Async), typeof(PostAttribute), "/shop/ec/basics/img/uploadv2"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.UploadImageV2ByBinaryAsync), typeof(PostAttribute), "/shop/ec/basics/img/uploadv2"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.GetDataByMediaIdAsync), typeof(GetAttribute), "/channels/ec/basics/media/get"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.UploadQualificationAsync), typeof(PostAttribute), "/shop/ec/basics/qualification/upload"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.InitVideoUploadAsync), typeof(PostAttribute), "/shop/ec/basics/video/initupload"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.UploadVideoPartAsync), typeof(PostAttribute), "/shop/ec/basics/video/uploadpart"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.FinishVideoUploadAsync), typeof(PostAttribute), "/shop/ec/basics/video/finishupload"),
        (typeof(IChannelsResourceService), nameof(IChannelsResourceService.GetVideoPlayInfoAsync), typeof(GetAttribute), "/shop/ec/basics/video/getplayinfo"),
    };

    /// <summary>契约守卫 CR1：Resource 域路由必须与官方契约一致（8 端点 / 10 方法，img_upload 与 img_uploadv2 各双传输形态）。</summary>
    [Fact]
    public void ResourceEndpoints_ShouldMatchOfficialRoutes()
    {
        // 10 方法 = 图片上传双形态 2 + 图片上传 V2 双形态 2 + media 取数 1 + 资质图片 1 + 视频四步流程 4。
        ResourceRoutes.Should().HaveCount(10, "官方 8 端点中 img_upload / img_uploadv2 各有两种互斥请求体传输形态，各以两个方法承载");
        ResourceRoutes.Select(r => r.Route).Distinct().Should().HaveCount(8,
            "图片上传两方法共用 /shop/ec/basics/img/upload、V2 两方法共用 /shop/ec/basics/img/uploadv2");

        ResourceRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/shop/ec/basics/", StringComparison.Ordinal))
            .Should().Be(9, "img_upload ×2 + img_uploadv2 ×2 + 资质图片 1 + 视频流程 4 共 9 个方法走 /shop/ec/basics/* 官方历史前缀（照抄原文禁止改写）");
        ResourceRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/basics/", StringComparison.Ordinal))
            .Should().Be(1, "仅 media/get 走 /channels/ec/basics/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in ResourceRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CR2：动词如实（8 POST + 2 GET）；JSON 端点带 [Body]、multipart 端点带 [MultipartForm]、无请求体端点不得带 [Body]。</summary>
    [Fact]
    public void ResourceEndpoints_ShouldHonorOfficialBodyRules()
    {
        // 动词分布：官方页如实——media/get 与 video/getplayinfo 为 GET，其余 8 方法 POST。
        ResourceRoutes.Count(static r => r.HttpAttribute == typeof(PostAttribute)).Should().Be(8);
        ResourceRoutes.Count(static r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(2);

        // JSON 请求体端点必须带 [Body]。
        foreach (var bodyEndpoints in new[]
                 {
                     nameof(IChannelsResourceService.UploadImageAsync),
                     nameof(IChannelsResourceService.UploadImageV2Async),
                     nameof(IChannelsResourceService.InitVideoUploadAsync),
                     nameof(IChannelsResourceService.FinishVideoUploadAsync),
                 })
        {
            var target = typeof(IChannelsResourceService).GetMethod(bodyEndpoints)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().NotBeEmpty($"{bodyEndpoints} 官方契约为 POST JSON 请求体——必须声明 [Body]");
        }

        // multipart 端点必须带 [MultipartForm] 且不得带 [Body]（官方请求体为 formdata 文件 media，非 JSON）。
        foreach (var multipartEndpoints in new[]
                 {
                     nameof(IChannelsResourceService.UploadImageByBinaryAsync),
                     nameof(IChannelsResourceService.UploadImageV2ByBinaryAsync),
                     nameof(IChannelsResourceService.UploadQualificationAsync),
                     nameof(IChannelsResourceService.UploadVideoPartAsync),
                 })
        {
            var target = typeof(IChannelsResourceService).GetMethod(multipartEndpoints)!;
            target.GetParameters().Should().Contain(
                static p => p.GetCustomAttribute<MultipartFormAttribute>() != null,
                $"{multipartEndpoints} 官方契约为 multipart/form-data 文件 media——必须声明 [MultipartForm]");
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{multipartEndpoints} 官方请求体为 formdata 文件（非 JSON）——不得声明 [Body]");
        }

        // 无请求体端点（GET media/get、GET video/getplayinfo）不得带 [Body]。
        foreach (var emptyEndpoints in new[]
                 {
                     nameof(IChannelsResourceService.GetDataByMediaIdAsync),
                     nameof(IChannelsResourceService.GetVideoPlayInfoAsync),
                 })
        {
            var target = typeof(IChannelsResourceService).GetMethod(emptyEndpoints)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{emptyEndpoints} 官方参数表无 Body 参数——不得声明 [Body]");
        }

        // 二进制响应端点：media/get 官方「成功时返回二进制数据」——返回类型必须为 byte[]?（对齐 Work 线 media/get 形态）。
        typeof(IChannelsResourceService).GetMethod(nameof(IChannelsResourceService.GetDataByMediaIdAsync))!
            .ReturnType.Should().Be(typeof(Task<byte[]?>),
                "media/get 成功时返回二进制数据、报错时返回 JSON——SDK 以 byte[]? 直读原始响应");

        // V1 与 V2 的 URL 参数面差异（官方参数表如实，不得互相「对齐」补参）：
        //   V1 = upload_type + resp_type（二进制模式再加 height/width）；V2 = scene_type + upload_type（二进制模式无 height/width）。
        QueryNamesOf(nameof(IChannelsResourceService.UploadImageAsync))
            .Should().Equal(new[] { "upload_type", "resp_type" }, "V1 URL 模式官方参数表为 upload_type + resp_type");
        QueryNamesOf(nameof(IChannelsResourceService.UploadImageByBinaryAsync))
            .Should().Equal(new[] { "upload_type", "resp_type", "height", "width" },
                "V1 二进制模式官方额外要求 height/width（URL 参数、upload_type=0 时必填）");
        QueryNamesOf(nameof(IChannelsResourceService.UploadImageV2Async))
            .Should().Equal(new[] { "scene_type", "upload_type" },
                "V2 官方参数表为 scene_type + upload_type，无 resp_type（响应只有图片链接）");
        QueryNamesOf(nameof(IChannelsResourceService.UploadImageV2ByBinaryAsync))
            .Should().Equal(new[] { "scene_type", "upload_type" },
                "V2 二进制模式官方参数表同样无 height/width——若照 V1 补参即偏离官方契约");
    }

    /// <summary>取指定方法的 [Query] 参数名（按签名顺序）。</summary>
    private static string[] QueryNamesOf(string methodName)
        => typeof(IChannelsResourceService).GetMethod(methodName)!
            .GetParameters()
            .Select(p => p.GetCustomAttribute<QueryAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    /// <summary>契约守卫 CR3：令牌绑定与注册形态。</summary>
    [Fact]
    public void ResourceInterface_ShouldBindChannelsToken_AndRegisterUnderResourceGroup()
    {
        var token = typeof(IChannelsResourceService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Resource 域端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsResourceService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(ResourceRegistryGroupName, "必须挂 Resource 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Resource");
        typeof(ChannelsServiceBuilder).GetMethod("AddResourceApi").Should().NotBeNull("必须提供 AddResourceApi 链式注册方法");
    }

    /// <summary>契约守卫 CR4：Resource 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void ResourceDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = ResourceJsonContext.Default;

        var domainTypes = typeof(ChannelsResourceImgUploadResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Resource"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(17,
            "Resource 域 8 端点（10 方法）的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（图片上传 3 + 图片上传 V2 3 + 资质图片 2 + 视频申请 3 + 视频分块 2 + 视频完成 2 + 视频临时 URL 2 = 17）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Resource 域命名空间，必须登记进 ResourceJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(ResourceRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Resource");
        }

        // 常量类断言计数且未标序列化特性（标了会进上下文并使上方计数失真）。
        var constantTypes = domainTypes.Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null).ToList();
        constantTypes.Should().HaveCount(4,
            "Resource 域常量类 = 图片上传类型 + 图片返回类型 + 图片上传 V2 场景类型 + 视频上传场景类型（枚举常量类不标 [HttpJsonSerializable]）");
    }

    /// <summary>契约守卫 CR5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 同路由双形态共用响应）。</summary>
    [Fact]
    public void ResourceDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 图片上传（img_upload）。
        AssertJsonProperty<ChannelsResourceImgUploadRequest>("img_url", "图片 URL（URL 模式请求体）");
        AssertJsonProperty<ChannelsResourceImgUploadResponse>("pic_file", "图片信息");
        AssertJsonProperty<ChannelsResourcePicFile>("media_id", "media_id");
        AssertJsonProperty<ChannelsResourcePicFile>("pay_media_id", "支付 media_id");
        AssertJsonProperty<ChannelsResourcePicFile>("img_url", "图片链接（resp_type=1）");

        // 图片上传 V2（img_uploadv2，官方 2026-10-10 新增）。
        AssertJsonProperty<ChannelsResourceImgUploadV2Request>("img_url", "图片 URL（URL 模式请求体）");
        AssertJsonProperty<ChannelsResourceImgUploadV2Response>("img_info", "图片信息（V2 的包装对象名）");
        AssertJsonProperty<ChannelsResourceImgUploadV2ImgInfo>("img_url", "图片链接（V2 唯一返回形态）");

        // 资质图片（qualificationupload）。
        AssertJsonProperty<ChannelsResourceQualificationUploadResponse>("data", "文件信息");
        AssertJsonProperty<ChannelsResourceQualificationFile>("file_id", "文件 file_id");

        // 视频申请上传（video_initupload）。
        AssertJsonProperty<ChannelsResourceVideoInitUploadRequest>("scene_type", "上传场景类型");
        AssertJsonProperty<ChannelsResourceVideoInitUploadRequest>("file_type", "文件类型");
        AssertJsonProperty<ChannelsResourceVideoInitUploadRequest>("file_size", "文件大小（字节）");
        AssertJsonProperty<ChannelsResourceVideoInitUploadResponse>("data", "上传流程信息");
        AssertJsonProperty<ChannelsResourceVideoUploadKey>("video_upload_key", "上传流程关联键");

        // 视频分块（video_uploadpart）。
        AssertJsonProperty<ChannelsResourceVideoUploadPartResponse>("data", "分片信息");
        AssertJsonProperty<ChannelsResourceVideoPartSha>("part_sha", "服务端计算的分片 SHA1");

        // 视频完成（video_finishupload）。
        AssertJsonProperty<ChannelsResourceVideoFinishUploadRequest>("video_upload_key", "上传流程关联键");
        AssertJsonProperty<ChannelsResourceVideoFinishUploadRequest>("finish_parts", "文件分片列表");
        AssertJsonProperty<ChannelsResourceVideoFinishPart>("partnum", "分块编号");
        AssertJsonProperty<ChannelsResourceVideoFinishPart>("part_sha", "分片 SHA1");

        // 视频临时 URL（video_getplayinfo）。
        AssertJsonProperty<ChannelsResourceVideoPlayInfoResponse>("data", "播放信息");
        AssertJsonProperty<ChannelsResourceVideoPlayInfo>("url", "视频临时 URL");

        // 共用 DTO 裁决：img_upload 的 URL 模式与二进制流模式返回结构一致（官方同一响应参数表）
        // ⇒ 同路由两方法共用 ChannelsResourceImgUploadResponse，不建第二份响应 DTO。
        typeof(IChannelsResourceService).GetMethod(nameof(IChannelsResourceService.UploadImageAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsResourceImgUploadResponse>));
        typeof(IChannelsResourceService).GetMethod(nameof(IChannelsResourceService.UploadImageByBinaryAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsResourceImgUploadResponse>));

        // 共用裁决（负向）：finishupload 官方响应无业务字段 ⇒ 返回基线 ChannelsResponse，不造空壳 DTO。
        typeof(IChannelsResourceService).GetMethod(nameof(IChannelsResourceService.FinishVideoUploadAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsResponse>));

        // V1 / V2 响应**不得共用**：V1 为 pic_file{media_id, pay_media_id, img_url}（resp_type 决定形态），
        // V2 只有 img_info{img_url}——包装对象名与子字段集都不同，合并即静默错读 ⇒ 分建 DTO。
        var v2Url = typeof(IChannelsResourceService).GetMethod(nameof(IChannelsResourceService.UploadImageV2Async))!;
        var v2Binary = typeof(IChannelsResourceService).GetMethod(nameof(IChannelsResourceService.UploadImageV2ByBinaryAsync))!;
        v2Url.ReturnType.Should().Be(typeof(Task<ChannelsResourceImgUploadV2Response>),
            "V2 两种传输形态共用同一响应 DTO（官方同一响应参数表）");
        v2Binary.ReturnType.Should().Be(v2Url.ReturnType,
            "V2 二进制模式与 URL 模式响应结构一致 ⇒ 不得分建");
        v2Url.ReturnType.Should().NotBe(typeof(Task<ChannelsResourceImgUploadResponse>),
            "V2 响应包装对象名是 img_info 而非 pic_file ⇒ 不复用 V1 响应");
        // 判据取 DeclaredOnly：响应继承 ChannelsResponse（errcode/errmsg 为基类成员），本域新增成员只有 img_info 一个。
        typeof(ChannelsResourceImgUploadV2Response).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(1, "V2 响应只有 img_info 一个业务字段（无 media_id/pay_media_id）");
        typeof(ChannelsResourceImgUploadV2ImgInfo).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Should().HaveCount(1, "img_info 只有 img_url 一个字段");
        JsonPropertyNamesOf<ChannelsResourceImgUploadV2Response>()
            .Should().NotContain(new[] { "media_id", "pay_media_id" },
                "V2 无 resp_type 参数 ⇒ 不返回 media_id/pay_media_id，不得照 V1 的 pic_file 补字段");
        v2Url.GetParameters().Select(static p => p.ParameterType)
            .Should().Contain(typeof(ChannelsResourceImgUploadV2Request),
                "V2 请求体 DTO 独立建模，不复用 V1 的 ChannelsResourceImgUploadRequest");
    }

    /// <summary>契约守卫 CR6：枚举常量字面量锁定（官方 upload_type / resp_type / scene_type）。</summary>
    [Fact]
    public void ResourceEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsResourceImgUploadTypes.BinaryStream.Should().Be(0);
        ChannelsResourceImgUploadTypes.Url.Should().Be(1);

        ChannelsResourceImgRespTypes.MediaId.Should().Be(0);
        ChannelsResourceImgRespTypes.Url.Should().Be(1);

        ChannelsResourceVideoSceneTypes.Aftersale.Should().Be(173);
        ChannelsResourceVideoSceneTypes.Product.Should().Be(162);

        // V2 场景类型：官方 img_uploadv2 页枚举表当前仅列 170（售后相关）。
        ChannelsResourceImgUploadV2SceneTypes.Aftersale.Should().Be(170, "官方 Query.scene_type 枚举值 170=售后相关");

        // 与视频上传场景类型分建（值集不同 ⇒ 合并即静默错读）：170 属于 V2 图片面，173/162 属于视频面。
        ConstValuesOf(typeof(ChannelsResourceImgUploadV2SceneTypes))
            .Should().Equal(new[] { 170 }, "img_uploadv2 官方枚举表只有 170 一值");
        ConstValuesOf(typeof(ChannelsResourceVideoSceneTypes))
            .Should().Equal(new[] { 162, 173 }, "video_initupload 官方枚举表为 162 / 173");
        ConstValuesOf(typeof(ChannelsResourceImgUploadV2SceneTypes))
            .Should().NotBeEquivalentTo(ConstValuesOf(typeof(ChannelsResourceVideoSceneTypes)),
                "两页 scene_type 是两张表（图片 V2 与视频上传场景不同集），不得合并常量类");

        // upload_type 两页同值同义（0 二进制流 / 1 图片 URL）⇒ 复用 ChannelsResourceImgUploadTypes，不建第二份。
        typeof(ChannelsResourceImgUploadV2SceneTypes).Assembly
            .GetType("Mud.Wechat.Channels.DataModels.Resource.ChannelsResourceImgUploadV2UploadTypes")
            .Should().BeNull("V2 的 upload_type 复用 V1 常量类，不得重名分建");
    }

    /// <summary>取静态常量类的 public const int 值集（按数值升序）。</summary>
    private static int[] ConstValuesOf(Type owner)
        => owner.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral && f.FieldType == typeof(int))
            .Select(static f => (int)f.GetRawConstantValue()!)
            .OrderBy(static v => v)
            .ToArray();

    /// <summary>
    /// 契约守卫 CR7：缺失端点清单转述的 <c>/channels/ec/basics/*</c> 六支图片 / 视频路由在全线零声明。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 2026-10-17 核验：清单（DotNetCore.SKIT.FlurlHttpClient.Wechat 转述）把资源管理六支写成
    /// <c>/channels/ec/basics/{img/upload, qualification/upload, video/initupload, video/uploadpart,
    /// video/finishupload, video/getplayinfo}</c>；而官方 <c>doc/channels/API/basics/*</c> 六页已全部
    /// 重定向到 <c>doc/store/shop/API/apimgnt/</c> 目录页，该目录页的「资源管理」接口列表给出的请求路径
    /// 逐支都是 <c>/shop/ec/basics/*</c>（同页仅「通过 mediaid 获取数据」为 <c>/channels/ec/basics/media/get</c>）
    /// ⇒ 六支 <c>/channels/ec/*</c> 形态属 SKIT 侧历史前缀转述，<b>非官方契约</b>。
    /// </para>
    /// <para>
    /// 本线按官方原文照抄 <c>/shop/ec/basics/*</c>（设计方案 §4.3「历史前缀禁止对齐改写」），
    /// 故须以零声明守卫钉死「不是漏做，是转述前缀不成立」，并防后续按清单回填。
    /// 同目录页另有 <c>/shop/ec/basics/img/uploadv2</c>（上传图片 V2，2026-10-10 新增）——清单未含（SKIT 未实现），
    /// 本线已于 CR1 路由表落地。
    /// </para>
    /// </remarks>
    [Fact]
    public void StaleChannelsBasicsPrefix_ShouldHaveZeroDeclaration()
    {
        var allRoutes = typeof(IChannelsResourceService).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.Namespace != null && t.Namespace.StartsWith("Mud.Wechat.Channels", StringComparison.Ordinal))
            .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(static m => m.GetCustomAttributes().OfType<HttpMethodAttribute>())
            .Select(static a => a.RequestUri ?? string.Empty)
            .ToList();

        allRoutes.Should().NotBeEmpty("反射枚举不得空跑（正对照）");

        foreach (var stale in new[]
        {
            "/channels/ec/basics/img/upload",
            "/channels/ec/basics/qualification/upload",
            "/channels/ec/basics/video/initupload",
            "/channels/ec/basics/video/uploadpart",
            "/channels/ec/basics/video/finishupload",
            "/channels/ec/basics/video/getplayinfo",
        })
        {
            allRoutes.Count(r => r.Equals(stale, StringComparison.Ordinal))
                .Should().Be(0,
                    $"{stale} 是清单转述的历史前缀：官方 apimgnt 目录页该端点现行请求路径为 {stale.Replace("/channels/ec/", "/shop/ec/")}（2026-10-17 核验）⇒ 非契约路由零声明");
        }

        // 正对照 1：前缀本身在本线合法在用（media/get），故本断言不能靠「整段前缀禁用」恒真；
        // 正对照 2：六支现行 /shop/ec/ 路由在场，证明扫描面覆盖本域接口。
        allRoutes.Should().Contain("/channels/ec/basics/media/get",
            "正对照：/channels/ec/basics/ 前缀确有现行端点，零声明只针对六支转述路由");
        foreach (var landed in new[]
                 {
                     "/shop/ec/basics/img/upload",
                     "/shop/ec/basics/qualification/upload",
                     "/shop/ec/basics/video/initupload",
                     "/shop/ec/basics/video/uploadpart",
                     "/shop/ec/basics/video/finishupload",
                     "/shop/ec/basics/video/getplayinfo",
                 })
        {
            allRoutes.Should().Contain(landed, $"正对照：现行路由 {landed} 必须被扫描面覆盖，否则上方零声明断言恒真");
        }
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }

    /// <summary>取类型（含继承面）的全部 JSON 字段名。</summary>
    private static string[] JsonPropertyNamesOf<T>()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? p.Name)
            .ToArray();
}

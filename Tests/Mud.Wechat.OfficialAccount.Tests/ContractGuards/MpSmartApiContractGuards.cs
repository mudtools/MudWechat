// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.SmartApi;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// P3-a 智能接口域契约守卫（AI 开放接口 3 + OCR 识别 7 + 图像处理 2 = 12 端点 / 21 方法）。
/// </summary>
/// <remarks>
/// 官方事实来源（逐页核验 2026-10-07，12 页全服务号域命中）。关键核验修正：
/// ① 9 个 OCR / 图像处理端点为「form 上传 <c>img</c> 或 Query <c>img_url</c>」二选一互斥形态 ⇒ 双方法建模；
/// ② 菜单识别页<b>无频率上限</b>（不得套用其它 OCR 页的 100 次/天）；
/// ③ 银行卡识别返回字段表 <c>number</c> 与返回示例 <c>id</c> 不一致；行驶证 / 驾驶证返回示例携带字段表未收录字段；
/// ④ 通用印刷体 <c>items[].text</c> 见示例、字段表漏列；⑤ 菜单识别 <c>content</c> 字段表标 object、示例为 JSON 字符串。
/// </remarks>
public class MpSmartApiContractGuards
{
    private const string RegistryGroupName = "SmartApi";

    /// <summary>双形态端点（11 个）：官方「form 上传 img」与「Query img_url」互斥 ⇒ 每端点两方法。</summary>
    private static readonly (string UploadMethod, string UrlMethod, string Route)[] DualFormRoutes =
    {
        (nameof(IMpSmartApiService.IdCardOcrByUploadAsync), nameof(IMpSmartApiService.IdCardOcrByUrlAsync), "/cv/ocr/idcard"),
        (nameof(IMpSmartApiService.BankCardOcrByUploadAsync), nameof(IMpSmartApiService.BankCardOcrByUrlAsync), "/cv/ocr/bankcard"),
        (nameof(IMpSmartApiService.DrivingOcrByUploadAsync), nameof(IMpSmartApiService.DrivingOcrByUrlAsync), "/cv/ocr/driving"),
        (nameof(IMpSmartApiService.DrivingLicenseOcrByUploadAsync), nameof(IMpSmartApiService.DrivingLicenseOcrByUrlAsync), "/cv/ocr/drivinglicense"),
        (nameof(IMpSmartApiService.BizLicenseOcrByUploadAsync), nameof(IMpSmartApiService.BizLicenseOcrByUrlAsync), "/cv/ocr/bizlicense"),
        (nameof(IMpSmartApiService.CommOcrByUploadAsync), nameof(IMpSmartApiService.CommOcrByUrlAsync), "/cv/ocr/comm"),
        (nameof(IMpSmartApiService.MenuOcrByUploadAsync), nameof(IMpSmartApiService.MenuOcrByUrlAsync), "/cv/ocr/menu"),
        (nameof(IMpSmartApiService.AiCropByUploadAsync), nameof(IMpSmartApiService.AiCropByUrlAsync), "/cv/img/aicrop"),
        (nameof(IMpSmartApiService.QrcodeRecognitionByUploadAsync), nameof(IMpSmartApiService.QrcodeRecognitionByUrlAsync), "/cv/img/qrcode"),
        (nameof(IMpSmartApiService.PlateNumberOcrByUploadAsync), nameof(IMpSmartApiService.PlateNumberOcrByUrlAsync), "/cv/ocr/platenum"),
    };

    /// <summary>AI 开放接口路由表（3 端点，全 POST）。</summary>
    private static readonly (string Method, string Route)[] VoiceRoutes =
    {
        (nameof(IMpSmartApiService.UploadVoiceForRecognitionAsync), "/cgi-bin/media/voice/addvoicetorecofortext"),
        (nameof(IMpSmartApiService.QueryVoiceRecognitionResultAsync), "/cgi-bin/media/voice/queryrecoresultfortext"),
        (nameof(IMpSmartApiService.TranslateContentAsync), "/cgi-bin/media/voice/translatecontent"),
    };

    /// <summary>契约守卫 SM1：12 端点路由表与官方契约一致（9 端点双方法 ⇒ 21 个方法全 POST）。</summary>
    [Fact]
    public void SmartApiEndpoints_ShouldMatchOfficialRoutes()
    {
        // 12 条唯一路由（9 双形态 + 3 单形态），互不重复。
        var allRoutes = DualFormRoutes.Select(r => r.Route).Concat(VoiceRoutes.Select(r => r.Route)).ToList();
        allRoutes.Should().HaveCount(13);
        allRoutes.Distinct().Should().HaveCount(13, "各端点路由互不重复");

        // 9 端点 × 2 形态：上传方法带 [MultipartForm]，URL 方法带 Query img_url，且两方法路由一致。
        foreach (var (uploadMethod, urlMethod, route) in DualFormRoutes)
        {
            var upload = FindMethod(uploadMethod);
            var url = FindMethod(urlMethod);

            var uploadAttr = upload.GetCustomAttribute<PostAttribute>();
            uploadAttr.Should().NotBeNull($"{uploadMethod} 必须声明 POST 路由");
            uploadAttr!.RequestUri.Should().Be(route, $"{uploadMethod} 路由必须与官方契约一致");

            var urlAttr = url.GetCustomAttribute<PostAttribute>();
            urlAttr.Should().NotBeNull($"{urlMethod} 必须声明 POST 路由");
            urlAttr!.RequestUri.Should().Be(route, $"{urlMethod} 与 {uploadMethod} 必须同路由（官方同一端点两调用形态）");

            HasMultipart(upload).Should().BeTrue($"{uploadMethod} 必须声明 [MultipartForm] 参数（官方 form 上传 img 形态）");
            HasQuery(url, "img_url").Should().BeTrue($"{urlMethod} 必须声明 [Query(\"img_url\")] 参数（官方 URL 形态）");
            HasMultipart(url).Should().BeFalse($"{urlMethod} 为 URL 形态，不得携带 [MultipartForm]（两形态互斥）");
        }

        // AI 开放接口 3 端点：全 POST，路径前缀 /cgi-bin/media/voice/（官方路径安排，非 /cv/）。
        foreach (var (method, route) in VoiceRoutes)
        {
            var attr = FindMethod(method).GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route);
            route.Should().StartWith("/cgi-bin/media/voice/",
                "AI 开放接口三端点在 /cgi-bin/media/voice/ 前缀下（含微信翻译，官方路径语义异常照抄）");
        }

        // 该域无 GET 端点（12 端点 / 21 方法全 POST）——防「顺手把查询型端点改 GET」。
        var methods = typeof(IMpSmartApiService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
            .ToList();
        methods.Should().HaveCount(23, "10 端点双方法 + 3 单方法 = 23");
        methods.Select(m => m.GetCustomAttribute<HttpMethodAttribute>()!)
            .Should().AllBeAssignableTo<PostAttribute>("智能接口 13 端点官方全部为 POST");
    }

    /// <summary>契约守卫 SM2：AI 语音三端点的参数位置（Query vs 请求体）与官方契约一致。</summary>
    [Fact]
    public void VoiceEndpoints_ShouldKeepOfficialParameterPlacement()
    {
        // 上传语音：format / voice_id / lang 全为 Query + form 上传形态。
        var upload = FindMethod(nameof(IMpSmartApiService.UploadVoiceForRecognitionAsync));
        QueryNames(upload).Should().BeEquivalentTo(new[] { "format", "voice_id", "lang" },
            "官方把 format / voice_id / lang 全部定义为 Query 参数");
        HasMultipart(upload).Should().BeTrue();

        // 获取识别结果：仅 voice_id / lang 两个 Query，且无请求体。
        var query = FindMethod(nameof(IMpSmartApiService.QueryVoiceRecognitionResultAsync));
        QueryNames(query).Should().BeEquivalentTo(new[] { "voice_id", "lang" });
        HasBody(query).Should().BeFalse("官方该端点无请求体");

        // 微信翻译：lfrom / lto 为 Query；content 在请求体内（不得混进 Query）。
        var translate = FindMethod(nameof(IMpSmartApiService.TranslateContentAsync));
        QueryNames(translate).Should().BeEquivalentTo(new[] { "lfrom", "lto" });
        HasBody(translate).Should().BeTrue();
        translate.GetParameters().Single(p => p.GetCustomAttribute<BodyAttribute>() != null)
            .ParameterType.Should().Be(typeof(MpVoiceTranslateRequest));
    }

    /// <summary>契约守卫 SM3：请求 / 响应 DTO 的官方字段名锁定（含核验修正与矛盾照录项）。</summary>
    [Fact]
    public void SmartApiShapes_ShouldLockOfficialFieldNames()
    {
        // 微信翻译：content 在 body、from_content / to_content 在响应；语言枚举仅两值。
        AssertJsonProperty<MpVoiceTranslateRequest>("content", "源内容（utf8，最大 600Byte）");
        AssertJsonProperty<MpVoiceTranslateResponse>("from_content", "原文内容");
        AssertJsonProperty<MpVoiceTranslateResponse>("to_content", "译文内容");
        MpVoiceLanguages.ZhCn.Should().Be("zh_CN");
        MpVoiceLanguages.EnUs.Should().Be("en_US");
        MpVoiceFileFormats.Mp3.Should().Be("mp3", "官方 format 仅给出 mp3 一个枚举值");

        // 语音识别结果：result。
        AssertJsonProperty<MpVoiceRecoResultResponse>("result", "识别结果");

        // 身份证识别：type / name / id / valid_date / addr / gender / nationality。
        AssertJsonProperty<MpOcrIdCardResponse>("type", "Front / Back");
        AssertJsonProperty<MpOcrIdCardResponse>("valid_date", "背面返回的有效期");
        AssertJsonProperty<MpOcrIdCardResponse>("nationality", "正面返回的民族");

        // 银行卡识别：官方字段表 number 与返回示例 id 不一致 ⇒ 双键并存（超集承载）。
        AssertJsonProperty<MpOcrBankCardResponse>("number", "官方字段表字段名");
        AssertJsonProperty<MpOcrBankCardResponse>("id", "官方返回示例实际使用的键");

        // 行驶证识别：字段表 15 项 + 示例独有 4 项。
        AssertJsonProperty<MpOcrDrivingResponse>("plate_num", "车牌号码");
        AssertJsonProperty<MpOcrDrivingResponse>("plate_num_b", "官方说明与 plate_num 完全相同（照录）");
        AssertJsonProperty<MpOcrDrivingResponse>("use_character", "使用性质");
        AssertJsonProperty<MpOcrDrivingResponse>("prepare_quality", "整备质量");
        AssertJsonProperty<MpOcrDrivingResponse>("overall_size", "示例独有（字段表未收录）");
        AssertJsonProperty<MpOcrDrivingResponse>("card_position_front", "示例独有（字段表未收录）");
        AssertJsonProperty<MpOcrDrivingResponse>("card_position_back", "示例独有（字段表未收录）");
        AssertJsonProperty<MpOcrCardPosition>("pos", "卡片位置四角点容器");

        // 驾驶证识别：官方字段 + 示例独有 nationality。
        AssertJsonProperty<MpOcrDrivingLicenseResponse>("id_num", "证号");
        AssertJsonProperty<MpOcrDrivingLicenseResponse>("car_class", "准驾车型");
        AssertJsonProperty<MpOcrDrivingLicenseResponse>("official_seal", "印章文字（官方说明原文误作「印章文构」，照录）");
        AssertJsonProperty<MpOcrDrivingLicenseResponse>("nationality", "示例独有（字段表未收录）");

        // 营业执照识别：字段表 14 项（含 cert_position / img_size）。
        AssertJsonProperty<MpOcrBizLicenseResponse>("reg_num", "注册号");
        AssertJsonProperty<MpOcrBizLicenseResponse>("legal_representative", "法定代表人姓名");
        AssertJsonProperty<MpOcrBizLicenseResponse>("business_scope", "经营范围");
        AssertJsonProperty<MpOcrBizLicenseResponse>("cert_position", "营业执照位置");
        AssertJsonProperty<MpOcrBizLicenseResponse>("img_size", "图片大小");

        // 通用印刷体：items[].text 见示例、字段表漏列 ⇒ 超集保留。
        AssertJsonProperty<MpOcrCommResponse>("items", "识别结果");
        AssertJsonProperty<MpOcrTextItem>("pos", "位置信息（字段表唯一列出的字段）");
        AssertJsonProperty<MpOcrTextItem>("text", "示例独有（字段表未收录）");

        // 菜单识别：content.menu_items[].name / price。
        AssertJsonProperty<MpOcrMenuResponse>("content", "识别的信息");
        AssertJsonProperty<MpOcrMenuContent>("menu_items", "菜单内容列表");
        AssertJsonProperty<MpOcrMenuItem>("name", "菜单名");
        AssertJsonProperty<MpOcrMenuItem>("price", "价格");

        // 图像处理·智能裁剪：results[].crop_left/top/right/bottom + img_size。
        AssertJsonProperty<MpImageAiCropResponse>("results", "智能裁剪结果");
        AssertJsonProperty<MpImageCropResult>("crop_left", "左上角 x");
        AssertJsonProperty<MpImageCropResult>("crop_bottom", "右下角 y");
        AssertJsonProperty<MpImageSize>("w", "宽度");
        AssertJsonProperty<MpImageSize>("h", "高度");

        // 图像处理·二维码识别：code_results[].type_name/data/pos。
        AssertJsonProperty<MpImageQrcodeResponse>("code_results", "处理结果");
        AssertJsonProperty<MpImageQrcodeResult>("type_name", "码的类型");
        AssertJsonProperty<MpImageQrcodeResult>("data", "码的信息");
        AssertJsonProperty<MpImageQrcodeResult>("pos", "码的坐标（条码 / PDF417 暂不返回）");
    }

    /// <summary>契约守卫 SM4：本域 DTO 全量登记进 AOT JSON 上下文且分组名一致。</summary>
    [Fact]
    public void SmartApiDataModels_ShouldBeRegisteredInJsonContext()
    {
        const string ns = "Mud.Wechat.OfficialAccount.DataModels.SmartApi";
        var domainTypes = typeof(MpOcrIdCardResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == ns
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(22,
            "智能接口域 DTO：OCR 响应 7 + OCR 条目/嵌套 7 + 图像处理响应 2 + 图像条目 2 + 语音/翻译 3 + B2a：车牌响应 1（superresolution 官方已下架不建模）");

        foreach (var type in domainTypes)
        {
            SmartApiJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 SmartApiJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }
    }

    /// <summary>契约守卫 SM5：模块枚举 / 注册入口 / Query 令牌白名单同批扩展。</summary>
    [Fact]
    public void SmartApiModule_ShouldBeRegisteredAndWhitelisted()
    {
        Enum.GetNames(typeof(MpModule)).Should().Contain("SmartApi");
        typeof(MpServiceBuilder).GetMethod("AddSmartApiApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();

        queryInterfaces.Should().Contain(nameof(IMpSmartApiService));
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");

        var token = typeof(IMpSmartApiService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull();
        token!.Name.Should().Be("access_token", "Query 注入参数名必须为官方契约的 access_token");
        token.TokenType.Should().Be(MpTokenTypes.AccessToken);
    }

    /// <summary>契约守卫 SM6：本域已核验错误码常量取值锁定。</summary>
    [Fact]
    public void SmartApiErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        MpErrorCodes.VoiceSizeInvalid.Should().Be(40010);
        MpErrorCodes.ArgsSizeInvalid.Should().Be(40035);
        MpErrorCodes.OcrImageUrlInvalid.Should().Be(101000);
        MpErrorCodes.OcrCertificateNotFound.Should().Be(101001);
        MpErrorCodes.OcrImageDecodeFailed.Should().Be(101002);
        MpErrorCodes.OcrMarketQuotaNotEnough.Should().Be(101003);
    }

    /// <summary>
    /// 契约守卫 SM7：**图片高清化 <c>/cv/img/superresolution</c> 刻意不建模**（官方已下架）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>事实来源（2026-10-09 核验）</b>：官方服务端 API 索引页在图像处理表内对该端点直接标注
    /// 「该接口用于将图片高清化，<b>由于系统维护原因，已下架</b>，如有需要使用，可前往微信开放社区发帖/
    /// 联系微信服务市场客服」；其专属文档页 <c>…/openpoc/image/api_imgsuperresolution.html</c>
    /// <b>实测返回 HTTP 404</b>。
    /// </para>
    /// <para>
    /// <b>为何本守卫不是「漏实现」而是「显式裁决」</b>：设计方案 §3.4 ⑤ 曾把它列为 P1-c 的
    /// 「SmartApi 增量」，并建议并入公众号线；后续复核发现该端点已被官方下架 ⇒ 按本仓既有纪律
    /// （<b>「官方页面 404 或声明下架即不建模」</b>，同 <c>/wxa/img_sec_check</c> 的处置）
    /// <b>不实现</b>，并把裁决固化在此，避免后来者照设计方案的字面清单「补」一个死端点。
    /// </para>
    /// </remarks>
    [Fact]
    public void SuperResolutionEndpoint_ShouldStayUnmodeled()
    {
        var declaredRoutes = typeof(IMpSmartApiService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(a => a.RequestUri)
            .ToList();

        declaredRoutes.Should().NotContain("/cv/img/superresolution",
            "官方索引页已标注该接口「由于系统维护原因，已下架」且文档页 404 ⇒ 不建模；" +
            "若将来官方恢复，须先复核字段契约再同批解除本裁决");

        // 防静默空跑：路由集合非空，说明上面的反射口径没失效。
        declaredRoutes.Should().NotBeEmpty();
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IMpSmartApiService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IMpSmartApiService.{methodName} 必须存在");

    /// <summary>（非表达式树谓词：FluentAssertions 的 Contain(predicate) 走表达式树，不支持 is 模式匹配。）</summary>
    private static bool HasMultipart(MethodInfo method)
        => method.GetParameters().Any(p => p.GetCustomAttribute<MultipartFormAttribute>() != null);

    private static bool HasBody(MethodInfo method)
        => method.GetParameters().Any(p => p.GetCustomAttribute<BodyAttribute>() != null);

    private static bool HasQuery(MethodInfo method, string queryName)
        => method.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Any(a => a.Name == queryName);

    private static List<string?> QueryNames(MethodInfo method)
        => method.GetParameters().SelectMany(p => p.GetCustomAttributes<QueryAttribute>()).Select(a => a.Name).ToList();

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}

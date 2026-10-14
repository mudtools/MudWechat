// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.DataModels.Apply4Subject;
using Mud.Wechat.Pay.DataModels.Apply4SubMerchant;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-2 主体进件（商户开户意愿确认）域契约守卫（<b>5 端点</b>：提交申请单 / 查询申请单审核结果 /
/// 按业务编号撤销 / 按申请单号撤销 / 获取商户开户意愿确认状态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（服务商文档中心，2026-10-14 逐页核验）</b>：提交申请单
/// <c>pay.weixin.qq.com/doc/v3/partner/4012722388</c>（<b>POST</b> <c>/v3/apply4subject/applyment/</c>，
/// <b>结尾斜杠是官方原文</b>）、查询申请单审核结果 <c>…/4012697715</c>（<b>GET</b>
/// <c>/v3/apply4subject/applyment</c>，<b>无</b>尾斜杠、定位键走 <b>query</b>）、撤销申请单
/// <c>…/4012697627</c>（<b>同页两个 path 变体</b>，应答 <b>204 无包体</b>）、获取商户开户意愿确认状态
/// <c>…/4012467549</c>（<b>GET</b> <c>/v3/apply4subject/applyment/merchants/{sub_mchid}/state</c>）。
/// 四页支持商户类型均为<b>渠道商 + 从业机构（银行）+ 从业机构（支付机构）</b>，<b>不含普通服务商</b>。
/// </para>
/// <para>
/// <b>本守卫要守住的四处「顺手就会写错」</b>（全部来自与本线另一进件域 <c>apply4sub</c> 的形似神异）：
/// ① 同一资源路径下 POST 带尾斜杠、GET 不带 —— 直觉会「统一」掉其中一个；
/// ② 查询定位键在本域是 <b>query</b>，进件域是<b>两个 path 端点</b>；
/// ③ <c>applyment_state</c> 与 <c>subject_type</c> 两域<b>同名不同值集</b>，
/// 合并常量或写「状态映射表」即静默错判；
/// ④ 营业执照块本域为英式 <c>business_licence_info</c>、金融机构标记为 <c>is_finance_institution</c>、
/// 单位证明函为 <c>company_prove_copy</c>，照进件域拼写抄即 400。
/// </para>
/// </remarks>
public class WechatPayApply4SubjectContractGuards
{
    private const string RegistryGroupName = "Apply4Subject";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.Apply4Subject";

    /// <summary>
    /// A4SJ-B1：5 端点的方法与路由照官方原文；path / query 位置不可互换。
    /// </summary>
    [Fact]
    public void Apply4SubjectEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayApply4SubjectService.SubmitApplymentAsync),
            "/v3/apply4subject/applyment/");
        AssertRoute<GetAttribute>(nameof(IWechatPayApply4SubjectService.QueryApplymentStateAsync),
            "/v3/apply4subject/applyment");
        AssertRoute<PostAttribute>(nameof(IWechatPayApply4SubjectService.CancelApplymentByBusinessCodeAsync),
            "/v3/apply4subject/applyment/{businessCode}/cancel");
        AssertRoute<PostAttribute>(nameof(IWechatPayApply4SubjectService.CancelApplymentByApplymentIdAsync),
            "/v3/apply4subject/applyment/{applymentId}/cancel");
        AssertRoute<GetAttribute>(nameof(IWechatPayApply4SubjectService.QueryMerchantAuthorizeStateAsync),
            "/v3/apply4subject/applyment/merchants/{subMchId}/state");

        // 查询审核结果：本域定位键走 **query**（官方 curl 即两键同传），不是进件域那样的两个 path 端点。
        var query = FindMethod(nameof(IWechatPayApply4SubjectService.QueryApplymentStateAsync));
        query.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name)
            .Should().BeEquivalentTo(new[] { "applyment_id", "business_code" },
                "官方两键均为 query（选填、至少传一个），不得下沉进 body 或改成 path 变体");
        query.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Should().BeEmpty("本页无 path 参数");

        // 两个撤销变体各带一个 path 定位键，且**没有请求体**（官方无 body）。
        FindMethod(nameof(IWechatPayApply4SubjectService.CancelApplymentByBusinessCodeAsync)).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "businessCode" });
        FindMethod(nameof(IWechatPayApply4SubjectService.CancelApplymentByApplymentIdAsync)).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "applymentId" });
        foreach (var name in new[]
                 {
                     nameof(IWechatPayApply4SubjectService.CancelApplymentByBusinessCodeAsync),
                     nameof(IWechatPayApply4SubjectService.CancelApplymentByApplymentIdAsync),
                 })
        {
            FindMethod(name).GetParameters()
                .Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
                    $"{name} 官方无请求体");
        }

        // 商户号只出现在「获取开户意愿确认状态」一个端点上，且是**调用期 path 参数**（非商户作用域配置）。
        FindMethod(nameof(IWechatPayApply4SubjectService.QueryMerchantAuthorizeStateAsync)).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "subMchId" });

        // 提交与查询两个端点**不含** sub_mchid：主体进件阶段该商户号还不存在。
        foreach (var name in new[]
                 {
                     nameof(IWechatPayApply4SubjectService.SubmitApplymentAsync),
                     nameof(IWechatPayApply4SubjectService.QueryApplymentStateAsync),
                     nameof(IWechatPayApply4SubjectService.CancelApplymentByBusinessCodeAsync),
                     nameof(IWechatPayApply4SubjectService.CancelApplymentByApplymentIdAsync),
                 })
        {
            FindMethod(name).GetParameters()
                .Select(static p => p.Name)
                .Should().NotContain("subMchId", $"{name} 属 apply4subject 申请单族，官方无 sub_mchid 参数");
        }
    }

    /// <summary>
    /// A4SJ-B2：资源前缀单一（<c>/v3/apply4subject/</c>）+ <b>POST 带尾斜杠、GET 不带</b>的同路径异形。
    /// </summary>
    [Fact]
    public void Apply4SubjectRoutes_ShouldStayOnSinglePrefixWithOfficialSlashAsymmetry()
    {
        var routes = typeof(IWechatPayApply4SubjectService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

        routes.Should().HaveCount(5, "本域 5 端点（数量变化须同批更新 A4SJ-B1 与 Tests/ContractBaseline.cs）");

        routes.Should().OnlyContain(static r => r.StartsWith("/v3/apply4subject/", StringComparison.Ordinal),
            "本域全部路由都在 /v3/apply4subject/ 下（apply4sub / applyment4sub 属另一域）");

        // 交叉污染正对照：本域不得出现另一进件域的两种「4sub」镜像前缀。
        routes.Should().NotContain(static r => r.Contains("apply4sub/", StringComparison.Ordinal),
            "/v3/apply4sub/ 是特约商户进件域的结算账户前缀，本域不得声明");
        routes.Should().NotContain(static r => r.Contains("applyment4sub", StringComparison.Ordinal),
            "/v3/applyment4sub/ 是特约商户进件域的申请单前缀，本域不得声明");

        // 提交申请单的结尾斜杠是官方原文，而查询同资源时**没有**斜杠 ⇒ 两形并存，不得「统一」。
        routes.Should().Contain("/v3/apply4subject/applyment/",
            "官方 POST 以斜杠结尾，不得纠正为 /v3/apply4subject/applyment");
        routes.Should().Contain("/v3/apply4subject/applyment",
            "官方 GET 无尾斜杠（带斜杠是另一个资源路径）");
        FindMethod(nameof(IWechatPayApply4SubjectService.QueryApplymentStateAsync))
            .GetCustomAttribute<GetAttribute>()!.RequestUri!
            .Should().NotEndWith("/", "官方查询端点无尾斜杠：删/加斜杠在这里是真实契约变更而非风格问题");

        // 两个撤销变体必须各自成方法（path 段字面相同、语义不同）。
        routes.Count(static r => r.EndsWith("/cancel", StringComparison.Ordinal))
            .Should().Be(2, "官方同页给出 business_code 与 applyment_id 两个 path 变体");
    }

    /// <summary>A4SJ-B3：本域官方字段名锁定（照官方原文拼写，<b>不得按进件域拼写「纠正」</b>）。</summary>
    [Fact]
    public void Apply4SubjectDtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<Apply4SubjectApplymentRequest>(nameof(Apply4SubjectApplymentRequest.ChannelId), "channel_id");
        JsonNameShouldBe<Apply4SubjectApplymentRequest>(nameof(Apply4SubjectApplymentRequest.BusinessCode), "business_code");
        JsonNameShouldBe<Apply4SubjectApplymentRequest>(nameof(Apply4SubjectApplymentRequest.IdentificationInfo), "identification_info");
        JsonNameShouldBe<Apply4SubjectApplymentRequest>(nameof(Apply4SubjectApplymentRequest.UboInfoList), "ubo_info_list");

        JsonNameShouldBe<Apply4SubjectApplymentResponse>(nameof(Apply4SubjectApplymentResponse.ApplymentId), "applyment_id");

        // 英式 licence / is_ 前缀 / company_prove_copy —— 三处都是「抄进件域拼写」的落点。
        JsonNameShouldBe<Apply4SubjectSubjectInfo>(nameof(Apply4SubjectSubjectInfo.IsFinanceInstitution), "is_finance_institution");
        JsonNameShouldBe<Apply4SubjectSubjectInfo>(nameof(Apply4SubjectSubjectInfo.BusinessLicenceInfo), "business_licence_info");
        JsonNameShouldBe<Apply4SubjectSubjectInfo>(nameof(Apply4SubjectSubjectInfo.CompanyProveCopy), "company_prove_copy");
        JsonNameShouldBe<Apply4SubjectSubjectInfo>(nameof(Apply4SubjectSubjectInfo.AssistProveInfo), "assist_prove_info");
        JsonNameShouldBe<Apply4SubjectBusinessLicenceInfo>(nameof(Apply4SubjectBusinessLicenceInfo.LicenceNumber), "licence_number");
        JsonNameShouldBe<Apply4SubjectBusinessLicenceInfo>(nameof(Apply4SubjectBusinessLicenceInfo.LicenceValidDate), "licence_valid_date");

        JsonNameShouldBe<Apply4SubjectContactInfo>(nameof(Apply4SubjectContactInfo.ContactIdDocType), "contact_id_doc_type");
        JsonNameShouldBe<Apply4SubjectContactInfo>(nameof(Apply4SubjectContactInfo.ContactType), "contact_type");
        JsonNameShouldBe<Apply4SubjectIdentificationInfo>(nameof(Apply4SubjectIdentificationInfo.IdHolderType), "id_holder_type");
        JsonNameShouldBe<Apply4SubjectIdentificationInfo>(nameof(Apply4SubjectIdentificationInfo.IdentificationFrontCopy), "identification_front_copy");
        JsonNameShouldBe<Apply4SubjectIdentificationInfo>(nameof(Apply4SubjectIdentificationInfo.AuthorizeLetterCopy), "authorize_letter_copy");

        // 审核结果应答四字段（qrcode_data 是 base64 图片数据，不是链接字段名）。
        JsonNameShouldBe<Apply4SubjectApplymentStateResponse>(nameof(Apply4SubjectApplymentStateResponse.ApplymentState), "applyment_state");
        JsonNameShouldBe<Apply4SubjectApplymentStateResponse>(nameof(Apply4SubjectApplymentStateResponse.QrcodeData), "qrcode_data");
        JsonNameShouldBe<Apply4SubjectApplymentStateResponse>(nameof(Apply4SubjectApplymentStateResponse.RejectParam), "reject_param");
        JsonNameShouldBe<Apply4SubjectApplymentStateResponse>(nameof(Apply4SubjectApplymentStateResponse.RejectReason), "reject_reason");

        JsonNameShouldBe<Apply4SubjectMerchantStateResponse>(nameof(Apply4SubjectMerchantStateResponse.AuthorizeState), "authorize_state");
        JsonNameShouldBe<Apply4SubjectAdditionInfo>(nameof(Apply4SubjectAdditionInfo.ConfirmMchidList), "confirm_mchid_list");
        JsonNameShouldBe<Apply4SubjectSpecialOperation>(nameof(Apply4SubjectSpecialOperation.OperationCopyList), "operation_copy_list");
        JsonNameShouldBe<Apply4SubjectFinanceInstitutionInfo>(nameof(Apply4SubjectFinanceInstitutionInfo.FinanceLicensePics), "finance_license_pics");
    }

    /// <summary>
    /// A4SJ-B4：与进件域「同名不同值」的两组枚举<b>不得合并、不得互转</b>（本域最隐蔽的一处漂移）。
    /// </summary>
    [Fact]
    public void ApplymentStateAndSubjectTypeEnums_ShouldNotMergeWithApply4SubMerchant()
    {
        var subjectStates = ValuesOf(typeof(Apply4SubjectApplymentStates));
        var subStates = ValuesOf(typeof(Apply4SubApplymentStates));

        subjectStates.Should().HaveCount(8, "官方 applyment_state 为 8 值");
        subStates.Should().HaveCount(8);

        // 只重合 EDITTING / REJECTED / CANCELED 三值，其余五值各走各路 ⇒ 没有可用的状态映射表。
        subjectStates.Intersect(subStates).Should().BeEquivalentTo(new[]
        {
            "APPLYMENT_STATE_EDITTING", "APPLYMENT_STATE_REJECTED", "APPLYMENT_STATE_CANCELED",
        }, "两域状态集只有三值重合：写成一套常量或加映射表即静默错判");

        subjectStates.Should().NotContain("APPLYMENT_STATE_FINISHED");
        subStates.Should().NotContain("APPLYMENT_STATE_PASSED");

        // 主体类型：本域多「小微商户」，且事业单位取值带官方内部复制后缀 _CLONED。
        var subjectTypes = ValuesOf(typeof(Apply4SubjectSubjectTypes));
        subjectTypes.Should().HaveCount(6, "官方 subject_type 为 6 值（比进件域多 SUBJECT_TYPE_MICRO）");
        subjectTypes.Should().Contain("SUBJECT_TYPE_MICRO");
        subjectTypes.Should().Contain("SUBJECT_TYPE_INSTITUTIONS_CLONED",
            "官方事业单位取值即带 _CLONED 后缀，不得纠正为进件域的 SUBJECT_TYPE_INSTITUTIONS");
        subjectTypes.Should().NotContain("SUBJECT_TYPE_INSTITUTIONS");
        subjectTypes.Except(ValuesOf(typeof(Apply4SubSubjectTypes)))
            .Should().BeEquivalentTo(new[] { "SUBJECT_TYPE_MICRO", "SUBJECT_TYPE_INSTITUTIONS_CLONED" });
    }

    /// <summary>A4SJ-B5：官方枚举取值逐项锁定（含数量，防漏抄 / 防自造值 / 防「纠正拼写」）。</summary>
    [Fact]
    public void Apply4SubjectEnumConstants_ShouldMatchOfficialValues()
    {
        ValuesOf(typeof(Apply4SubjectApplymentStates)).Should().BeEquivalentTo(new[]
        {
            "APPLYMENT_STATE_EDITTING", "APPLYMENT_STATE_WAITTING_FOR_AUDIT",
            "APPLYMENT_STATE_WAITTING_FOR_CONFIRM_CONTACT", "APPLYMENT_STATE_WAITTING_FOR_CONFIRM_LEGALPERSON",
            "APPLYMENT_STATE_PASSED", "APPLYMENT_STATE_REJECTED", "APPLYMENT_STATE_FREEZED",
            "APPLYMENT_STATE_CANCELED",
        }, "官方三处非常规拼写（EDITTING 双 T、WAITTING 双 T、FREEZED 非 FROZEN）照原文，纠正即判错失效");

        ValuesOf(typeof(Apply4SubjectAuthorizeStates)).Should().BeEquivalentTo(new[]
        {
            "AUTHORIZE_STATE_UNAUTHORIZED", "AUTHORIZE_STATE_AUTHORIZED",
        }, "官方 authorize_state 仅 2 值，与申请单状态是两个独立维度");

        ValuesOf(typeof(Apply4SubjectHolderTypes)).Should().BeEquivalentTo(new[] { "LEGAL", "SUPER" },
            "contact_type 与 id_holder_type 共用同一组**裸值**（无前缀）");

        ValuesOf(typeof(Apply4SubjectIdDocTypes)).Should().HaveCount(8, "官方证件类型共 8 值");
        ValuesOf(typeof(Apply4SubjectCertificateTypes)).Should().HaveCount(10, "官方 cert_type 共 10 值");
        ValuesOf(typeof(Apply4SubjectMicroBizTypes)).Should().BeEquivalentTo(new[]
        {
            "MICRO_TYPE_STORE", "MICRO_TYPE_MOBILE", "MICRO_TYPE_ONLINE",
        });
        ValuesOf(typeof(Apply4SubjectFinanceTypes)).Should().BeEquivalentTo(new[]
        {
            "BANK_AGENT", "PAYMENT_AGENT", "INSURANCE", "TRADE_AND_SETTLE", "SMALL_LOAN", "OTHER",
        });

        // 本域四页业务错误码表**只有公共码**（INVALID_REQUEST / NO_AUTH / SYSTEM_ERROR）⇒
        // 刻意不建本域专有错误码常量类；以相邻进件域作正对照（那边确有专有码）。
        typeof(Apply4SubjectApplymentRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace)
            .Select(static t => t.Name)
            .Should().NotContain(static n => n.EndsWith("ErrorCodes", StringComparison.Ordinal),
                "官方本域错误码表无专有码：照进件域抄一个 APPLYMENT_NOT_EXIST 即制造「能识别单不存在」的假象");
        ValuesOf(typeof(Apply4SubMerchantErrorCodes)).Should().Contain("APPLYMENT_NOT_EXIST",
            "正对照：进件域确有该专有码，两域判错面不同");
    }

    /// <summary>A4SJ-B6：判错面与应答形态 —— 三支有包体的应答继承 <see cref="WechatPayResponse"/>，两个撤销端点无包体。</summary>
    [Fact]
    public void Apply4SubjectResponses_ShouldDeriveFromWechatPayResponseAndMatchOfficialBodies()
    {
        foreach (var responseType in new[]
                 {
                     typeof(Apply4SubjectApplymentResponse),
                     typeof(Apply4SubjectApplymentStateResponse),
                     typeof(Apply4SubjectMerchantStateResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
        }

        // 官方明确「无应答包体」（204）⇒ 两个撤销方法必须无返回值。
        FindMethod(nameof(IWechatPayApply4SubjectService.CancelApplymentByBusinessCodeAsync)).ReturnType
            .Should().Be(typeof(Task), "官方撤销成功为 204 No Content 且无包体");
        FindMethod(nameof(IWechatPayApply4SubjectService.CancelApplymentByApplymentIdAsync)).ReturnType
            .Should().Be(typeof(Task));

        var declared = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        // 本域审核结果应答**没有**进件域那几个字段 ⇒ 不得为「方便」共用类型而补上。
        foreach (var forbidden in new[] { "SignUrl", "SubMchId", "ApplymentStateMsg", "AuditDetail", "BusinessCode", "ApplymentId" })
        {
            typeof(Apply4SubjectApplymentStateResponse).GetProperty(forbidden, declared)
                .Should().BeNull($"官方本页应答字段表无 {forbidden}（进件域才有），不得靠共用类型补上");
        }

        // 提交应答只有 applyment_id 一个字段。
        typeof(Apply4SubjectApplymentResponse).GetProperties(declared).Should().HaveCount(1,
            "官方提交应答字段表只有 applyment_id");
    }

    /// <summary>A4SJ-B7：本域 DTO 全量登记进 AOT 上下文且分组名一致；示例报文里的遗留字段不得建类型。</summary>
    [Fact]
    public void Apply4SubjectDataModels_ShouldBeRegisteredInJsonContext()
    {
        var domainTypes = typeof(Apply4SubjectApplymentResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(14,
            "主体进件域 DTO：申请单族 5 + 主体族 6 + 联系人 1 + 法定代表人/受益人 2");

        foreach (var type in domainTypes)
        {
            Apply4SubjectJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 Apply4SubjectJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 官方示例 JSON 里有单数 ubo_info（字段表内没有）⇒ 以字段表为权威，不得据示例建字段。
        typeof(Apply4SubjectApplymentRequest).GetProperties()
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Should().NotContain("ubo_info",
                "字段表只有 ubo_info_list（复数），示例里的单数 ubo_info 是遗留形态、不建模");
        typeof(Apply4SubjectApplymentRequest).GetProperty(nameof(Apply4SubjectApplymentRequest.UboInfoList))!
            .PropertyType.Should().Be(typeof(List<Apply4SubjectUboInfo>),
                "ubo_info_list 是数组，列表项类型即 Apply4SubjectUboInfo");
    }

    // ---- helpers -------------------------------------------------------------

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayApply4SubjectService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayApply4SubjectService.{methodName} 必须存在");

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
    {
        var property = typeof(T).GetProperty(propertyName);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }

    private static string[] ValuesOf(Type constantsType)
        => constantsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();
}

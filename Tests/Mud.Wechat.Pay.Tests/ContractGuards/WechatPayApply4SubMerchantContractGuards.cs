// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.DataModels.Apply4SubMerchant;
using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-1 特约商户进件域契约守卫（<b>6 端点</b>：提交申请单 / 两路查询申请状态 / 修改结算账户 /
/// 查询结算账户 / 查询结算账户修改申请状态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（服务商文档中心，2026-10-14 逐页核验）</b>：提交申请单
/// <c>pay.weixin.qq.com/doc/v3/partner/4012719997</c>（<b>POST</b> <c>/v3/applyment4sub/applyment/</c>，
/// <b>结尾斜杠是官方原文</b>）、申请单号查询申请状态 <c>…/4012697052</c>、业务申请编号查询申请状态
/// <c>…/4012697168</c>、修改结算账户 <c>…/4012761102</c>（平台商户文档树同页 <c>…/4012761138</c>）、
/// 查询结算账户 <c>…/4012761113</c>（同 <c>…/4012761142</c>）、查询结算账户修改申请状态
/// <c>…/4012761169</c>（同 <c>…/4012761120</c>）。
/// </para>
/// <para>
/// <b>本守卫要守住的三个「顺手就会写错」</b>：
/// ① 两族前缀互为镜像（<c>applyment4sub</c> 装申请单、<c>apply4sub</c> 装结算账户），
/// 按「一个域一个前缀」的直觉统一掉即产出调不动的路由；
/// ② 申请单查询<b>不含</b> <c>sub_mchid</c> path（进件时商户号还不存在），结算账户三端点<b>恒含</b>；
/// ③ <c>account_type</c> 与 <c>verify_result</c> 这两个字段名在本域各对应<b>两套不同取值</b>，
/// 合并成一组常量即静默错判。
/// </para>
/// </remarks>
public class WechatPayApply4SubMerchantContractGuards
{
    private const string RegistryGroupName = "Apply4SubMerchant";

    /// <summary>
    /// A4S-B1：6 端点的方法与路由照官方原文；path / query 位置不可互换。
    /// </summary>
    [Fact]
    public void Apply4SubMerchantEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayApply4SubMerchantService.SubmitApplymentAsync),
            "/v3/applyment4sub/applyment/");
        AssertRoute<GetAttribute>(nameof(IWechatPayApply4SubMerchantService.QueryApplymentStateByApplymentIdAsync),
            "/v3/applyment4sub/applyment/applyment_id/{applymentId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayApply4SubMerchantService.QueryApplymentStateByBusinessCodeAsync),
            "/v3/applyment4sub/applyment/business_code/{businessCode}");
        AssertRoute<PostAttribute>(nameof(IWechatPayApply4SubMerchantService.ModifySettlementAsync),
            "/v3/apply4sub/sub_merchants/{subMchId}/modify-settlement");
        AssertRoute<GetAttribute>(nameof(IWechatPayApply4SubMerchantService.QuerySettlementAsync),
            "/v3/apply4sub/sub_merchants/{subMchId}/settlement");
        AssertRoute<GetAttribute>(nameof(IWechatPayApply4SubMerchantService.QuerySettlementModifyApplicationAsync),
            "/v3/apply4sub/sub_merchants/{subMchId}/application/{applicationNo}");

        // 结算账户三端点恒以 sub_mchid 定位（官方 path 参数）。
        FindMethod(nameof(IWechatPayApply4SubMerchantService.ModifySettlementAsync)).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "subMchId" });

        // 修改申请状态查询是**双 path**（sub_mchid + application_no），account_number_rule 走 query。
        var application = FindMethod(nameof(IWechatPayApply4SubMerchantService.QuerySettlementModifyApplicationAsync));
        application.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "subMchId", "applicationNo" });
        application.GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name)
            .Should().BeEquivalentTo(new[] { "account_number_rule" },
                "官方 account_number_rule 是选填 query，不得下沉进 body 或 path");

        // 申请单两路查询**不含** sub_mchid：进件提交时特约商户号尚不存在（官方该两页无此参数）。
        foreach (var name in new[]
                 {
                     nameof(IWechatPayApply4SubMerchantService.SubmitApplymentAsync),
                     nameof(IWechatPayApply4SubMerchantService.QueryApplymentStateByApplymentIdAsync),
                     nameof(IWechatPayApply4SubMerchantService.QueryApplymentStateByBusinessCodeAsync),
                 })
        {
            FindMethod(name).GetParameters()
                .Select(static p => p.Name)
                .Should().NotContain("subMchId", $"{name} 属 applyment4sub 族，官方无 sub_mchid 参数");
        }
    }

    /// <summary>
    /// A4S-B2：两族前缀<b>互为镜像</b>，不得交叉（防「按一个域一个前缀」的直觉把路由统一掉）。
    /// </summary>
    [Fact]
    public void ApplymentAndSettlementRouteFamilies_ShouldNotCrossPrefixes()
    {
        var routes = typeof(IWechatPayApply4SubMerchantService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

        routes.Should().HaveCount(6, "本域 6 端点（数量变化须同批更新 A4S-B1 与 Tests/ContractBaseline.cs）");

        routes.Where(static r => r.Contains("/sub_merchants/", StringComparison.Ordinal))
            .Should().OnlyContain(static r => r.StartsWith("/v3/apply4sub/", StringComparison.Ordinal),
                "结算账户族只能在 /v3/apply4sub/sub_merchants/…");

        routes.Where(static r => r.Contains("/applyment/", StringComparison.Ordinal))
            .Should().OnlyContain(static r => r.StartsWith("/v3/applyment4sub/", StringComparison.Ordinal),
                "申请单族只能在 /v3/applyment4sub/applyment…（勿写成 apply4sub）");

        // 提交申请单的结尾斜杠是官方原文：删掉它是「顺手规整 URL」最典型的落点。
        routes.Should().Contain("/v3/applyment4sub/applyment/",
            "官方路由以斜杠结尾，不得纠正为 /v3/applyment4sub/applyment");
    }

    /// <summary>A4S-B3：进件域 DTO 官方字段名锁定（照官方原文，<b>不得「纠正」</b>）。</summary>
    [Fact]
    public void Apply4SubMerchantDtos_ShouldExposeOfficialFieldNames()
    {
        JsonNameShouldBe<Apply4SubApplymentRequest>(nameof(Apply4SubApplymentRequest.BusinessCode), "business_code");
        JsonNameShouldBe<Apply4SubApplymentRequest>(nameof(Apply4SubApplymentRequest.BankAccountInfo), "bank_account_info");
        JsonNameShouldBe<Apply4SubApplymentStateResponse>(nameof(Apply4SubApplymentStateResponse.ApplymentState), "applyment_state");
        JsonNameShouldBe<Apply4SubApplymentStateResponse>(nameof(Apply4SubApplymentStateResponse.ApplymentStateMsg), "applyment_state_msg");
        JsonNameShouldBe<Apply4SubApplymentStateResponse>(nameof(Apply4SubApplymentStateResponse.SignUrl), "sign_url");
        JsonNameShouldBe<Apply4SubApplymentStateResponse>(nameof(Apply4SubApplymentStateResponse.SubMchId), "sub_mchid");
        JsonNameShouldBe<Apply4SubAuditDetail>(nameof(Apply4SubAuditDetail.FieldName), "field_name");

        JsonNameShouldBe<Apply4SubSubjectInfo>(nameof(Apply4SubSubjectInfo.SubjectType), "subject_type");
        JsonNameShouldBe<Apply4SubSubjectInfo>(nameof(Apply4SubSubjectInfo.UboInfoList), "ubo_info_list");
        JsonNameShouldBe<Apply4SubIdentityInfo>(nameof(Apply4SubIdentityInfo.IdHolderType), "id_holder_type");
        JsonNameShouldBe<Apply4SubContactInfo>(nameof(Apply4SubContactInfo.ContactIdDocType), "contact_id_doc_type");

        // 官方 sales_scenes_type 是**单数拼写的数组**（勿按 C# 复数习惯改成 *_types）。
        JsonNameShouldBe<Apply4SubSalesInfo>(nameof(Apply4SubSalesInfo.SalesScenesType), "sales_scenes_type");
        JsonNameShouldBe<Apply4SubBusinessInfo>(nameof(Apply4SubBusinessInfo.MerchantShortName), "merchant_shortname");
        JsonNameShouldBe<Apply4SubSettlementInfo>(nameof(Apply4SubSettlementInfo.SettlementId), "settlement_id");
        JsonNameShouldBe<Apply4SubBankAccountInfo>(nameof(Apply4SubBankAccountInfo.BankAccountType), "bank_account_type");

        JsonNameShouldBe<Apply4SubModifySettlementResponse>(nameof(Apply4SubModifySettlementResponse.ApplicationNo), "application_no");
        JsonNameShouldBe<Apply4SubSettlementModifyApplicationResponse>(nameof(Apply4SubSettlementModifyApplicationResponse.VerifyFinishTime), "verify_finish_time");
    }

    /// <summary>
    /// A4S-B4：两套「同名不同值」的枚举不得合并（本域最隐蔽的一处漂移）。
    /// </summary>
    /// <remarks>
    /// 进件侧账户类型是 <c>BANK_ACCOUNT_TYPE_*</c>、结算账户侧是 <c>ACCOUNT_TYPE_*</c>；
    /// <c>verify_result</c> 在「查询结算账户」是 <c>VERIFY_*</c>（账户验证态）、
    /// 在「查询修改申请状态」是 <c>AUDIT_*</c>（申请审核态）。同名 ⇒ 极易被合并成一个常量类，
    /// 合并后运行期症状是「把审核中读成验证失败」。
    /// </remarks>
    [Fact]
    public void SettlementAndApplymentAccountTypeEnums_ShouldNotMerge()
    {
        Apply4SubBankAccountTypes.Corporate.Should().Be("BANK_ACCOUNT_TYPE_CORPORATE");
        Apply4SubBankAccountTypes.Personal.Should().Be("BANK_ACCOUNT_TYPE_PERSONAL");
        Apply4SubSettlementAccountTypes.Business.Should().Be("ACCOUNT_TYPE_BUSINESS");
        Apply4SubSettlementAccountTypes.Private.Should().Be("ACCOUNT_TYPE_PRIVATE");

        Apply4SubSettlementAccountTypes.Business.Should().NotBe(Apply4SubBankAccountTypes.Corporate,
            "进件侧与结算账户侧的账户类型是两套官方取值，不得统一");

        Apply4SubSettlementVerifyResults.Success.Should().Be("VERIFY_SUCCESS");
        Apply4SubSettlementAuditResults.Success.Should().Be("AUDIT_SUCCESS");
        Apply4SubSettlementVerifyResults.Success.Should().NotBe(Apply4SubSettlementAuditResults.Success,
            "同名字段 verify_result 在两页语义不同：VERIFY_* 是账户验证、AUDIT_* 是申请审核");

        Apply4SubSettlementVerifyResults.Verifying.Should().Be("VERIFYING");
        Apply4SubSettlementAuditResults.Auditing.Should().Be("AUDITING");
    }

    /// <summary>A4S-B5：官方枚举取值逐项锁定（含数量，防漏抄 / 防自造值）。</summary>
    [Fact]
    public void Apply4SubMerchantEnumConstants_ShouldMatchOfficialValues()
    {
        ValuesOf(typeof(Apply4SubApplymentStates)).Should().BeEquivalentTo(new[]
        {
            "APPLYMENT_STATE_EDITTING", "APPLYMENT_STATE_AUDITING", "APPLYMENT_STATE_REJECTED",
            "APPLYMENT_STATE_TO_BE_CONFIRMED", "APPLYMENT_STATE_TO_BE_SIGNED", "APPLYMENT_STATE_SIGNING",
            "APPLYMENT_STATE_FINISHED", "APPLYMENT_STATE_CANCELED",
        }, "官方 applyment_state 为 8 值（EDITTING 系官方原文拼写，不得纠正为 EDITING）");

        ValuesOf(typeof(Apply4SubSubjectTypes)).Should().BeEquivalentTo(new[]
        {
            "SUBJECT_TYPE_INDIVIDUAL", "SUBJECT_TYPE_ENTERPRISE", "SUBJECT_TYPE_GOVERNMENT",
            "SUBJECT_TYPE_INSTITUTIONS", "SUBJECT_TYPE_OTHERS",
        });

        ValuesOf(typeof(Apply4SubIdDocTypes)).Should().HaveCount(8, "官方 id_doc_type 共 8 值");
        ValuesOf(typeof(Apply4SubCertificateTypes)).Should().HaveCount(10, "官方 cert_type 共 10 值");
        ValuesOf(typeof(Apply4SubFinanceTypes)).Should().BeEquivalentTo(new[]
        {
            "BANK_AGENT", "PAYMENT_AGENT", "INSURANCE", "TRADE_AND_SETTLE", "SMALL_LOAN", "OTHER",
        });
        ValuesOf(typeof(Apply4SubSalesScenesTypes)).Should().HaveCount(6, "官方 sales_scenes_type 共 6 值");
        ValuesOf(typeof(Apply4SubAccountNumberRules)).Should().BeEquivalentTo(new[]
        {
            "ACCOUNT_NUMBER_RULE_MASK_V1", "ACCOUNT_NUMBER_RULE_MASK_V2",
        });

        // 官方错误码 FREQENCY_LIMIT 少一个字母 U ⇒ 照原文（纠正即判错失效）。
        Apply4SubMerchantErrorCodes.FrequencyLimit.Should().Be("FREQENCY_LIMIT");
        Apply4SubMerchantErrorCodes.ApplymentNotExist.Should().Be("APPLYMENT_NOT_EXIST");
        Apply4SubMerchantErrorCodes.Processing.Should().Be("PROCESSING");
        Apply4SubMerchantErrorCodes.OrderNotExist.Should().Be("ORDER_NOT_EXIST");
    }

    /// <summary>A4S-B6：判错面 —— 四支应答须继承 <see cref="WechatPayResponse"/>。</summary>
    [Fact]
    public void Apply4SubMerchantResponses_ShouldDeriveFromWechatPayResponse()
    {
        foreach (var responseType in new[]
                 {
                     typeof(Apply4SubApplymentResponse),
                     typeof(Apply4SubApplymentStateResponse),
                     typeof(Apply4SubModifySettlementResponse),
                     typeof(Apply4SubSettlementResponse),
                     typeof(Apply4SubSettlementModifyApplicationResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
        }

        // 「查询结算账户」与「查询修改申请状态」是**两个应答类型**：后者多 account_name 与 verify_finish_time。
        typeof(Apply4SubSettlementModifyApplicationResponse).GetProperty("VerifyFinishTime")
            .Should().NotBeNull("修改申请应答含 verify_finish_time");
        typeof(Apply4SubSettlementResponse).GetProperty("VerifyFinishTime")
            .Should().BeNull("查询结算账户应答官方无 verify_finish_time 字段，不得靠共用类型补上");
    }

    /// <summary>A4S-B7：进件域 DTO 全量登记进 AOT 上下文且分组名一致。</summary>
    [Fact]
    public void Apply4SubMerchantDataModels_ShouldBeRegisteredInJsonContext()
    {
        var domainTypes = typeof(Apply4SubApplymentResponse).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == "Mud.Wechat.Pay.DataModels.Apply4SubMerchant")
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(28,
            "进件域 DTO：申请单族 4 + 主体族 8 + 联系人 1 + 经营信息族 11 + 结算账户族 4");

        foreach (var type in domainTypes)
        {
            Apply4SubMerchantJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 Apply4SubMerchantJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }
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
        => typeof(IWechatPayApply4SubMerchantService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayApply4SubMerchantService.{methodName} 必须存在");

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

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.OfficialAccount.DataModels.Invoice;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// P4 第三域「微信发票」契约守卫（17 端点：商户开票 5 + 开票平台 5 + 发票报销 4 + 极速开发票 3）。
/// </summary>
/// <remarks>
/// <para>
/// 官方事实来源（逐页核验 2026-10-07，17 页全服务号域命中）。关键核验：
/// ① <b>17 页全部用 <c>access_token</c>，零 <c>api_ticket</c></b>（检查单「发票域例外」担忧不成立）；
/// ② 账号门槛三档并存（需申请 / ✔ / 仅认证）；③ 16/17 页支持第三方代调用（<c>scantitle</c> 独家不支持）；
/// ④ 无频率数值；⑤ 错误码为<b>族级共用表</b>（多页重复列出同一张约 43 枚的表）。
/// </para>
/// <para>
/// <b>守卫锁定的形态与共用裁决（勿「顺手修正」）</b>：<c>setbizattr</c> 请求/响应同构 ⇒ 共用三类型；
/// <c>reimburse</c> 单张响应与批量条目同构 ⇒ 共用 <see cref="MpInvoiceReimburseInfo"/>；
/// 插卡的 <c>invoice_user_data</c> 与报销的 <c>user_info</c> 同构 ⇒ 共用 <see cref="MpInvoiceUserData"/>（超集）；
/// <c>trip_pdf_ur</c> 为官方字段原文（不得规范化）。
/// </para>
/// </remarks>
public class MpInvoiceContractGuards
{
    private const string RegistryGroupName = "Invoice";

    /// <summary>官方路由表（17 端点，全 POST、全带 body，除 <c>seturl</c> 无请求体）。</summary>
    private static readonly (string Route, string Method, bool HasBody)[] Routes =
    {
        ("/card/invoice/setbizattr", nameof(IMpInvoiceService.SetInvoiceBizAttrAsync), true),
        ("/card/invoice/getauthdata", nameof(IMpInvoiceService.QueryInvoiceAuthDataAsync), true),
        ("/card/invoice/getauthurl", nameof(IMpInvoiceService.GetInvoiceAuthUrlAsync), true),
        ("/card/invoice/rejectinsert", nameof(IMpInvoiceService.RejectInsertAsync), true),
        ("/card/invoice/seturl", nameof(IMpInvoiceService.GetInvoicePlatformIdentifyAsync), false),
        ("/card/invoice/platform/setpdf", nameof(IMpInvoiceService.UploadInvoicePdfAsync), true),
        ("/card/invoice/platform/getpdf", nameof(IMpInvoiceService.GetInvoicePdfAsync), true),
        ("/card/invoice/platform/updatestatus", nameof(IMpInvoiceService.UpdateInvoiceCardStatusAsync), true),
        ("/card/invoice/platform/createcard", nameof(IMpInvoiceService.CreateInvoiceCardAsync), true),
        ("/card/invoice/insert", nameof(IMpInvoiceService.InsertInvoiceAsync), true),
        ("/card/invoice/reimburse/getinvoiceinfo", nameof(IMpInvoiceService.GetReimburseInvoiceAsync), true),
        ("/card/invoice/reimburse/updateinvoicestatus", nameof(IMpInvoiceService.UpdateReimburseInvoiceStatusAsync), true),
        ("/card/invoice/reimburse/updatestatusbatch", nameof(IMpInvoiceService.BatchUpdateReimburseInvoiceStatusAsync), true),
        ("/card/invoice/reimburse/getinvoicebatch", nameof(IMpInvoiceService.BatchGetReimburseInvoicesAsync), true),
        ("/card/invoice/biz/getusertitleurl", nameof(IMpInvoiceService.GetUserTitleUrlAsync), true),
        ("/card/invoice/biz/getselecttitleurl", nameof(IMpInvoiceService.GetSelectTitleUrlAsync), true),
        ("/card/invoice/scantitle", nameof(IMpInvoiceService.ScanInvoiceTitleAsync), true),
        ("/card/invoice/makeoutinvoice", nameof(IMpInvoiceService.MakeOutInvoiceAsync), true),
        ("/card/invoice/clearoutinvoice", nameof(IMpInvoiceService.ClearOutInvoiceAsync), true),
        ("/card/invoice/queryinvoceinfo", nameof(IMpInvoiceService.QueryInvoiceInfoAsync), true),
    };

    /// <summary>契约守卫 IT1：20 端点路由 / 方法 / 请求体形态与官方契约一致。</summary>
    [Fact]
    public void InvoiceEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Select(r => r.Route).Distinct().Should().HaveCount(Routes.Length, "各端点路由互不重复");
        Routes.Should().OnlyContain(r => r.Route.StartsWith("/card/invoice/", StringComparison.Ordinal),
            "本域 20 端点全在 /card/invoice/ 前缀下（官方路径安排）");

        foreach (var (route, method, hasBody) in Routes)
        {
            var target = FindMethod(method);
            var attr = target.GetCustomAttribute<PostAttribute>();
            attr.Should().NotBeNull($"{method} 必须声明 POST 路由");
            attr!.RequestUri.Should().Be(route, $"{method} 路由必须与官方契约一致");
            HasPayload(method).Should().Be(hasBody,
                hasBody ? $"{method} 官方定义为请求体（JSON 或 multipart）" : $"{method} 官方无请求体（seturl）");
        }

        typeof(IMpInvoiceService).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<HttpMethodAttribute>() != null)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()!)
            .Should().AllBeAssignableTo<PostAttribute>("20 端点官方全部为 POST");

        // setbizattr 的 action 为 Query（非请求体）——官方契约。
        FindMethod(nameof(IMpInvoiceService.SetInvoiceBizAttrAsync)).GetParameters()
            .Should().Contain(p => p.ParameterType == typeof(string)
                                   && p.GetCustomAttributes<QueryAttribute>().Any(a => a.Name == "action"),
                "setbizattr 的 action 是 Query 参数");

        // setpdf 为 multipart 上传（表单文件字段名 pdf）。
        FindMethod(nameof(IMpInvoiceService.UploadInvoicePdfAsync)).GetParameters()
            .Any(p => p.GetCustomAttribute<MultipartFormAttribute>() != null)
            .Should().BeTrue("setpdf 官方为 multipart/form-data（表单字段 pdf）");
    }

    /// <summary>契约守卫 IT2：同构共用裁决锁定（防重复造 DTO）+ 响应复用。</summary>
    [Fact]
    public void InvoiceSharedShapes_ShouldKeepAdjudicatedForms()
    {
        // setbizattr 请求 / 响应同构 ⇒ 共用 auth_field / paymch_info / contact 三类型。
        foreach (var shared in new[]
                 {
                     typeof(MpInvoiceAuthField), typeof(MpInvoicePayMchInfo), typeof(MpInvoiceContact),
                 })
        {
            typeof(MpInvoiceBizAttrRequest).GetProperties().Any(p => p.PropertyType == shared)
                .Should().BeTrue($"setbizattr 请求体应使用共用类型 {shared.Name}");
            typeof(MpInvoiceBizAttrResponse).GetProperties().Any(p => p.PropertyType == shared)
                .Should().BeTrue($"setbizattr 响应应使用共用类型 {shared.Name}");
        }

        // 报销单张响应与批量条目同构 ⇒ 共用 MpInvoiceReimburseInfo（批量条目类型即该类型）。
        TaskResultType(nameof(IMpInvoiceService.GetReimburseInvoiceAsync)).Should().Be(typeof(MpInvoiceReimburseInfo));
        typeof(MpInvoiceReimburseBatchGetResponse)
            .GetProperty(nameof(MpInvoiceReimburseBatchGetResponse.ItemList))!.PropertyType
            .Should().Be(typeof(List<MpInvoiceReimburseInfo>),
                "批量条目与单张响应字段集完全一致 ⇒ 共用（条目的 errcode 缺省 0）");

        // 插卡 invoice_user_data 与报销 user_info 同构 ⇒ 共用超集 MpInvoiceUserData。
        typeof(MpInvoiceUserCard).GetProperty(nameof(MpInvoiceUserCard.InvoiceUserData))!.PropertyType
            .Should().Be(typeof(MpInvoiceUserData));
        typeof(MpInvoiceReimburseInfo).GetProperty(nameof(MpInvoiceReimburseInfo.UserInfo))!.PropertyType
            .Should().Be(typeof(MpInvoiceUserData));
        JsonNamesOf<MpInvoiceUserData>().Should().Contain("s_pdf_media_id").And.Contain("pdf_url").And.Contain("reimburse_status");

        // 批量更新与批量查询的发票定位同构 ⇒ 共用 MpInvoiceRef。
        typeof(MpInvoiceReimburseBatchUpdateRequest)
            .GetProperty(nameof(MpInvoiceReimburseBatchUpdateRequest.InvoiceList))!.PropertyType
            .Should().Be(typeof(List<MpInvoiceRef>));
        typeof(MpInvoiceReimburseBatchGetRequest).GetProperty(nameof(MpInvoiceReimburseBatchGetRequest.ItemList))!.PropertyType
            .Should().Be(typeof(List<MpInvoiceRef>));

        // 四个「仅 errcode/errmsg」端点复用 MpResponse（防空壳 DTO）。
        foreach (var method in new[]
                 {
                     nameof(IMpInvoiceService.RejectInsertAsync),
                     nameof(IMpInvoiceService.UpdateInvoiceCardStatusAsync),
                     nameof(IMpInvoiceService.UpdateReimburseInvoiceStatusAsync),
                     nameof(IMpInvoiceService.BatchUpdateReimburseInvoiceStatusAsync),
                 })
        {
            TaskResultType(method).Should().Be(typeof(MpResponse), $"{method} 官方响应仅 errcode/errmsg");
        }
    }

    /// <summary>契约守卫 IT3：DTO 官方字段名锁定（抽样覆盖 41 类型）。</summary>
    [Fact]
    public void InvoiceShapes_ShouldLockOfficialFieldNames()
    {
        // setbizattr 全树。
        AssertJsonProperty<MpInvoiceBizAttrRequest>("auth_field", "set_auth_field 使用");
        AssertJsonProperty<MpInvoiceBizAttrRequest>("paymch_info", "set_pay_mch 使用");
        AssertJsonProperty<MpInvoiceBizAttrRequest>("contact", "set_contact 使用");
        AssertJsonProperty<MpInvoiceAuthField>("user_field", "个人发票字段");
        AssertJsonProperty<MpInvoiceAuthField>("biz_field", "单位发票字段");
        AssertJsonProperty<MpInvoiceUserField>("require_email", "仅 get_auth_field 返回");
        AssertJsonProperty<MpInvoiceBizField>("show_bank_no", "是否填写银行账号");
        AssertJsonProperty<MpInvoiceBizField>("require_tax_no", "官方字段表重复列出两次");
        AssertJsonProperty<MpInvoiceCustomField>("is_require", "0 否 / 1 是，默认 0");
        AssertJsonProperty<MpInvoicePayMchInfo>("s_pappid", "开票平台 id（非票据）");
        AssertJsonProperty<MpInvoiceContact>("time_out", "开票超时时间");
        AssertJsonProperty<MpInvoiceBizAttrResponse>("auth_field", "被设置/查询的字段");

        // 授权链。
        AssertJsonProperty<MpInvoiceAuthDataRequest>("order_id", "订单号");
        AssertJsonProperty<MpInvoiceAuthDataResponse>("invoice_status", "官方未给枚举表");
        AssertJsonProperty<MpInvoiceAuthDataResponse>("auth_time", "授权时间戳");
        AssertJsonProperty<MpInvoiceAuthUrlRequest>("s_pappid", "开票平台标识号");
        AssertJsonProperty<MpInvoiceAuthUrlRequest>("redirect_url", "官方称仅 H5（疑指 web）需要");
        AssertJsonProperty<MpInvoiceAuthUrlRequest>("ticket", "授权页 ticket（非鉴权凭证）");
        AssertJsonProperty<MpInvoiceAuthUrlResponse>("auth_url", "授权链接");
        AssertJsonProperty<MpInvoiceAuthUrlResponse>("appid", "仅 source=wxa 返回");
        AssertJsonProperty<MpInvoiceRejectInsertRequest>("reason", "拒绝原因");
        AssertJsonProperty<MpInvoiceSetUrlResponse>("invoice_url", "s_pappid 须从该链接解析");

        // 开票平台链。
        AssertJsonProperty<MpInvoiceSetPdfResponse>("s_media_id", "有效期 3 天（string 承载「64 位整数」矛盾）");
        AssertJsonProperty<MpInvoiceGetPdfRequest>("action", "官方要求填 get_url");
        AssertJsonProperty<MpInvoiceGetPdfResponse>("pdf_url_expire_time", "7200 秒");
        AssertJsonProperty<MpInvoiceUpdateStatusRequest>("reimburse_status", "报销状态");
        AssertJsonProperty<MpInvoiceCreateCardRequest>("invoice_info", "发票模板对象");
        AssertJsonProperty<MpInvoiceTemplateInfo>("base_info", "卡券模板基础信息");
        AssertJsonProperty<MpInvoiceCardBaseInfo>("logo_url", "须走永久素材接口");
        AssertJsonProperty<MpInvoiceCardBaseInfo>("promotion_url_sub_title", "≤6 汉字");
        AssertJsonProperty<MpInvoiceCreateCardResponse>("card_id", "插卡必填");
        AssertJsonProperty<MpInvoiceInsertRequest>("card_ext", "发票具体内容");
        AssertJsonProperty<MpInvoiceCardExt>("nonce_str", "防重复");
        AssertJsonProperty<MpInvoiceUserCard>("invoice_user_data", "用户信息");
        AssertJsonProperty<MpInvoiceUserData>("billing_no", "数电发票传 20 位");
        AssertJsonProperty<MpInvoiceUserData>("s_trip_pdf_media_id", "行程单等附件");
        AssertJsonProperty<MpInvoiceUserData>("check_code", "数电发票为空");
        AssertJsonProperty<MpInvoiceItem>("num", "数量");
        AssertJsonProperty<MpInvoiceItem>("price", "单价");
        AssertJsonProperty<MpInvoiceInsertResponse>("unionid", "绑定开放平台后才有");

        // 报销链。
        AssertJsonProperty<MpInvoiceReimburseQueryRequest>("encrypt_code", "官方请求体栏标「无」而示例有");
        AssertJsonProperty<MpInvoiceReimburseInfo>("begin_time", "开始时间");
        AssertJsonProperty<MpInvoiceReimburseInfo>("user_info", "用户发票信息");
        AssertJsonProperty<MpInvoiceReimburseUpdateRequest>("reimburse_status", "报销状态");
        AssertJsonProperty<MpInvoiceReimburseBatchUpdateRequest>("invoice_list", "发票列表");
        AssertJsonProperty<MpInvoiceRef>("card_id", "与 encrypt_code 构成唯一标识");
        AssertJsonProperty<MpInvoiceReimburseBatchGetRequest>("item_list", "官方未标注最大条数");
        AssertJsonProperty<MpInvoiceReimburseBatchGetResponse>("item_list", "条目共用单张响应类型");

        // 极速开发票链。
        AssertJsonProperty<MpInvoiceUserTitleUrlRequest>("tax_no", "15-20 位数字或英文字母");
        AssertJsonProperty<MpInvoiceUserTitleUrlRequest>("user_fill", "0 企业设置 / 1 用户填写");
        AssertJsonProperty<MpInvoiceUserTitleUrlRequest>("out_title_id", "开票码");
        AssertJsonProperty<MpInvoiceUserTitleUrlResponse>("url", "用户确认链接");
        AssertJsonProperty<MpInvoiceSelectTitleUrlRequest>("biz_name", "商户名称");
        AssertJsonProperty<MpInvoiceSelectTitleUrlResponse>("url", "专属抬头链接（须转二维码）");
        AssertJsonProperty<MpInvoiceScanTitleRequest>("scan_text", "扫码原始数据");
        AssertJsonProperty<MpInvoiceScanTitleResponse>("title_type", "0 单位 / 1 个人");
    }

    /// <summary>契约守卫 IT4：官方文档冲突处置的负向锁定（防「顺手修正」）。</summary>
    [Fact]
    public void InvoiceDocumentConflicts_ShouldKeepAdjudicatedShapes()
    {
        // 官方字段原文 trip_pdf_ur（疑 trip_pdf_url 笔误）⇒ 不得规范化；且不得额外造 trip_pdf_url。
        var userDataNames = JsonNamesOf<MpInvoiceUserData>();
        userDataNames.Should().Contain("trip_pdf_ur", "官方字段原文，疑笔误，不得「规范化」");
        userDataNames.Should().NotContain("trip_pdf_url");

        // s_media_id 标 string 但描述「64 位整数」⇒ 以 string 承载（不丢前导零 / 不受位宽限制）。
        typeof(MpInvoiceSetPdfResponse).GetProperty(nameof(MpInvoiceSetPdfResponse.SMediaId))!.PropertyType
            .Should().Be(typeof(string));

        // 「请求体栏标无、示例有」的两个端点必须建模请求体（否则无法寻址发票）。
        typeof(MpInvoiceReimburseQueryRequest).GetProperties().Should().NotBeEmpty(
            "getinvoiceinfo 官方请求体栏标「无」而代码示例给出 card_id/encrypt_code ⇒ 取并集建模");
        typeof(MpInvoiceReimburseUpdateRequest).GetProperties().Should().NotBeEmpty(
            "updateinvoicestatus 同处该缺陷");

        // 批量返回示例的 order_id 未在字段表列出且无形态说明 ⇒ 不建模。
        JsonNamesOf<MpInvoiceReimburseInfo>().Should().NotContain("order_id",
            "批量示例独有字段、字段表未收录且无形态说明 ⇒ 不建模");

        // scantitle 是本域唯一「不支持第三方平台调用」的端点（守卫锚定该事实，防误标为支持）。
        // （第三方支持面只记录在 XML；此处锚定 method 存在性，避免端点被误删。）
        FindMethod(nameof(IMpInvoiceService.ScanInvoiceTitleAsync)).Should().NotBeNull();
    }

    /// <summary>契约守卫 IT5：DTO 全量登记进上下文 + 模块枚举 / 注册入口 / Query 令牌白名单。</summary>
    [Fact]
    public void InvoiceModuleAndDataModels_ShouldBeRegistered()
    {
        const string ns = "Mud.Wechat.OfficialAccount.DataModels.Invoice";
        var domainTypes = typeof(MpInvoiceRef).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == ns
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(48,
            "商户开票 14 + 开票平台 14 + 报销与极速开发票 13 + B2a 电子发票开具 7（明细项 / 票面信息 / 开票请求 / 冲红请求 / 查询请求 / 查询明细 / 查询响应）");
        foreach (var type in domainTypes)
        {
            InvoiceJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 InvoiceJsonContext");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        Enum.GetNames(typeof(MpModule)).Should().Contain("Invoice");
        typeof(MpServiceBuilder).GetMethod("AddInvoiceApi").Should().NotBeNull();

        var queryInterfaces = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .ToList();
        queryInterfaces.Should().Contain(nameof(IMpInvoiceService));
        queryInterfaces.Should().NotBeEmpty("防「发现机制失效导致白名单真空」的静默空跑");

        var token = typeof(IMpInvoiceService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull();
        token!.Name.Should().Be("access_token");
        token.TokenType.Should().Be(MpTokenTypes.AccessToken);
    }

    /// <summary>契约守卫 IT6：枚举常量与族级共用错误码面锁定。</summary>
    [Fact]
    public void InvoiceConstants_ShouldMatchVerifiedOfficialValues()
    {
        MpInvoiceBizAttrActions.SetAuthField.Should().Be("set_auth_field");
        MpInvoiceBizAttrActions.GetAuthField.Should().Be("get_auth_field");
        MpInvoiceBizAttrActions.SetPayMch.Should().Be("set_pay_mch");
        MpInvoiceBizAttrActions.GetPayMch.Should().Be("get_pay_mch");
        MpInvoiceBizAttrActions.SetContact.Should().Be("set_contact");
        MpInvoiceBizAttrActions.GetContact.Should().Be("get_contact");

        MpInvoiceAuthSources.App.Should().Be("app");
        MpInvoiceAuthSources.Web.Should().Be("web");
        MpInvoiceAuthSources.Wxa.Should().Be("wxa");
        MpInvoiceAuthSources.Wap.Should().Be("wap");

        MpInvoiceAuthUrlTypes.Open.Should().Be(0);
        MpInvoiceAuthUrlTypes.FillFields.Should().Be(1, "官方原文：支付后开票业务只能调用 type=1");
        MpInvoiceAuthUrlTypes.Collect.Should().Be(2);

        MpInvoiceReimburseStatuses.Init.Should().Be("INVOICE_REIMBURSE_INIT");
        MpInvoiceReimburseStatuses.Lock.Should().Be("INVOICE_REIMBURSE_LOCK");
        MpInvoiceReimburseStatuses.Closure.Should().Be("INVOICE_REIMBURSE_CLOSURE");
        MpInvoiceReimburseStatuses.Cancel.Should().Be("INVOICE_REIMBURSE_CANCEL", "platform/updatestatus 页的冲红值");

        MpInvoiceTitleTypes.Business.Should().Be(0);
        MpInvoiceTitleTypes.Personal.Should().Be(1);

        // 族级共用错误码面（官方多页重复列出同一张表）抽样锁定。
        MpErrorCodes.InvoiceCardStatusInvalid.Should().Be(40078);
        MpErrorCodes.InvoiceUnauthorized.Should().Be(72015);
        MpErrorCodes.InvoiceTitleMismatch.Should().Be(72017);
        MpErrorCodes.InvoiceLockedByOthers.Should().Be(72023);
        MpErrorCodes.InvoiceStatusError.Should().Be(72024);
        MpErrorCodes.InvoiceTokenError.Should().Be(72025);
        MpErrorCodes.InvoicePayMchNotSet.Should().Be(72028);
        MpErrorCodes.InvoiceAuthFieldNotSet.Should().Be(72029);
        MpErrorCodes.InvoiceMchIdInvalid.Should().Be(72030);
        MpErrorCodes.InvoiceParamsInvalid.Should().Be(72031);
        MpErrorCodes.InvoiceBizRejectInsert.Should().Be(72035);
        MpErrorCodes.InvoiceBusy.Should().Be(72036);
        MpErrorCodes.InvoiceOrderNotAuthorized.Should().Be(72038);
        MpErrorCodes.InvoiceMustLockFirst.Should().Be(72039);
        MpErrorCodes.InvoicePdfError.Should().Be(72040);
        MpErrorCodes.InvoiceBillingRepeated.Should().Be(72042);
        MpErrorCodes.InvoiceBillingSizeError.Should().Be(72043);
        MpErrorCodes.InvoiceScanTextOutOfTime.Should().Be(72044);
        MpErrorCodes.InvoiceContactEmpty.Should().Be(72063);
        MpErrorCodes.InvoiceMakeOutFailed.Should().Be(73000);
        MpErrorCodes.InvoiceNsrsbhEmpty.Should().Be(73016);
        MpErrorCodes.InvoiceKaPlatError.Should().Be(73100);
        MpErrorCodes.InvoicePlatformSystemError.Should().Be(73102);
        MpErrorCodes.InvoiceFpqqlshPrefixNotCmp.Should().Be(73110);
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IMpInvoiceService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IMpInvoiceService.{methodName} 必须存在");

    private static Type TaskResultType(string methodName)
        => FindMethod(methodName).ReturnType.GetGenericArguments()[0];

    /// <summary>是否携带请求体：JSON（<c>[Body]</c>）或 multipart 表单（<c>[MultipartForm]</c>，仅 setpdf）。</summary>
    private static bool HasPayload(string methodName)
        => FindMethod(methodName).GetParameters().Any(p =>
            p.GetCustomAttribute<BodyAttribute>() != null
            || p.GetCustomAttribute<MultipartFormAttribute>() != null);

    private static List<string> JsonNamesOf<T>()
        => typeof(T).GetProperties()
            .Select(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}

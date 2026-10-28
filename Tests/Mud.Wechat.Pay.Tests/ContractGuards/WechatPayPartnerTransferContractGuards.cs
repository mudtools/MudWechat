// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.BankTransfer;
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.PartnerTransfer;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-6 服务商批量转账到零钱域契约守卫（<b>5 端点</b>：发起 1 + 批次查询 2 + 明细查询 2）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（合作伙伴文档中心「开发文档(服务商)」·「批量转账到零钱」，2026-10-14 逐页核验，
/// 各页「支持商户」栏恒为【普通服务商】）：4012465738 发起批量转账、4012465772 微信支付批次单号查询批次单、
/// 4012465781 微信支付明细单号查询明细单、4012465790 商家批次单号查询批次单、
/// 4012465795 商家明细单号查询明细单。逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>SKIT 清单本域 5 行 ⇒ 5 端点、零归并</b>。同章其余侧栏组（转账电子回单五支、余额查询 / 提现 /
/// 帐单 / 来账识别）的路由<b>无 <c>partner-</c> 段</b>或属他形态 ⇒ 本域零端点，守卫 PTF-B2 带正对照。
/// </para>
/// <para>
/// <b>本守卫要守住的六处「顺手就会写错」</b>：
/// ① 路由是 <c>/v3/partner-transfer/batches</c>，<b>不是</b>直连新版商家转账的
/// <c>/v3/fund-app/mch-transfer/transfer-bills</c>（后者按笔发起、无批次概念）；
/// ② 计数字段是 <c>total_num</c> / <c>success_num</c> / <c>fail_num</c>、状态字段是
/// <c>batch_status</c> / <c>detail_status</c> —— 银行面同族能力用 <c>*_count</c> 与 <c>*_state</c>；
/// ③ 明细<b>请求</b>姓名是 <c>user_name</c>、<b>应答</b>是 <c>username</c>；
/// ④ <c>need_query_detail</c> 官方是<b>必填 query</b>（不是选填开关）；
/// ⑤ 明细查询应答<b>没有</b> <c>sub_mchid</c>（批次应答有），且时间字段是 <c>initiate_time</c>；
/// ⑥ 官方 <c>account_type</c> 正文笔写 <c>WXPAY_ACCOUNT</c> 而取值表写 <c>WECHATPAY_ACCOUNT</c>
/// ⇒ 采<b>取值表</b>拼写，不做「统一」。
/// </para>
/// </remarks>
public class WechatPayPartnerTransferContractGuards
{
    private const string RegistryGroupName = "PartnerTransfer";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PartnerTransfer";

    private const string Prefix = "/v3/partner-transfer/";

    /// <summary>
    /// PTF-B1：5 端点的方法与路由照官方原文；两支批次查询的 query 集逐支锁定、path 定位键逐支锁定；
    /// 本域<b>无 204 端点</b>。
    /// </summary>
    [Fact]
    public void PartnerTransferEndpoints_ShouldMatchOfficialRoutesAndParameterPositions()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerTransferService.CreateBatchAsync),
            Prefix + "batches");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerTransferService.QueryBatchByBatchIdAsync),
            Prefix + "batches/batch-id/{batchId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerTransferService.QueryDetailByDetailIdAsync),
            Prefix + "batches/batch-id/{batchId}/details/detail-id/{detailId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerTransferService.QueryBatchByOutBatchNoAsync),
            Prefix + "batches/out-batch-no/{outBatchNo}");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerTransferService.QueryDetailByOutDetailNoAsync),
            Prefix + "batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}");

        // 两支批次查询：官方四参数（need_query_detail 必填 + 三个选填），两支明细查询：官方无 query。
        QueryNamesOf(nameof(IWechatPayPartnerTransferService.QueryBatchByBatchIdAsync))
            .Should().BeEquivalentTo(new[] { "need_query_detail", "offset", "limit", "detail_status" });
        QueryNamesOf(nameof(IWechatPayPartnerTransferService.QueryBatchByOutBatchNoAsync))
            .Should().BeEquivalentTo(new[] { "need_query_detail", "offset", "limit", "detail_status" },
                "两支批次查询官方参数表相同 ⇒ 形参也须相同");
        QueryNamesOf(nameof(IWechatPayPartnerTransferService.QueryDetailByDetailIdAsync))
            .Should().BeEmpty("明细查询直接回全字段，官方无「是否查明细」开关");
        QueryNamesOf(nameof(IWechatPayPartnerTransferService.QueryDetailByOutDetailNoAsync))
            .Should().BeEmpty();

        // need_query_detail 必填 ⇒ 声明为非可空 bool 且无默认值（可空或带默认值即允许漏传，官方报 PARAM_ERROR）。
        var needDetail = FindMethod(nameof(IWechatPayPartnerTransferService.QueryBatchByBatchIdAsync))
            .GetParameters().Single(static p => p.GetCustomAttribute<QueryAttribute>()?.Name == "need_query_detail");
        needDetail.ParameterType.Should().Be(typeof(bool),
            "官方该 query 为必填 boolean：声明成 bool? 或有默认值即可能不带参数发出");
        needDetail.HasDefaultValue.Should().BeFalse();

        // 过滤参数名是 detail_status（银行面同名过滤叫 detail_state）。
        QueryNamesOf(nameof(IWechatPayPartnerTransferService.QueryBatchByBatchIdAsync))
            .Should().Contain("detail_status")
            .And.NotContain("detail_state", "本域过滤名与银行面不同，照抄即静默丢过滤条件");

        // path 定位键逐支锁定（明细两支是双层定位）。
        PathKeysOf(nameof(IWechatPayPartnerTransferService.QueryBatchByBatchIdAsync))
            .Should().BeEquivalentTo(new[] { "batchId" });
        PathKeysOf(nameof(IWechatPayPartnerTransferService.QueryBatchByOutBatchNoAsync))
            .Should().BeEquivalentTo(new[] { "outBatchNo" });
        PathKeysOf(nameof(IWechatPayPartnerTransferService.QueryDetailByDetailIdAsync))
            .Should().BeEquivalentTo(new[] { "batchId", "detailId" });
        PathKeysOf(nameof(IWechatPayPartnerTransferService.QueryDetailByOutDetailNoAsync))
            .Should().BeEquivalentTo(new[] { "outBatchNo", "outDetailNo" });

        // 发起支：官方无 path / query，只有一个包体参数。
        var createParameters = FindMethod(nameof(IWechatPayPartnerTransferService.CreateBatchAsync)).GetParameters();
        createParameters.Should().NotContain(static p => p.GetCustomAttribute<PathAttribute>() != null);
        createParameters.Should().NotContain(static p => p.GetCustomAttribute<QueryAttribute>() != null);
        BodyTypeOf(nameof(IWechatPayPartnerTransferService.CreateBatchAsync))
            .Should().Be(typeof(PartnerTransferBatchCreateRequest));

        // GET 一律无请求体。
        foreach (var method in typeof(IWechatPayPartnerTransferService)
                     .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(static m => m.GetCustomAttribute<GetAttribute>() != null))
        {
            method.GetParameters().Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{method.Name} 是 GET，官方无请求体");
        }

        // 本域五支端点全部有 JSON 应答 ⇒ 零 204（与还款 / 支付分域相反）。
        NoContentMethodsOf(typeof(IWechatPayPartnerTransferService)).Should().BeEmpty(
            "本域官方无「无应答包体」端点：出现裸 Task 即把有应答的端点写丢了");
    }

    /// <summary>
    /// PTF-B2：单一前缀 <c>/v3/partner-transfer/</c> 的全量归属 + 与相邻三域的路由互斥 +
    /// 本章其余侧栏组（回单 / 余额 / 提现 / 账单）零声明，每处均须正对照。
    /// </summary>
    [Fact]
    public void PartnerTransferRoutes_ShouldStayOnSinglePartnerTransferPrefixAndNotDeclareNeighbourGroups()
    {
        var routes = RoutesOf(typeof(IWechatPayPartnerTransferService));

        routes.Should().HaveCount(5, "本域 5 端点（数量变化须同批更新 PTF-B1 与 Tests/ContractBaseline.cs）");
        routes.Count(static r => r.StartsWith(Prefix, StringComparison.Ordinal))
            .Should().Be(5, "本域五端点全在 /v3/partner-transfer/ 单前缀下");

        // 零声明①：转账电子回单族（路由实测无 partner- 段）不进本域。
        routes.Should().NotContain(static r => r.Contains("receipt", StringComparison.Ordinal)
                                               || r.Contains("electronic-receipts", StringComparison.Ordinal),
            "服务商转账电子回单的官方路由不带 partner- 段，与本章批量转账不同族 ⇒ 不得并入本域");
        // 正对照：银行面确实声明了回单端点（同一扫描口径能发现该能力）。
        RoutesOf(typeof(IWechatPayBankTransferService))
            .Should().Contain(static r => r.Contains("/receipt/", StringComparison.Ordinal),
                "正对照：回单能力只落在银行面，本域零声明是有据裁决而非漏落");

        // 零声明②：余额查询 / 提现 / 资金账单不进本域（且受 Capital 域守卫 C1 全域约束）。
        routes.Should().NotContain(static r => r.Contains("/fund", StringComparison.Ordinal)
                                               || r.Contains("/bill", StringComparison.Ordinal),
            "资金与账单能力属他章（余额查询 / 提现 / 帐单），且 /v3/merchant/fund 受守卫 C1 全域零声明约束");
        // 正对照：账单域在同口径扫描下确有 /v3/bill/ 路由。
        RoutesOf(typeof(IWechatPayBillService))
            .Should().Contain(static r => r.StartsWith("/v3/bill/", StringComparison.Ordinal));

        // 零声明③：直连新版商家转账与本域互斥。
        routes.Should().NotContain(static r => r.StartsWith("/v3/fund-app/", StringComparison.Ordinal),
            "直连面走 /v3/fund-app/mch-transfer/transfer-bills（按笔发起），与本域批次面互斥");
        RoutesOf(typeof(IWechatPayTransferService))
            .Should().Contain(static r => r.StartsWith("/v3/fund-app/mch-transfer/", StringComparison.Ordinal),
                "正对照：直连新版商家转账路由确实存在，上一条不是因该域缺失而恒真");

        // 三域两两互斥 +  Interfaces 目录形态。
        routes.Intersect(RoutesOf(typeof(IWechatPayBankTransferService))).Should().BeEmpty("服务商面与银行面路由不得重叠");
        routes.Intersect(RoutesOf(typeof(IWechatPayTransferService))).Should().BeEmpty("服务商面与直连面路由不得重叠");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", "PartnerTransfer"))
            .Should().BeTrue("正对照：Interfaces/PartnerTransfer/ 确实已落位");
        File.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"),
            "Interfaces", "PartnerTransferReceipt", "IWechatPayPartnerTransferReceiptService.cs"))
            .Should().BeFalse("本线不建服务商转账回单域：官方路由无 partner- 段，须单独立域再逐页取证");
    }

    /// <summary>
    /// PTF-B3：官方字段名逐项锁定（含 <c>*_num</c> 计数族与 <c>batch_status</c> / <c>detail_status</c>）+
    /// 与银行面的四处字段名分歧（防「顺手复用字段」）+ 时间字段恒 <c>string</c>。
    /// </summary>
    [Fact]
    public void PartnerTransferDtos_ShouldExposeOfficialFieldNamesAndDivergeFromBankDomain()
    {
        // 服务商标识三件套 + 批次必填六件套。
        JsonNameShouldBe<PartnerTransferBatchCreateRequest>(nameof(PartnerTransferBatchCreateRequest.SubMchId), "sub_mchid");
        JsonNameShouldBe<PartnerTransferBatchCreateRequest>(nameof(PartnerTransferBatchCreateRequest.SubAppId), "sub_appid");
        JsonNameShouldBe<PartnerTransferBatchCreateRequest>(nameof(PartnerTransferBatchCreateRequest.SpAppId), "sp_appid");
        JsonNameShouldBe<PartnerTransferBatchCreateRequest>(nameof(PartnerTransferBatchCreateRequest.AuthorizationType), "authorization_type");
        JsonNameShouldBe<PartnerTransferBatchCreateRequest>(nameof(PartnerTransferBatchCreateRequest.OutBatchNo), "out_batch_no");
        JsonNameShouldBe<PartnerTransferBatchCreateRequest>(nameof(PartnerTransferBatchCreateRequest.BatchName), "batch_name");
        JsonNameShouldBe<PartnerTransferBatchCreateRequest>(nameof(PartnerTransferBatchCreateRequest.TotalNum), "total_num");
        JsonNameShouldBe<PartnerTransferBatchCreateRequest>(nameof(PartnerTransferBatchCreateRequest.TransferDetailList), "transfer_detail_list");

        // 明细请求：姓名 / 身份证是 user_name / user_id_card（应答侧姓名是 username）。
        JsonNameShouldBe<PartnerTransferDetailInput>(nameof(PartnerTransferDetailInput.UserName), "user_name");
        JsonNameShouldBe<PartnerTransferDetailInput>(nameof(PartnerTransferDetailInput.UserIdCard), "user_id_card");
        JsonNameShouldBe<PartnerTransferDetailInput>(nameof(PartnerTransferDetailInput.OpenId), "openid");
        JsonNameShouldBe<PartnerTransferDetailQueryResponse>(nameof(PartnerTransferDetailQueryResponse.UserName), "username");

        // 批次应答的计数与状态：total_num / success_num / fail_num + batch_status。
        DeclaredJsonNamesOf<PartnerTransferBatchQueryResponse>().Should()
            .Contain(new[] { "total_num", "success_num", "fail_num", "batch_status", "batch_type", "close_reason" });
        DeclaredJsonNamesOf<PartnerTransferDetailQueryResponse>().Should()
            .Contain(new[] { "detail_status", "initiate_time", "account_type", "bank_card_number_tail" });

        // 与银行面的四处名差（正对照双向）：本域 *_num / *_status，银行面 *_count / *_state。
        DeclaredJsonNamesOf<PartnerTransferBatchQueryResponse>().Should().NotContain(new[] { "total_count", "success_count", "fail_count", "batch_state" },
            "本域官方无 count/state 形态：从银行面抄字段即静默丢字段");
        DeclaredJsonNamesOf<BankTransferBatchQueryResponse>().Should().Contain(new[] { "total_count", "success_count", "fail_count", "batch_state" },
            "正对照：银行面确用 count/state");
        DeclaredJsonNamesOf<PartnerTransferBatchDetailItem>().Should().Contain("detail_status")
            .And.NotContain("detail_state");
        DeclaredJsonNamesOf<BankTransferBatchDetailItem>().Should().Contain("detail_state",
            "正对照：银行面批次内明细项状态名是 detail_state");

        // 本域批次应答有 sub_mchid，明细应答没有（官方两页字段表差异，不得按「同一实体」推定）。
        DeclaredJsonNamesOf<PartnerTransferBatchQueryResponse>().Should().Contain("sub_mchid");
        DeclaredJsonNamesOf<PartnerTransferDetailQueryResponse>().Should().NotContain("sub_mchid",
            "官方明细查询页应答表无 sub_mchid（只有 sp_mchid + 选填 appid）");

        // 全部时间字段保持 string（官方 rfc3339，支付线不做 DateTimeOffset 往返）。
        foreach (var type in DomainTypes())
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                property.PropertyType.Should().NotBe(typeof(DateTimeOffset),
                    $"{type.Name}.{property.Name} 不得用 DateTimeOffset");
                property.PropertyType.Should().NotBe(typeof(DateTime));
                if (property.Name.EndsWith("Time", StringComparison.Ordinal))
                {
                    property.PropertyType.Should().Be(typeof(string), $"{type.Name}.{property.Name} 时间字段恒 string");
                }
            }
        }
    }

    /// <summary>
    /// PTF-B4：官方取值集合逐项锁定（9 个常量类）+ 与银行面的三处值集分歧 +
    /// 官方正文笔写 <c>WXPAY_ACCOUNT</c> 的取舍。
    /// </summary>
    [Fact]
    public void PartnerTransferEnumConstants_ShouldMatchOfficialValuesAndDivergeFromBankDomain()
    {
        ValuesOf(typeof(PartnerTransferAuthorizationTypes)).Should().BeEquivalentTo(new[]
        {
            "INFORMATION_AUTHORIZATION_TYPE", "FUND_AUTHORIZATION_TYPE", "INFORMATION_AND_FUND_AUTHORIZATION_TYPE",
        }, "官方 authorization_type 恰三值（信息与出资授权组合各一）");

        ValuesOf(typeof(PartnerTransferBatchStates)).Should().BeEquivalentTo(new[]
        {
            "WAIT_PAY", "ACCEPTED", "PROCESSING", "FINISHED", "CLOSED",
        }, "本域批次状态五值：比银行面多 WAIT_PAY（服务商须付款确认）");

        ValuesOf(typeof(PartnerTransferBatchTypes)).Should().BeEquivalentTo(new[] { "API", "WEB" });

        ValuesOf(typeof(PartnerTransferCloseReasons)).Should().BeEquivalentTo(new[]
        {
            "MERCHANT_REVOCATION", "OVERDUE_CLOSE", "PAYER_ACCOUNT_ABNORMAL",
        }, "官方注明 close_reason 后续可能新增 ⇒ 本类非穷尽集合，判错不得按集合收敛");

        ValuesOf(typeof(PartnerTransferDetailStates)).Should().BeEquivalentTo(new[]
        {
            "INIT", "WAIT_PAY", "PROCESSING", "SUCCESS", "FAIL", "REFUND",
        }, "本域明细六值：比银行面多 INIT 与 WAIT_PAY");

        ValuesOf(typeof(PartnerTransferDetailStateFilters)).Should().BeEquivalentTo(new[]
        {
            "ALL", "FAIL", "SUCCESS", "WAIT_PAY",
        }, "查询过滤四值（含 WAIT_PAY），银行面只有三值");

        ValuesOf(typeof(PartnerTransferPurposes)).Should().BeEquivalentTo(new[]
        {
            "GOODSPAYMENT", "COMMISSION", "REFUND", "REIMBURSEMENT", "FREIGHT", "OTHERS",
        });

        ValuesOf(typeof(PartnerTransferScenes)).Should().BeEquivalentTo(new[]
        {
            "ORDINARY_TRANSFER", "PAYROLL_CARD_TRANSFER",
        });

        ValuesOf(typeof(PartnerTransferAccountTypes)).Should().BeEquivalentTo(new[] { "WECHATPAY_ACCOUNT", "BANK_ACCOUNT" },
            "采官方**取值表**拼写 WECHATPAY_ACCOUNT：官方正文笔写 WXPAY_ACCOUNT 是残留，不建模");

        // 与银行面的分歧（正对照）：证明两域常量各自独立。
        ValuesOf(typeof(BankTransferBatchStates)).Should().NotContain("WAIT_PAY",
            "正对照：银行面确无 WAIT_PAY（出资方直接扣款）");
        ValuesOf(typeof(BankTransferDetailStateFilters)).Should().BeEquivalentTo(new[] { "ALL", "SUCCESS", "FAIL" });
        ValuesOf(typeof(PartnerTransferDetailStates)).Should().Contain("INIT");
        ValuesOf(typeof(BankTransferDetailStates)).Should().NotContain("INIT",
            "正对照：银行面明细无 INIT");

        ConstantClassNames().Should().BeEquivalentTo(new[]
        {
            nameof(PartnerTransferAuthorizationTypes),
            nameof(PartnerTransferBatchStates),
            nameof(PartnerTransferBatchTypes),
            nameof(PartnerTransferCloseReasons),
            nameof(PartnerTransferDetailStates),
            nameof(PartnerTransferDetailStateFilters),
            nameof(PartnerTransferPurposes),
            nameof(PartnerTransferScenes),
            nameof(PartnerTransferAccountTypes),
        }, "本域常量类恰 9 个（值集差异的权威清单）");
    }

    /// <summary>
    /// PTF-B5：本域<b>不建 <c>fail_reason</c> 常量类、不建专有错误码类</b>的裁决锁定 + 正对照。
    /// </summary>
    /// <remarks>
    /// 官方明细失败原因在本域是 <b>41 值</b>长表（银行面为其超集、且跨页重复列举），
    /// 且该字段<b>仅作诊断</b>（官方明令处置动作只按 <c>detail_status</c> 判别）⇒
    /// 建模为常量类会带来「能据码分支处置」的假象，且银行 / 服务商两域必然漂移。
    /// 与 <c>BS-B5</c> 同源裁决：<b>透传 <see cref="WechatPayResponse.Code"/> 与 <c>fail_reason</c> 原样字符串</b>。
    /// </remarks>
    [Fact]
    public void PartnerTransfer_ShouldNotDeclareFailReasonOrErrorCodeConstants()
    {
        ConstantClassNames().Should().NotContain(static n => n.Contains("FailReason", StringComparison.Ordinal),
            "fail_reason 是诊断字段（本域 41 值、银行面为其超集）：建模常量类即制造「可按码分支处置」的假象");
        DomainTypeNames().Should().NotContain(static n => n.EndsWith("ErrorCodes", StringComparison.Ordinal),
            "本域官方业务码全为 APIv3 通用名：建专有码常量类即制造「能识别本域专有错误」的假象");

        // 该字段仍在 DTO 上、且类型是 string（不建模取值 ≠ 丢掉字段）。
        JsonNameShouldBe<PartnerTransferDetailQueryResponse>(nameof(PartnerTransferDetailQueryResponse.FailReason), "fail_reason");
        typeof(PartnerTransferDetailQueryResponse).GetProperty(nameof(PartnerTransferDetailQueryResponse.FailReason))!
            .PropertyType.Should().Be(typeof(string));

        // 正对照：同口径扫描能发现本域确有 9 个常量类（NotContain 不是因为集合空而恒真）。
        ConstantClassNames().Should().HaveCount(9);
        // 正对照：进件域确有专有错误码类，两域判错面不同。
        ValuesOf(typeof(Mud.Wechat.Pay.DataModels.Apply4SubMerchant.Apply4SubMerchantErrorCodes))
            .Should().Contain("APPLYMENT_NOT_EXIST");

        var commonCodes = ValuesOf(typeof(Mud.Wechat.Pay.Abstractions.Enums.WechatPayErrorCodes));
        commonCodes.Should().Contain(new[] { "INVALID_REQUEST", "SYSTEM_ERROR", "FREQUENCY_LIMITED" },
            "本域官方业务码为通用名，既有承载面已覆盖");
    }

    /// <summary>
    /// PTF-B6：判错面继承 + 「表相同则共用、表不同则分建」的两处共用与四处不共用。
    /// </summary>
    [Fact]
    public void PartnerTransferResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch()
    {
        foreach (var responseType in new[]
                 {
                     typeof(PartnerTransferBatchCreateResponse),
                     typeof(PartnerTransferBatchQueryResponse),
                     typeof(PartnerTransferDetailQueryResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
            responseType.BaseType.Should().Be(typeof(WechatPayResponse),
                $"{responseType.Name} 必须直接继承 WechatPayResponse");
        }

        // 共用①②：微信单号支与商家单号支官方字段表逐字段相同 ⇒ 两支同返回类型。
        FindMethod(nameof(IWechatPayPartnerTransferService.QueryBatchByBatchIdAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayPartnerTransferService.QueryBatchByOutBatchNoAsync)).ReturnType,
            "两支批次查询同表 ⇒ 共用 PartnerTransferBatchQueryResponse");
        FindMethod(nameof(IWechatPayPartnerTransferService.QueryDetailByDetailIdAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayPartnerTransferService.QueryDetailByOutDetailNoAsync)).ReturnType,
            "两支明细查询同表 ⇒ 共用 PartnerTransferDetailQueryResponse");

        // 不共用①：发起应答三字段，与批次应答不可合并。
        DeclaredJsonNamesOf<PartnerTransferBatchCreateResponse>()
            .Should().BeEquivalentTo(new[] { "out_batch_no", "batch_id", "create_time" });

        // 不共用②：批次内明细项（三字段摘要）与明细查询应答（全字段）是两形态。
        DeclaredJsonNamesOf<PartnerTransferBatchDetailItem>()
            .Should().BeEquivalentTo(new[] { "detail_id", "out_detail_no", "detail_status" });
        DeclaredJsonNamesOf<PartnerTransferDetailQueryResponse>().Should().HaveCountGreaterThan(15,
            "明细查询应答是官方全字段表，不得与批次内摘要项共用");

        // 不共用③：请求侧明细项（含 openid / user_name）与应答侧摘要项分建。
        BodyTypeOf(nameof(IWechatPayPartnerTransferService.CreateBatchAsync))
            .GetProperty(nameof(PartnerTransferBatchCreateRequest.TransferDetailList))!
            .PropertyType.GetGenericArguments()[0]
            .Should().Be(typeof(PartnerTransferDetailInput));
        typeof(PartnerTransferDetailInput).Should().NotBeSameAs(typeof(PartnerTransferBatchDetailItem),
            "请求明细项有金额/备注/openid，应答摘要项只有单号与状态");

        // 不共用④（跨域）：与银行面同角色类型<b>不共用</b> —— 源生成的 JsonContext 按「域 == SerializerClassName」
        // 一一对应，跨域共用构件会让一个上下文引用他域类型（偏离记录见方案 §2.6 与本守卫 remarks）。
        typeof(PartnerTransferDetailInput).Should().NotBeSameAs(typeof(BankTransferDetailInput),
            "两域各建明细入参类型：字段名与必填集不同（本域 user_name 选填、银行面必填）");
        DeclaredJsonNamesOf<BankTransferDetailInput>().Should().Contain("user_name");
    }

    /// <summary>
    /// PTF-B7：本域 6 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void PartnerTransferDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(6, "服务商批量转账 DTO：请求 2 + 应答 3 + 摘要项 1");

        foreach (var type in domainTypes)
        {
            PartnerTransferJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PartnerTransferJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（本域类型 = 6 DTO + 9 常量类，上下文类已排除）。
        DomainTypeNames().Should().HaveCount(15, "本域类型 = 6 DTO + 9 常量类");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", 
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PartnerTransferJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 PartnerTransfer 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddPartnerTransferWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        var api = typeof(IWechatPayPartnerTransferService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay），复用默认实例会与企微线撞 BaseUrl");
        typeof(IWechatPayPartnerTransferService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线凭据为商户 RSA 私钥签名，不存在 access_token（红线 PAY-B1）");
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
        => typeof(IWechatPayPartnerTransferService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayPartnerTransferService.{methodName} 必须存在");

    private static Type BodyTypeOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Single(static p => p.GetCustomAttribute<PathAttribute>() == null
                                 && p.GetCustomAttribute<QueryAttribute>() == null
                                 && p.ParameterType != typeof(CancellationToken))
            .ParameterType;

    private static string[] QueryNamesOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
            .Select(static a => a.Name!)
            .ToArray();

    private static string[] PathKeysOf(string methodName)
        => FindMethod(methodName).GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name!)
            .ToArray();

    private static string[] NoContentMethodsOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(static m => m.ReturnType == typeof(Task))
            .Select(static m => m.Name)
            .ToArray();

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static IEnumerable<Type> DomainTypes()
        => typeof(PartnerTransferBatchCreateRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(PartnerTransferBatchCreateRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(PartnerTransferBatchCreateRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsAbstract && t.IsSealed)
            .Select(static t => t.Name)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
        => JsonNameShouldBe(typeof(T), propertyName, expectedJsonName);

    private static void JsonNameShouldBe(Type type, string propertyName, string expectedJsonName)
    {
        var property = type.GetProperty(propertyName);
        property.Should().NotBeNull($"{type.Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }

    private static string[] DeclaredJsonNamesOf<T>()
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n != null)
            .Select(static n => n!)
            .ToArray();

    private static string[] ValuesOf(Type constantsType)
        => constantsType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .ToArray();

    private static string SourcePath(params string[] segments)
        => Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName)
        => SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }
}

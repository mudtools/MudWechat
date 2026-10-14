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
/// P0-6 从业机构（银行）批量转账与电子回单域契约守卫（<b>9 端点</b>：转账 5 + 电子回单 4）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源</b>（合作伙伴文档中心「开发文档(银行/机构)」树，2026-10-14 逐页核验，
/// 各页「支持商户」栏恒为【从业机构（银行）】）：4012465889 发起批量转账、4012465891 微信支付批次单号查询批次单、
/// 4012465897 微信支付明细单号查询明细单、4012465903 商家批次单号查询批次单、4012465915 商家明细单号查询明细单；
/// 4012470746 受理汇总电子回单、4012470761 查询汇总电子回单、4012470770 受理明细电子回单、
/// 4012470775 查询明细电子回单。逐端点 URL 落在接口 XML 注释（本守卫不复述）。
/// </para>
/// <para>
/// <b>SKIT 清单本域 9 行 ⇒ 9 端点、零归并</b>。<b>最易数错的一处</b>：回单族的两条路径各有
/// POST（受理）与 GET（查受理结果）两支、路径字符串<b>完全相同</b> ⇒ 路由去重后只剩 7 条，
/// 按「路由集合」计数会把 9 数成 7。守卫 BT-B1 同时锁 <c>9</c>（含重复）与 <c>7</c>（去重）两个数，
/// 使该计数口径本身成为被守卫的契约。
/// </para>
/// <para>
/// <b>本守卫要守住的五处「顺手就会写错」</b>：
/// ① 两套前缀 <c>/v3/bank-transfer/</c> 与 <c>/v3/bank-batch-transfer/</c> <b>同域共存</b>，
/// 与服务商面的 <c>/v3/partner-transfer/</c> 单前缀形态不同；
/// ② 商户标识是<b>六层</b>（<c>bank_sp_mchid</c> / <c>platform_mchid</c> / <c>specialized_payment_mchid</c> /
/// <c>sponsor_mchid</c> / <c>user_authorized_mchid</c> / <c>user_authorized_appid</c>），
/// <b>没有</b> <c>sp_mchid</c> / <c>sub_mchid</c>；
/// ③ 计数与状态字段是 <c>*_count</c> 与 <c>batch_state</c> / <c>detail_state</c>（服务商面是 <c>*_num</c> /
/// <c>*_status</c>），查询过滤参数名同为 <c>detail_state</c>；
/// ④ 明细请求 <c>user_name</c> <b>必填</b>（服务商面选填），应答字段名是 <c>username</c>；
/// ⑤ 回单的 <c>download_url</c> 是<b>动态路由 + 非 JSON</b> 文件地址 ⇒ 只建模「取 URL」，
/// 不建模取文件（同账单下载通道的分工）。
/// </para>
/// </remarks>
public class WechatPayBankTransferContractGuards
{
    private const string RegistryGroupName = "BankTransfer";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.BankTransfer";

    private const string TransferPrefix = "/v3/bank-transfer/";

    private const string ReceiptPrefix = "/v3/bank-batch-transfer/";

    /// <summary>
    /// BT-B1：9 端点的方法 × 路由照官方原文（<b>同路径的 POST 与 GET 各算一支</b>），
    /// 查询参数名 <c>detail_state</c> 与回单 query 定位键逐支锁定。
    /// </summary>
    [Fact]
    public void BankTransferEndpoints_ShouldMatchOfficialRoutesAndParameterPositions()
    {
        // 转账族 5 支。
        AssertRoute<PostAttribute>(nameof(IWechatPayBankTransferService.CreateBatchAsync),
            TransferPrefix + "batches");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankTransferService.QueryBatchByBatchIdAsync),
            TransferPrefix + "batches/batch-id/{batchId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankTransferService.QueryDetailByDetailIdAsync),
            TransferPrefix + "batches/batch-id/{batchId}/details/detail-id/{detailId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankTransferService.QueryBatchByOutBatchNoAsync),
            TransferPrefix + "batches/out-batch-no/{outBatchNo}");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankTransferService.QueryDetailByOutDetailNoAsync),
            TransferPrefix + "batches/out-batch-no/{outBatchNo}/details/out-detail-no/{outDetailNo}");

        // 回单族 4 支：两条路径各有 POST（受理）与 GET（查受理结果）。
        AssertRoute<PostAttribute>(nameof(IWechatPayBankTransferService.AcceptSummaryReceiptAsync),
            ReceiptPrefix + "receipt/summary-receipts");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankTransferService.QuerySummaryReceiptAsync),
            ReceiptPrefix + "receipt/summary-receipts");
        AssertRoute<PostAttribute>(nameof(IWechatPayBankTransferService.AcceptDetailReceiptAsync),
            ReceiptPrefix + "receipt/detail-receipts");
        AssertRoute<GetAttribute>(nameof(IWechatPayBankTransferService.QueryDetailReceiptAsync),
            ReceiptPrefix + "receipt/detail-receipts");

        // 计数口径即契约：9 支（含同路径重复）vs 7 条去重路由。
        var routes = RoutesOf(typeof(IWechatPayBankTransferService));
        routes.Should().HaveCount(9,
            "本域 9 端点：回单族的两条路径各有 POST 与 GET（数量变化须同批更新 BT-B1 与 Tests/ContractBaseline.cs）");
        routes.Distinct().Should().HaveCount(7,
            "去重后只有 7 条路径 ⇒ 「按路由去重计数」会得 7 而漏掉两支 GET，基线数是 9");

        // 两支批次查询：过滤参数名是 detail_state（服务商面是 detail_status）。
        QueryNamesOf(nameof(IWechatPayBankTransferService.QueryBatchByBatchIdAsync))
            .Should().BeEquivalentTo(new[] { "need_query_detail", "offset", "limit", "detail_state" });
        QueryNamesOf(nameof(IWechatPayBankTransferService.QueryBatchByOutBatchNoAsync))
            .Should().BeEquivalentTo(new[] { "need_query_detail", "offset", "limit", "detail_state" });
        QueryNamesOf(nameof(IWechatPayBankTransferService.QueryBatchByBatchIdAsync))
            .Should().NotContain("detail_status", "本域官方过滤名是 detail_state，抄服务商面即静默丢过滤");

        // 回单两支 GET 的定位键全在 query，且与 POST 包体同名。
        QueryNamesOf(nameof(IWechatPayBankTransferService.QuerySummaryReceiptAsync))
            .Should().BeEquivalentTo(new[] { "out_batch_no" });
        QueryNamesOf(nameof(IWechatPayBankTransferService.QueryDetailReceiptAsync))
            .Should().BeEquivalentTo(new[] { "out_batch_no", "out_detail_no" });

        // 两支明细查询与发起支无 query；发起支只有一个包体参数。
        QueryNamesOf(nameof(IWechatPayBankTransferService.QueryDetailByDetailIdAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayBankTransferService.QueryDetailByOutDetailNoAsync)).Should().BeEmpty();
        QueryNamesOf(nameof(IWechatPayBankTransferService.CreateBatchAsync)).Should().BeEmpty();
        BodyTypeOf(nameof(IWechatPayBankTransferService.CreateBatchAsync))
            .Should().Be(typeof(BankTransferBatchCreateRequest));

        // path 定位键逐支锁定。
        PathKeysOf(nameof(IWechatPayBankTransferService.QueryBatchByBatchIdAsync))
            .Should().BeEquivalentTo(new[] { "batchId" });
        PathKeysOf(nameof(IWechatPayBankTransferService.QueryBatchByOutBatchNoAsync))
            .Should().BeEquivalentTo(new[] { "outBatchNo" });
        PathKeysOf(nameof(IWechatPayBankTransferService.QueryDetailByDetailIdAsync))
            .Should().BeEquivalentTo(new[] { "batchId", "detailId" });
        PathKeysOf(nameof(IWechatPayBankTransferService.QueryDetailByOutDetailNoAsync))
            .Should().BeEquivalentTo(new[] { "outBatchNo", "outDetailNo" });
        PathKeysOf(nameof(IWechatPayBankTransferService.QuerySummaryReceiptAsync)).Should().BeEmpty();
        PathKeysOf(nameof(IWechatPayBankTransferService.QueryDetailReceiptAsync)).Should().BeEmpty();

        // GET 一律无请求体（回单两支 GET 的定位键在 query，不在包体）。
        foreach (var method in typeof(IWechatPayBankTransferService)
                     .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                     .Where(static m => m.GetCustomAttribute<GetAttribute>() != null))
        {
            method.GetParameters().Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
                $"{method.Name} 是 GET，官方无请求体");
        }

        // 本域九支全部有 JSON 应答 ⇒ 零 204。
        NoContentMethodsOf(typeof(IWechatPayBankTransferService)).Should().BeEmpty(
            "本域官方无「无应答包体」端点（回单受理也回字段表）");
    }

    /// <summary>
    /// BT-B2：双前缀归属（转账 5 + 回单 4）+ 三处「官方非本面」零声明（回单文件下载 / 他形态回单 /
    /// 余额提现账单来账），每处均须正对照。
    /// </summary>
    [Fact]
    public void BankTransferRoutes_ShouldSplitAcrossTwoPrefixesAndNotDeclareDownloadOrForeignGroups()
    {
        var routes = RoutesOf(typeof(IWechatPayBankTransferService));

        routes.Count(static r => r.StartsWith(TransferPrefix, StringComparison.Ordinal))
            .Should().Be(5, "转账族五支全在 /v3/bank-transfer/ 下");
        routes.Count(static r => r.StartsWith(ReceiptPrefix, StringComparison.Ordinal))
            .Should().Be(4, "回单族四支全在 /v3/bank-batch-transfer/ 下");

        // 零声明①：download_url 的文件下载不进生成式接口（动态路由 + 响应非 JSON）。
        routes.Should().NotContain(static r => r.Contains("download", StringComparison.Ordinal),
            "回单文件经 download_url 直取，该地址由应答动态给出且响应非 JSON ⇒ 不得声明成生成式端点");
        FindMethod(nameof(IWechatPayBankTransferService.QuerySummaryReceiptAsync)).GetParameters()
            .Should().NotContain(static p => p.Name == "downloadUrl",
                "不得把 download_url 做成入参（它是应答产物，不是定位键）");
        // 正对照：该字段确在应答上（「不建模取文件」≠「丢掉取 URL 的能力」）。
        DeclaredJsonNamesOf<BankTransferSummaryReceiptResponse>().Should().Contain("download_url");
        DeclaredJsonNamesOf<BankTransferDetailReceiptResponse>().Should().Contain("download_url");
        // 正对照：本线唯一的「非生成式下载通道」形态是账单下载服务，本域不得另起同款。
        typeof(IWechatPayBankTransferService).Assembly.GetTypes()
            .Select(static t => t.Name)
            .Should().Contain("IWechatPayBillDownloadService",
                "正对照：账单下载通道的形态确在（本域零声明不是因为找不到该类型）");
        typeof(IWechatPayBankTransferService).Assembly.GetTypes()
            .Select(static t => t.Name)
            .Should().NotContain(static n => n.Contains("ReceiptDownload", StringComparison.Ordinal),
                "本域不建回单下载服务：回单文件的取用与校验属宿主（URL + hash_value 已给出）");

        // 零声明②：他形态回单（直连 / 服务商面的 bill-receipt、electronic-receipts）不并入本域。
        routes.Should().NotContain(static r => r.Contains("bill-receipt", StringComparison.Ordinal)
                                               || r.Contains("electronic-receipts", StringComparison.Ordinal),
            "直连与服务商面的回单路由无 bank- 段，属另一族 ⇒ 须单独立域再逐页取证");
        RoutesOf(typeof(IWechatPayTransferService))
            .Should().Contain(static r => r.Contains("/elecsign/", StringComparison.Ordinal),
                "正对照：直连面确有电子凭证族端点（在 fund-app 前缀下），本域零声明是有据裁决");

        // 零声明③：余额查询 / 余额提现 / 资金账单 / 来账识别属他章。
        routes.Should().NotContain(static r => (r.Contains("/fund", StringComparison.Ordinal)
                                                && !r.StartsWith(ReceiptPrefix, StringComparison.Ordinal))
                                              || r.Contains("/bill", StringComparison.Ordinal)
                                              || r.Contains("deposit", StringComparison.Ordinal),
            "资金与账单、来账能力不属本章：/v3/merchant/fund 另受守卫 C1 全域零声明约束");
        RoutesOf(typeof(IWechatPayBillService))
            .Should().Contain(static r => r.StartsWith("/v3/bill/", StringComparison.Ordinal),
                "正对照：账单域路由确在 /v3/bill/（本域不复用账单通道）");

        // 与相邻两转账域两两互斥。
        routes.Intersect(RoutesOf(typeof(IWechatPayPartnerTransferService))).Should().BeEmpty("银行面与服务商面路由不得重叠");
        routes.Intersect(RoutesOf(typeof(IWechatPayTransferService))).Should().BeEmpty("银行面与直连面路由不得重叠");

        Directory.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"), "Interfaces", "BankTransfer"))
            .Should().BeTrue("正对照：Interfaces/BankTransfer/ 确实已落位");
        File.Exists(Path.Combine(SourceProjectDir("Mud.Wechat.Pay"),
            "Interfaces", "BankTransferReceipt", "IWechatPayBankTransferReceiptService.cs"))
            .Should().BeFalse("回单四支与转账五支同域共置（官方同树、同商户形态），不另立 BankTransferReceipt 域");
    }

    /// <summary>
    /// BT-B3：官方字段名逐项锁定（<b>六层商户标识</b>、无 <c>sp_</c> / <c>sub_</c>）+
    /// 与服务商面的字段名分歧 + 时间字段恒 <c>string</c>。
    /// </summary>
    [Fact]
    public void BankTransferDtos_ShouldExposeOfficialFieldNamesAndDivergeFromPartnerDomain()
    {
        // 六层标识（请求五层 + 应答另加 bank_sp_mchid）。
        foreach (var name in new[] { "platform_mchid", "specialized_payment_mchid", "sponsor_mchid", "user_authorized_mchid", "user_authorized_appid" })
        {
            JsonNamesOf(typeof(BankTransferBatchCreateRequest)).Should().Contain(name,
                $"请求体缺 {name} 即丢掉一层必填商户标识");
        }

        JsonNameShouldBe<BankTransferBatchCreateRequest>(nameof(BankTransferBatchCreateRequest.TotalCount), "total_count");
        JsonNameShouldBe<BankTransferBatchCreateRequest>(nameof(BankTransferBatchCreateRequest.AttachRemark), "attach_remark");
        JsonNameShouldBe<BankTransferBatchCreateRequest>(nameof(BankTransferBatchCreateRequest.TransferScene), "transfer_scene");
        JsonNameShouldBe<BankTransferDetailInput>(nameof(BankTransferDetailInput.UserName), "user_name");
        JsonNameShouldBe<BankTransferDetailQueryResponse>(nameof(BankTransferDetailQueryResponse.UserName), "username");
        JsonNameShouldBe<BankTransferBatchQueryResponse>(nameof(BankTransferBatchQueryResponse.BankSpMchId), "bank_sp_mchid");
        JsonNameShouldBe<BankTransferBatchQueryResponse>(nameof(BankTransferBatchQueryResponse.BatchState), "batch_state");
        JsonNameShouldBe<BankTransferDetailQueryResponse>(nameof(BankTransferDetailQueryResponse.DetailState), "detail_state");
        JsonNameShouldBe<BankTransferDetailQueryResponse>(nameof(BankTransferDetailQueryResponse.InitiateTime), "initiate_time");
        JsonNameShouldBe<BankTransferSummaryReceiptResponse>(nameof(BankTransferSummaryReceiptResponse.ReceiptState), "receipt_state");
        JsonNameShouldBe<BankTransferSummaryReceiptResponse>(nameof(BankTransferSummaryReceiptResponse.HashValue), "hash_value");

        // 本域<b>没有</b> sp_ / sub_ 形态标识（正对照：服务商面有）。
        var allJsonNames = DomainTypes().SelectMany(static t => JsonNamesOf(t)).ToArray();
        allJsonNames.Should().NotContain(static n => n.StartsWith("sp_", StringComparison.Ordinal),
            "从业机构面不存在 sp_ 前缀字段");
        allJsonNames.Should().NotContain(static n => n == "sub_mchid" || n == "sub_appid",
            "从业机构面用六层标识，不存在服务商面的 sub_mchid / sub_appid");
        allJsonNames.Should().NotBeEmpty("参照集为空会让上面的零声明断言静默恒真");
        JsonNamesOf(typeof(PartnerTransferBatchCreateRequest)).Should().Contain(new[] { "sub_mchid", "sub_appid", "sp_appid" },
            "正对照：服务商面确有 sub_/sp_ 三件套");

        // 与服务商面的计数 / 状态名差（双向）。
        DeclaredJsonNamesOf<BankTransferBatchQueryResponse>().Should()
            .Contain(new[] { "total_count", "success_count", "fail_count", "batch_state" })
            .And.NotContain(new[] { "total_num", "success_num", "fail_num", "batch_status" });
        DeclaredJsonNamesOf<PartnerTransferBatchQueryResponse>().Should().Contain("total_num",
            "正对照：服务商面确用 *_num");

        // 批次内摘要项：本域多一个 fail_reason（服务商面摘要项只有三字段）。
        DeclaredJsonNamesOf<BankTransferBatchDetailItem>()
            .Should().BeEquivalentTo(new[] { "detail_id", "out_detail_no", "detail_state", "fail_reason" });
        DeclaredJsonNamesOf<PartnerTransferBatchDetailItem>().Should().NotContain("fail_reason",
            "正对照：服务商面批次摘要项无 fail_reason（失败原因只在明细查询应答）");

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
    /// BT-B4：官方取值集合逐项锁定（8 个常量类）+ 与服务商面的四处值集分歧 +
    /// 本页<b>无 account_type 取值表</b>故本域不建该常量类的裁决。
    /// </summary>
    [Fact]
    public void BankTransferEnumConstants_ShouldMatchOfficialValuesAndDivergeFromPartnerDomain()
    {
        ValuesOf(typeof(BankTransferScenes)).Should().BeEquivalentTo(new[]
        {
            "ORDINARY_TRANSFER", "PAYROLL_CARD_TRANSFER",
        }, "与服务商面同值集，但本面为必填（差异在接口签名，不在值集）");

        ValuesOf(typeof(BankTransferPurposes)).Should().BeEquivalentTo(new[]
        {
            "GOODSPAYMENT", "COMMISSION", "REFUND", "REIMBURSEMENT", "FREIGHT", "OTHERS",
        });

        ValuesOf(typeof(BankTransferBatchStates)).Should().BeEquivalentTo(new[]
        {
            "ACCEPTED", "PROCESSING", "FINISHED", "CLOSED",
        }, "本域批次状态四值：无 WAIT_PAY（出资方直接扣款，无付款确认环节）");

        ValuesOf(typeof(BankTransferCloseReasons)).Should().BeEquivalentTo(new[]
        {
            "MERCHANT_REVOCATION", "OVERDUE_CLOSE",
        }, "比服务商面少 PAYER_ACCOUNT_ABNORMAL（本面以 ACCOUNT_ERROR 错误码承载）");

        ValuesOf(typeof(BankTransferDetailStates)).Should().BeEquivalentTo(new[]
        {
            "PROCESSING", "SUCCESS", "FAIL", "REFUND",
        }, "本域明细四值：无 INIT 与 WAIT_PAY");

        ValuesOf(typeof(BankTransferDetailStateFilters)).Should().BeEquivalentTo(new[] { "ALL", "SUCCESS", "FAIL" },
            "过滤三值（服务商面四值，含 WAIT_PAY）");

        ValuesOf(typeof(BankTransferReceiptStates)).Should().BeEquivalentTo(new[] { "ACCEPTED", "FINISHED" },
            "hash / download_url 仅在 FINISHED 时出现 ⇒ 受理后立即读 URL 必为 null");

        ValuesOf(typeof(BankTransferReceiptHashTypes)).Should().BeEquivalentTo(new[] { "SHA256" },
            "官方「当前版本使用 SHA256」⇒ 本类不是穷尽集合，比较须兼容新增值");

        // 与服务商面的分歧（正对照）。
        ValuesOf(typeof(PartnerTransferBatchStates)).Should().Contain("WAIT_PAY");
        ValuesOf(typeof(PartnerTransferCloseReasons)).Should().Contain("PAYER_ACCOUNT_ABNORMAL");
        ValuesOf(typeof(PartnerTransferDetailStates)).Should().Contain(new[] { "INIT", "WAIT_PAY" });

        // 裁决：官方本页 account_type 无取值表 ⇒ 本域不建 AccountTypes 常量类（不做穷尽校验）。
        ConstantClassNames().Should().NotContain(BankTransferAccountTypesPlaceholder,
            "本域不得出现账户类型常量类：官方明细查询页未附取值表");
        JsonNameShouldBe<BankTransferDetailQueryResponse>(nameof(BankTransferDetailQueryResponse.AccountType), "account_type");
        // 正对照：服务商面有官方取值表，故有该类（且值是 WECHATPAY_ACCOUNT/BANK_ACCOUNT）。
        ValuesOf(typeof(PartnerTransferAccountTypes)).Should().BeEquivalentTo(new[] { "WECHATPAY_ACCOUNT", "BANK_ACCOUNT" });

        ConstantClassNames().Should().BeEquivalentTo(new[]
        {
            nameof(BankTransferScenes),
            nameof(BankTransferPurposes),
            nameof(BankTransferBatchStates),
            nameof(BankTransferCloseReasons),
            nameof(BankTransferDetailStates),
            nameof(BankTransferDetailStateFilters),
            nameof(BankTransferReceiptStates),
            nameof(BankTransferReceiptHashTypes),
        }, "本域常量类恰 8 个（值集差异的权威清单）");
    }

    /// <summary>
    /// BT-B5：本域<b>不建 <c>fail_reason</c> 常量类、不建专有错误码类</b>的裁决锁定 + 正对照。
    /// </summary>
    /// <remarks>
    /// 本域失败原因取值表是服务商面 41 值的<b>超集</b>（官方跨多页重复列举，本次取证未见完整表），
    /// 且官方明令处置动作按 <c>detail_state</c> 判别（<c>FAIL</c> 才可对该笔重发）⇒ 与 <c>PTF-B5</c> /
    /// <c>BS-B5</c> 同源裁决：原样透传字符串，不建常量类。
    /// </remarks>
    [Fact]
    public void BankTransfer_ShouldNotDeclareFailReasonOrErrorCodeConstants()
    {
        ConstantClassNames().Should().NotContain(static n => n.Contains("FailReason", StringComparison.Ordinal),
            "fail_reason 是诊断字段（本面为服务商面 41 值的超集）：建模常量类即制造「可按码分支处置」的假象");
        DomainTypeNames().Should().NotContain(static n => n.EndsWith("ErrorCodes", StringComparison.Ordinal),
            "本域官方业务码全为 APIv3 通用名（含两行不同文案的 NO_AUTH）：不建专有码常量类");

        // 该字段仍在两处 DTO 上、类型是 string。
        DeclaredJsonNamesOf<BankTransferBatchDetailItem>().Should().Contain("fail_reason");
        JsonNameShouldBe<BankTransferDetailQueryResponse>(nameof(BankTransferDetailQueryResponse.FailReason), "fail_reason");
        typeof(BankTransferDetailQueryResponse).GetProperty(nameof(BankTransferDetailQueryResponse.FailReason))!
            .PropertyType.Should().Be(typeof(string));

        ConstantClassNames().Should().HaveCount(8, "正对照：同口径扫描能数出 8 个常量类");

        var commonCodes = ValuesOf(typeof(Mud.Wechat.Pay.Abstractions.Enums.WechatPayErrorCodes));
        commonCodes.Should().Contain(new[] { "NO_AUTH", "NOT_ENOUGH", "SYSTEM_ERROR", "FREQUENCY_LIMITED" },
            "本域官方业务码为通用名，既有承载面已覆盖");
    }

    /// <summary>
    /// BT-B6：判错面继承 + 「表相同则共用、表不同则分建」的四处共用与三处不共用。
    /// </summary>
    [Fact]
    public void BankTransferResponses_ShouldDeriveFromWechatPayResponseAndShareTypesOnlyWhenTablesMatch()
    {
        foreach (var responseType in new[]
                 {
                     typeof(BankTransferBatchCreateResponse),
                     typeof(BankTransferBatchQueryResponse),
                     typeof(BankTransferDetailQueryResponse),
                     typeof(BankTransferSummaryReceiptResponse),
                     typeof(BankTransferDetailReceiptResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
            responseType.BaseType.Should().Be(typeof(WechatPayResponse),
                $"{responseType.Name} 必须直接继承 WechatPayResponse");
        }

        // 共用①②：批次两支、明细两支官方字段表逐字段相同。
        FindMethod(nameof(IWechatPayBankTransferService.QueryBatchByBatchIdAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayBankTransferService.QueryBatchByOutBatchNoAsync)).ReturnType);
        FindMethod(nameof(IWechatPayBankTransferService.QueryDetailByDetailIdAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayBankTransferService.QueryDetailByOutDetailNoAsync)).ReturnType);

        // 共用③④：回单的受理支与查询支官方字段表相同（POST 与 GET 同返回类型）。
        FindMethod(nameof(IWechatPayBankTransferService.AcceptSummaryReceiptAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayBankTransferService.QuerySummaryReceiptAsync)).ReturnType,
            "汇总回单 POST/GET 同表 ⇒ 共用一型（若按方法分建即造出「受理有别于查询」的假象）");
        FindMethod(nameof(IWechatPayBankTransferService.AcceptDetailReceiptAsync)).ReturnType
            .Should().Be(FindMethod(nameof(IWechatPayBankTransferService.QueryDetailReceiptAsync)).ReturnType);

        // 不共用①：发起应答三字段。
        DeclaredJsonNamesOf<BankTransferBatchCreateResponse>()
            .Should().BeEquivalentTo(new[] { "out_batch_no", "batch_id", "create_time" });

        // 不共用②：汇总回单应答有 create_time / update_time，明细回单应答没有 ⇒ 两型分建。
        DeclaredJsonNamesOf<BankTransferSummaryReceiptResponse>()
            .Should().Contain(new[] { "create_time", "update_time", "out_batch_no" })
            .And.NotContain("out_detail_no");
        DeclaredJsonNamesOf<BankTransferDetailReceiptResponse>()
            .Should().NotContain(new[] { "create_time", "update_time" })
            .And.Contain("out_detail_no");
        typeof(BankTransferSummaryReceiptResponse).Should().NotBeSameAs(typeof(BankTransferDetailReceiptResponse),
            "两应答表不同 ⇒ 不得共用");

        // 不共用③：回单两支的请求类型不同形（明细支多一个定位键）。
        BodyTypeOf(nameof(IWechatPayBankTransferService.AcceptSummaryReceiptAsync))
            .Should().Be(typeof(BankTransferSummaryReceiptRequest));
        DeclaredJsonNamesOf<BankTransferSummaryReceiptRequest>().Should().BeEquivalentTo(new[] { "out_batch_no" });
        DeclaredJsonNamesOf<BankTransferDetailReceiptRequest>()
            .Should().BeEquivalentTo(new[] { "out_batch_no", "out_detail_no" });

        // 不共用④（跨域）：与服务商面同角色类型各建一套。
        BodyTypeOf(nameof(IWechatPayBankTransferService.CreateBatchAsync))
            .GetProperty(nameof(BankTransferBatchCreateRequest.TransferDetailList))!
            .PropertyType.GetGenericArguments()[0].Should().Be(typeof(BankTransferDetailInput));
        typeof(BankTransferDetailInput).Should().NotBeSameAs(typeof(PartnerTransferDetailInput),
            "两域各建明细入参：本域 user_name 必填、服务商面选填，且跨域共用会破坏「一域一 JsonContext」");
    }

    /// <summary>
    /// BT-B7：本域 10 个 DTO 全量登记进 AOT 上下文 + 三段式注册接线齐备 + 域内零 <c>[Token]</c>。
    /// </summary>
    [Fact]
    public void BankTransferDataModels_ShouldBeRegisteredInJsonContextAndDomainFullyWired()
    {
        var domainTypes = DomainTypes().ToList();

        domainTypes.Should().HaveCount(10,
            "从业机构转账 DTO：转账族 6（请求 2 + 应答 3 + 摘要项 1）+ 回单族 4");

        foreach (var type in domainTypes)
        {
            BankTransferJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 BankTransferJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 常量类不得被标注（本域类型 = 10 DTO + 8 常量类，上下文类已排除）。
        DomainTypeNames().Should().HaveCount(18, "本域类型 = 10 DTO + 8 常量类");

        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("BankTransferJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");

        Enum.GetNames(typeof(PayModule)).Should().Contain(RegistryGroupName,
            "PayModule 须有 BankTransfer 成员（漏挂即 AddModules 静默不注册）");
        var builderSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay", "Extensions", "PayServiceBuilder.cs"));
        builderSource.Should().Contain("AddBankTransferWebApiHttpClient()",
            "注册委托必须指向源生成方法（编译期已证其存在，此处锁「真的被挂上」）");

        var api = typeof(IWechatPayBankTransferService)
            .GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api).Should().Be(RegistryGroupName);
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName,
                "本域必须绑支付专用命名客户端（wechat-pay）");
        typeof(IWechatPayBankTransferService)
            .GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线凭据为商户 RSA 私钥签名，不存在 access_token（红线 PAY-B1）");
    }

    // ---- helpers -------------------------------------------------------------

    /// <summary>占位用类型名：断言「本域不存在该常量类」时引用其字面名（不建该类型本身）。</summary>
    private const string BankTransferAccountTypesPlaceholder = "BankTransferAccountTypes";

    private static void AssertRoute<TAttribute>(string methodName, string route)
        where TAttribute : HttpMethodAttribute
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<TAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明 {typeof(TAttribute).Name}");
        attribute!.RequestUri.Should().Be(route, $"{methodName} 路由必须与官方契约一致");
    }

    private static MethodInfo FindMethod(string methodName)
        => typeof(IWechatPayBankTransferService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayBankTransferService.{methodName} 必须存在");

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
        => typeof(BankTransferBatchCreateRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t));

    private static string[] DomainTypeNames()
        => typeof(BankTransferBatchCreateRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace && t.IsPublic
                               && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .Select(static t => t.Name)
            .ToArray();

    private static string[] ConstantClassNames()
        => typeof(BankTransferBatchCreateRequest).Assembly.GetTypes()
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

    private static string[] JsonNamesOf(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
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

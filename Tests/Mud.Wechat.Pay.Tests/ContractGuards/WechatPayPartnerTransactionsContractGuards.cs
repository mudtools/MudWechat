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
using Mud.Wechat.Pay.DataModels.Common;
using Mud.Wechat.Pay.DataModels.PartnerTransactions;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P0-3 服务商基础支付 / 分期域契约守卫（<b>9 端点</b>：下单五族 + 双路查单 + 关单 + 分期曝光咨询）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方事实来源（服务商文档中心，2026-10-14 逐页核验）</b>：JSAPI/小程序下单
/// <c>pay.weixin.qq.com/doc/v3/partner/4012714911</c>、Native 下单 <c>…/4012714902</c>、
/// APP 下单 <c>…/4012714669</c>、H5 下单 <c>…/4012714759</c>、扫码支付下单 <c>…/4012492476</c>、
/// 订单号查询 <c>…/4012738964</c>、商户订单号查询 <c>…/4012739008</c>、关闭订单
/// <c>…/4012739019</c>（成功应答 <b>204 无包体</b>）、分期曝光咨询 <c>…/4016196820</c>
/// （<b>频率 200 次/秒</b>）。各端点的产品族分身页字段表与主页逐项一致。
/// </para>
/// <para>
/// <b>SKIT 清单 12 行 ⇒ 本域 9 端点的三处裁定</b>（守卫 PT-B2 以「零声明 + 正对照」锁定）：
/// ① <c>/pay/partner/transactions/codepay</c> 与 ② <c>…/out-trade-no/{out_trade_no}/reverse</c>
/// 的现行正文是<b>付款码支付（V2）</b>指引页、无 APIv3 字段表 ⇒ 归 APIv2 批次；
/// ③ <c>/pay/partner/transactions/exposure</c> 页面<b>已下架</b>（302 到 50x，两次独立核验）⇒
/// 曝光能力只由 <c>/v3/pay/instal/partner/exposure</c> 承载。
/// 另：查单与关单的多个 docId 是<b>同一端点</b>的产品族分身页，各落 1 个方法（12 行里 6 行是分身页）。
/// </para>
/// <para>
/// <b>本守卫要守住的五处「顺手就会写错」</b>（多数源自与直连域 <c>Transactions</c> 的形似神异）：
/// ① 商户标识四分（<c>sp_</c>/<c>sub_</c> 前缀），把直连的 <c>appid</c>/<c>mchid</c> 抄进来即 400；
/// ② <c>payer</c> <b>只有 JSAPI 与扫码支付两族有</b>，Native/APP/H5 加上即多余字段；
/// ③ <c>support_fapiao</c> 在四族位于 <c>settle_info</c> 内、<b>唯扫码支付在请求顶层</b>；
/// ④ 三套 <c>scene_info</c> / 两套 <c>store_info</c> <b>不可共用一个类型</b>
/// （H5 多 <c>h5_info</c>、扫码支付的 <c>store_info</c> 只有 <c>id</c>/<c>out_id</c> 且必填）；
/// ⑤ 关单请求体<b>没有任何 AppID</b>，查单的 <c>sp_mchid</c> + <c>sub_mchid</c> 走 <b>Query</b> 而非 body。
/// </para>
/// </remarks>
public class WechatPayPartnerTransactionsContractGuards
{
    private const string RegistryGroupName = "PartnerTransactions";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.PartnerTransactions";

    private const string TransactionsPrefix = "/v3/pay/partner/transactions/";

    /// <summary>
    /// PT-B1：9 端点的方法与路由照官方原文；path / query / body 三种位置不可互换。
    /// </summary>
    [Fact]
    public void PartnerTransactionsEndpoints_ShouldMatchOfficialRoutes()
    {
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerTransactionsService.CreateJsapiOrderAsync),
            TransactionsPrefix + "jsapi");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerTransactionsService.CreateNativeOrderAsync),
            TransactionsPrefix + "native");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerTransactionsService.CreateAppOrderAsync),
            TransactionsPrefix + "app");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerTransactionsService.CreateH5OrderAsync),
            TransactionsPrefix + "h5");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerTransactionsService.CreateScannedPosOrderAsync),
            TransactionsPrefix + "scannedpos");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerTransactionsService.QueryByTransactionIdAsync),
            TransactionsPrefix + "id/{transactionId}");
        AssertRoute<GetAttribute>(nameof(IWechatPayPartnerTransactionsService.QueryByOutTradeNoAsync),
            TransactionsPrefix + "out-trade-no/{outTradeNo}");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerTransactionsService.CloseOrderAsync),
            TransactionsPrefix + "out-trade-no/{outTradeNo}/close");
        AssertRoute<PostAttribute>(nameof(IWechatPayPartnerTransactionsService.GetInstalExposureAdviceAsync),
            "/v3/pay/instal/partner/exposure");

        // 两个查单端点：官方把 sp_mchid 与 sub_mchid 都放在 **Query**（直连域只有一个 mchid）。
        foreach (var name in new[]
                 {
                     nameof(IWechatPayPartnerTransactionsService.QueryByTransactionIdAsync),
                     nameof(IWechatPayPartnerTransactionsService.QueryByOutTradeNoAsync),
                 })
        {
            var parameters = FindMethod(name).GetParameters();
            parameters
                .SelectMany(static p => p.GetCustomAttributes<QueryAttribute>())
                .Select(static a => a.Name)
                .Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" },
                    $"{name} 的两个商户号均为官方必填 Query 参数，不得下沉进 body 或改成 path 段");
            parameters
                .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
                .Select(static p => p.Name)
                .Should().HaveCount(1, $"{name} 只有一个 path 定位键");
            parameters
                .Should().NotContain(static p => p.GetCustomAttribute<BodyAttribute>() != null,
                    $"{name} 是 GET，官方无请求体");
        }

        // 关单：一个 path 键 + 一个 body（body 只有两个商户号字段，见 PT-B3 / PT-B4）。
        var close = FindMethod(nameof(IWechatPayPartnerTransactionsService.CloseOrderAsync));
        close.GetParameters()
            .Where(static p => p.GetCustomAttribute<PathAttribute>() != null)
            .Select(static p => p.Name)
            .Should().BeEquivalentTo(new[] { "outTradeNo" });
        close.GetParameters()
            .Select(static p => p.ParameterType)
            .Should().Contain(typeof(PartnerCloseOrderRequest));

        // 五个下单族与曝光咨询都是「纯 body」端点：无 path、无 query。
        foreach (var name in new[]
                 {
                     nameof(IWechatPayPartnerTransactionsService.CreateJsapiOrderAsync),
                     nameof(IWechatPayPartnerTransactionsService.CreateNativeOrderAsync),
                     nameof(IWechatPayPartnerTransactionsService.CreateAppOrderAsync),
                     nameof(IWechatPayPartnerTransactionsService.CreateH5OrderAsync),
                     nameof(IWechatPayPartnerTransactionsService.CreateScannedPosOrderAsync),
                     nameof(IWechatPayPartnerTransactionsService.GetInstalExposureAdviceAsync),
                 })
        {
            var parameters = FindMethod(name).GetParameters();
            parameters.Should().NotContain(static p => p.GetCustomAttribute<PathAttribute>() != null,
                $"{name} 官方无 path 参数");
            parameters.Should().NotContain(static p => p.GetCustomAttribute<QueryAttribute>() != null,
                $"{name} 官方无 query 参数");
        }
    }

    /// <summary>
    /// PT-B2：前缀纪律（8 条 transactions + 1 条 instal）+ 三条<b>零声明</b>路由 + 与直连域不撞车。
    /// </summary>
    [Fact]
    public void PartnerTransactionsRoutes_ShouldStayOnPartnerPrefixAndNotDeclareV2OrDelistedEndpoints()
    {
        var routes = RoutesOf(typeof(IWechatPayPartnerTransactionsService));

        routes.Should().HaveCount(9, "本域 9 端点（数量变化须同批更新 PT-B1 与 Tests/ContractBaseline.cs）");

        routes.Count(static r => r.StartsWith(TransactionsPrefix, StringComparison.Ordinal))
            .Should().Be(8, "下单五族 + 双路查单 + 关单共 8 条在 /v3/pay/partner/transactions/ 下");
        routes.Should().Contain("/v3/pay/instal/partner/exposure",
            "分期曝光咨询在 /v3/pay/instal/ 前缀下，是本域唯一例外");

        // 三条本轮不落端点的裁定：V2 两支归 APIv2 批次、下架页零端点 ⇒ 源码里只允许出现在注释中。
        routes.Should().NotContain(static r => r.Contains("codepay", StringComparison.Ordinal),
            "/pay/partner/transactions/codepay 现行官方正文是付款码支付（V2）指引，无 APIv3 字段表");
        routes.Should().NotContain(static r => r.EndsWith("/reverse", StringComparison.Ordinal),
            "…/out-trade-no/{out_trade_no}/reverse 同为 V2 面，本域不得声明");
        routes.Should().NotContain(static r => r == "/v3/pay/partner/transactions/exposure",
            "transactions/exposure 页面已下架（302→50x），曝光只落 instal/partner/exposure");

        // 正对照：三条字符串确实在本线源码中**被提及**（注释里），只是不成为路由——否则本断言会因扫描根漂移而恒真。
        var sourceText = string.Join("\n", CollectPaySourceFiles().Select(static path => File.ReadAllText(path)));
        sourceText.Should().Contain("codepay", "正对照：V2 付款码支付在文档注释里有记录性提及");
        sourceText.Should().Contain("reverse", "正对照：V2 撤销接口在文档注释里有记录性提及");

        // 与直连域不撞车：本域全部路由带 partner 段，直连域全部不带 ⇒ 两个集合必须互斥。
        var directRoutes = RoutesOf(typeof(IWechatPayTransactionsService));
        directRoutes.Should().OnlyContain(static r => r.StartsWith("/v3/pay/transactions/", StringComparison.Ordinal),
            "直连域前缀是 /v3/pay/transactions/（无 partner 段）");
        directRoutes.Intersect(routes).Should().BeEmpty("两线路由不得重叠，否则同一方法名会注册到不同 BaseUrl 的同一路径");
    }

    /// <summary>PT-B3：官方字段名逐项锁定（照原文拼写，含 <c>sp_</c>/<c>sub_</c> 前缀与下划线形态）。</summary>
    [Fact]
    public void PartnerTransactionsDtos_ShouldExposeOfficialFieldNames()
    {
        // 下单请求公共骨架：商户标识四分 + 订单主字段。
        JsonNameShouldBe<PartnerJsapiPrepayRequest>(nameof(PartnerJsapiPrepayRequest.SpAppId), "sp_appid");
        JsonNameShouldBe<PartnerJsapiPrepayRequest>(nameof(PartnerJsapiPrepayRequest.SpMchId), "sp_mchid");
        JsonNameShouldBe<PartnerJsapiPrepayRequest>(nameof(PartnerJsapiPrepayRequest.SubAppId), "sub_appid");
        JsonNameShouldBe<PartnerJsapiPrepayRequest>(nameof(PartnerJsapiPrepayRequest.SubMchId), "sub_mchid");
        JsonNameShouldBe<PartnerJsapiPrepayRequest>(nameof(PartnerJsapiPrepayRequest.OutTradeNo), "out_trade_no");
        JsonNameShouldBe<PartnerJsapiPrepayRequest>(nameof(PartnerJsapiPrepayRequest.NotifyUrl), "notify_url");
        JsonNameShouldBe<PartnerJsapiPrepayRequest>(nameof(PartnerJsapiPrepayRequest.GoodsTag), "goods_tag");
        JsonNameShouldBe<PartnerJsapiPrepayRequest>(nameof(PartnerJsapiPrepayRequest.TimeExpire), "time_expire");

        // 结算 / 支付者 / 场景：三处都是「服务商形态专有命名」。
        JsonNameShouldBe<PartnerSettleInfo>(nameof(PartnerSettleInfo.ProfitSharing), "profit_sharing");
        JsonNameShouldBe<PartnerSettleInfo>(nameof(PartnerSettleInfo.SubsidyAmount), "subsidy_amount");
        JsonNameShouldBe<PartnerPayerInfo>(nameof(PartnerPayerInfo.SpOpenId), "sp_openid");
        JsonNameShouldBe<PartnerPayerInfo>(nameof(PartnerPayerInfo.SubOpenId), "sub_openid");
        JsonNameShouldBe<PartnerSceneInfo>(nameof(PartnerSceneInfo.PayerClientIp), "payer_client_ip");
        JsonNameShouldBe<PartnerH5SceneInfo>(nameof(PartnerH5SceneInfo.H5Info), "h5_info");
        JsonNameShouldBe<PartnerH5Info>(nameof(PartnerH5Info.BundleId), "bundle_id");
        JsonNameShouldBe<PartnerH5Info>(nameof(PartnerH5Info.PackageName), "package_name");
        JsonNameShouldBe<PartnerScannedPosStoreInfo>(nameof(PartnerScannedPosStoreInfo.OutId), "out_id");
        JsonNameShouldBe<PartnerGoodsDetailItem>(nameof(PartnerGoodsDetailItem.MerchantGoodsId), "merchant_goods_id");
        JsonNameShouldBe<PartnerGoodsDetailItem>(nameof(PartnerGoodsDetailItem.WechatpayGoodsId), "wechatpay_goods_id");

        // 关单请求体只有两个商户号：官方无 AppID 字段。
        JsonNameShouldBe<PartnerCloseOrderRequest>(nameof(PartnerCloseOrderRequest.SpMchId), "sp_mchid");
        JsonNameShouldBe<PartnerCloseOrderRequest>(nameof(PartnerCloseOrderRequest.SubMchId), "sub_mchid");
        JsonNamesOf<PartnerCloseOrderRequest>().Should().BeEquivalentTo(new[] { "sp_mchid", "sub_mchid" },
            "官方关单请求体就此两字段，抄下单族把 sp_appid 带进来即制造非法字段");

        // 查单应答：金额四项与优惠明细的官方拼写（payer_total / wechatpay_contribute 最常被「纠正」）。
        JsonNameShouldBe<PartnerTransactionQueryResponse>(nameof(PartnerTransactionQueryResponse.TradeStateDesc), "trade_state_desc");
        JsonNameShouldBe<PartnerTransactionQueryResponse>(nameof(PartnerTransactionQueryResponse.BankType), "bank_type");
        JsonNameShouldBe<PartnerTransactionQueryResponse>(nameof(PartnerTransactionQueryResponse.SuccessTime), "success_time");
        JsonNameShouldBe<PartnerQueryAmountInfo>(nameof(PartnerQueryAmountInfo.PayerTotal), "payer_total");
        JsonNameShouldBe<PartnerQueryAmountInfo>(nameof(PartnerQueryAmountInfo.PayerCurrency), "payer_currency");
        JsonNameShouldBe<PartnerPromotionDetail>(nameof(PartnerPromotionDetail.WechatpayContribute), "wechatpay_contribute");
        JsonNameShouldBe<PartnerPromotionDetail>(nameof(PartnerPromotionDetail.MerchantContribute), "merchant_contribute");
        JsonNameShouldBe<PartnerPromotionDetail>(nameof(PartnerPromotionDetail.OtherContribute), "other_contribute");
        JsonNameShouldBe<PartnerPromotionDetail>(nameof(PartnerPromotionDetail.StockId), "stock_id");
        JsonNameShouldBe<PartnerPromotionGoodsDetail>(nameof(PartnerPromotionGoodsDetail.DiscountAmount), "discount_amount");
        JsonNameShouldBe<PartnerPromotionGoodsDetail>(nameof(PartnerPromotionGoodsDetail.GoodsRemark), "goods_remark");

        // 分期曝光：user_id 是**对象**、四值类型字段名与应答两节。
        JsonNameShouldBe<PartnerInstalExposureRequest>(nameof(PartnerInstalExposureRequest.UserId), "user_id");
        JsonNameShouldBe<PartnerInstalUserId>(nameof(PartnerInstalUserId.UserIdType), "user_id_type");
        JsonNameShouldBe<PartnerInstalExposureResponse>(nameof(PartnerInstalExposureResponse.RecommendExposure), "recommend_exposure");
        JsonNameShouldBe<PartnerInstalExposureResponse>(nameof(PartnerInstalExposureResponse.RepaymentInfo), "repayment_info");
        JsonNameShouldBe<PartnerRepaymentInfo>(nameof(PartnerRepaymentInfo.RepaymentType), "repayment_type");
        JsonNameShouldBe<PartnerRepaymentInfo>(nameof(PartnerRepaymentInfo.ExistCashOffCoupon), "exist_cash_off_coupon");
    }

    /// <summary>
    /// PT-B4：五族形态差异用类型精确表达 —— <c>payer</c> 只两族有、<c>support_fapiao</c> 位置按族分列、
    /// 三套 <c>scene_info</c> 与两套 <c>store_info</c> 不共用。
    /// </summary>
    [Fact]
    public void PartnerOrderFamilies_ShouldKeepTheirOfficialShapeDifferencesInTypes()
    {
        // ① payer 只存在于 JSAPI 与扫码支付两族（Native / APP / H5 官方字段表内无该节）。
        JsonNamesOf<PartnerJsapiPrepayRequest>().Should().Contain("payer", "JSAPI 下单官方必填 payer");
        JsonNamesOf<PartnerScannedPosPrepayRequest>().Should().Contain("payer");
        foreach (var type in new[]
                 {
                     typeof(PartnerNativePrepayRequest), typeof(PartnerAppPrepayRequest), typeof(PartnerH5PrepayRequest),
                 })
        {
            JsonNamesOf(type).Should().NotContain("payer",
                $"{type.Name} 官方无 payer：扫码/APP/H5 下单时不存在支付者标识");
        }

        // ② support_fapiao 位置：唯扫码支付在顶层，其余四族在 settle_info 内。
        JsonNamesOf<PartnerScannedPosPrepayRequest>().Should().Contain("support_fapiao",
            "扫码支付族把该字段放在请求顶层（官方原文），不得搬进 settle_info");
        foreach (var type in new[]
                 {
                     typeof(PartnerJsapiPrepayRequest), typeof(PartnerNativePrepayRequest),
                     typeof(PartnerAppPrepayRequest), typeof(PartnerH5PrepayRequest),
                 })
        {
            JsonNamesOf(type).Should().NotContain("support_fapiao",
                $"{type.Name} 的 support_fapiao 属 settle_info 内字段，顶层出现即报文错位");
        }
        JsonNamesOf<PartnerSettleInfo>().Should().Contain("support_fapiao");
        JsonNamesOf<PartnerScannedPosSettleInfo>().Should().BeEquivalentTo(new[] { "profit_sharing" },
            "扫码支付族的 settle_info 官方只有 profit_sharing（无 subsidy_amount / support_fapiao）");

        // ③ 三套 scene_info 各自成类型，且属性类型按族精确。
        typeof(PartnerJsapiPrepayRequest).GetProperty(nameof(PartnerJsapiPrepayRequest.SceneInfo))!
            .PropertyType.Should().Be(typeof(PartnerSceneInfo));
        typeof(PartnerH5PrepayRequest).GetProperty(nameof(PartnerH5PrepayRequest.SceneInfo))!
            .PropertyType.Should().Be(typeof(PartnerH5SceneInfo),
                "H5 场景信息多一节必填 h5_info，复用 PartnerSceneInfo 会传不了该字段");
        typeof(PartnerScannedPosPrepayRequest).GetProperty(nameof(PartnerScannedPosPrepayRequest.SceneInfo))!
            .PropertyType.Should().Be(typeof(PartnerScannedPosSceneInfo),
                "扫码支付场景信息的 store_info 必填且字段集不同");
        JsonNamesOf<PartnerH5SceneInfo>().Should().Contain("h5_info");
        JsonNamesOf<PartnerSceneInfo>().Should().NotContain("h5_info", "JSAPI/Native/APP 三族无 h5_info");
        JsonNamesOf<PartnerScannedPosSceneInfo>().Should().NotContain("h5_info");

        // ④ 两套 store_info：通用四字段 vs 扫码支付只有 id / out_id。
        JsonNamesOf<PartnerStoreInfo>().Should().BeEquivalentTo(new[] { "id", "name", "area_code", "address" });
        JsonNamesOf<PartnerScannedPosStoreInfo>().Should().BeEquivalentTo(new[] { "id", "out_id" },
            "官方本族门店节就此两键（二选一），不得靠共用类型补出 name/area_code/address");
        typeof(PartnerScannedPosSceneInfo).GetProperty(nameof(PartnerScannedPosSceneInfo.StoreInfo))!
            .PropertyType.Should().Be(typeof(PartnerScannedPosStoreInfo));

        // ⑤ 两个下单应答形态不可互换：prepay_id 与 code_url / h5_url 分类型。
        //    （只看本类声明的字段：code / message 来自 WechatPayResponse 基底，见 PT-B6。）
        DeclaredJsonNamesOf<PartnerPrepayIdResponse>().Should().BeEquivalentTo(new[] { "prepay_id" });
        DeclaredJsonNamesOf<PartnerNativePrepayResponse>().Should().BeEquivalentTo(new[] { "code_url" });
        DeclaredJsonNamesOf<PartnerH5PrepayResponse>().Should().BeEquivalentTo(new[] { "h5_url" });
    }

    /// <summary>PT-B5：官方枚举取值与数量逐项锁定（防漏抄 / 防自造值 / 防「纠正拼写」）。</summary>
    [Fact]
    public void PartnerTransactionEnumConstants_ShouldMatchOfficialValues()
    {
        ValuesOf(typeof(PartnerTradeTypes)).Should().BeEquivalentTo(new[]
        {
            "JSAPI", "NATIVE", "APP", "MICROPAY", "MWEB", "FACEPAY",
        }, "官方 trade_type 为 6 值：MICROPAY / FACEPAY 只在查单应答出现，本域无对应下单端点");

        ValuesOf(typeof(PartnerTradeStates)).Should().BeEquivalentTo(new[]
        {
            "SUCCESS", "REFUND", "NOTPAY", "CLOSED", "REVOKED", "USERPAYING", "PAYERROR",
        }, "官方 trade_state 为 7 值，只有 SUCCESS 是支付成功");

        ValuesOf(typeof(PartnerPromotionScopes)).Should().BeEquivalentTo(new[] { "GLOBAL", "SINGLE" });
        ValuesOf(typeof(PartnerPromotionTypes)).Should().BeEquivalentTo(new[] { "CASH", "NOCASH" });

        ValuesOf(typeof(PartnerUserIdTypes)).Should().BeEquivalentTo(new[]
        {
            "UNKNOWN_USER_ID_TYPE", "OPENID", "SP_OPENID", "SUB_OPENID",
        }, "官方 user_id_type 为 4 值（含 UNKNOWN_USER_ID_TYPE），不得自造 APP_OPENID 之类");

        ValuesOf(typeof(PartnerRepaymentTypes)).Should().BeEquivalentTo(new[]
        {
            "REPAYMENT_DAILY", "REPAYMENT_INSTALL_3", "REPAYMENT_INSTALL_6",
            "REPAYMENT_INSTALL_12", "REPAYMENT_INSTALL_24",
        }, "官方档期数字直接拼进枚举名，日付档是 REPAYMENT_DAILY 而非 REPAYMENT_INSTALL_1");

        // 本域各页错误码表只有 APIv3 公共码（PARAM_ERROR / SIGN_ERROR / …）⇒ 刻意不建专有码常量类，
        // 以特约商户进件域作正对照（那边确有 APPLYMENT_NOT_EXIST 专有码）。
        typeof(PartnerJsapiPrepayRequest).Assembly.GetTypes()
            .Where(static t => t.Namespace == DomainNamespace)
            .Select(static t => t.Name)
            .Should().NotContain(static n => n.EndsWith("ErrorCodes", StringComparison.Ordinal),
                "本域官方错误码全是公共码：建专有码常量类即制造「能识别本域专有错误」的假象");
        ValuesOf(typeof(Apply4SubMerchantErrorCodes)).Should().Contain("APPLYMENT_NOT_EXIST",
            "正对照：进件域确有专有码，两域判错面不同");
    }

    /// <summary>PT-B6：判错面与应答形态 —— 有包体的应答继承 <see cref="WechatPayResponse"/>，关单为 204 纯 <c>Task</c>。</summary>
    [Fact]
    public void PartnerTransactionsResponses_ShouldDeriveFromWechatPayResponseAndMatchOfficialBodies()
    {
        foreach (var responseType in new[]
                 {
                     typeof(PartnerPrepayIdResponse), typeof(PartnerNativePrepayResponse),
                     typeof(PartnerH5PrepayResponse), typeof(PartnerTransactionQueryResponse),
                     typeof(PartnerInstalExposureResponse),
                 })
        {
            typeof(WechatPayResponse).IsAssignableFrom(responseType).Should().BeTrue(
                $"{responseType.Name} 必须继承 WechatPayResponse，否则 4xx 的 code/message 无落点");
        }

        // 官方关单成功为 204 No Content 且无包体 ⇒ 方法必须无返回值。
        FindMethod(nameof(IWechatPayPartnerTransactionsService.CloseOrderAsync)).ReturnType
            .Should().Be(typeof(Task), "官方关单成功应答 204 无包体：声明成响应类型会造出永不填充的假字段");

        // 应答共用判据「表相同则共用」：JSAPI / APP / 扫码支付三族同为 prepay_id ⇒ 同一类型；
        // 双路查单应答表逐项一致 ⇒ 同一类型。
        ReturnTypeOf(nameof(IWechatPayPartnerTransactionsService.CreateJsapiOrderAsync))
            .Should().Be(typeof(PartnerPrepayIdResponse));
        ReturnTypeOf(nameof(IWechatPayPartnerTransactionsService.CreateAppOrderAsync))
            .Should().Be(typeof(PartnerPrepayIdResponse));
        ReturnTypeOf(nameof(IWechatPayPartnerTransactionsService.CreateScannedPosOrderAsync))
            .Should().Be(typeof(PartnerPrepayIdResponse));
        ReturnTypeOf(nameof(IWechatPayPartnerTransactionsService.QueryByTransactionIdAsync))
            .Should().Be(typeof(PartnerTransactionQueryResponse));
        ReturnTypeOf(nameof(IWechatPayPartnerTransactionsService.QueryByOutTradeNoAsync))
            .Should().Be(typeof(PartnerTransactionQueryResponse));
        ReturnTypeOf(nameof(IWechatPayPartnerTransactionsService.CreateNativeOrderAsync))
            .Should().Be(typeof(PartnerNativePrepayResponse));
        ReturnTypeOf(nameof(IWechatPayPartnerTransactionsService.CreateH5OrderAsync))
            .Should().Be(typeof(PartnerH5PrepayResponse));

        // 查单应答必须带服务商四件套（直连类型没有）⇒ 证实两线未「顺手共用」DTO。
        var queryNames = JsonNamesOf(typeof(PartnerTransactionQueryResponse));
        queryNames.Should().Contain(new[] { "sp_appid", "sp_mchid", "sub_appid", "sub_mchid" });
        queryNames.Should().NotContain(new[] { "appid", "mchid" },
            "本域应答没有直连的 appid/mchid：两线 DTO 各自独立，不得合并");
        JsonNamesOf(typeof(TransactionQueryResponse)).Should().NotContain("sp_mchid",
            "正对照：直连域查单应答确实没有服务商四件套");
    }

    /// <summary>PT-B7：本域 30 个 DTO 全量登记进 AOT 上下文且分组名一致。</summary>
    [Fact]
    public void PartnerTransactionsDataModels_ShouldBeRegisteredInJsonContext()
    {
        var domainTypes = typeof(PartnerJsapiPrepayRequest).Assembly.GetTypes()
            .Where(static t => t.IsClass && !t.IsAbstract && !t.IsNested)
            .Where(static t => t.Namespace == DomainNamespace)
            .Where(static t => !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(30,
            "服务商支付域 DTO：共享构件 12 + 五族请求 5 + 三种下单应答 3 + 查单应答族 5 + 关单 1 + 分期 4");

        foreach (var type in domainTypes)
        {
            PartnerTransactionsJsonContext.Default.GetTypeInfo(type)
                .Should().NotBeNull($"{type.Name} 必须登记进 PartnerTransactionsJsonContext（否则 Native AOT 下无元数据）");
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 {RegistryGroupName}");
        }

        // 上下文必须接进本线的合并解析器（否则声明式客户端在 AOT 下取不到 JsonTypeInfo）。
        var resolverSource = File.ReadAllText(SourcePath("Mud.Wechat.Pay",
            "Extensions", "PayJsonResolverExtensions.cs"));
        resolverSource.Should().Contain("PartnerTransactionsJsonContext.Default",
            "新增域上下文须同批登记进 PayJsonResolverExtensions（AOT 红线：无登记即无元数据）");
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
        => typeof(IWechatPayPartnerTransactionsService)
               .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
           ?? throw new InvalidOperationException($"IWechatPayPartnerTransactionsService.{methodName} 必须存在");

    private static Type ReturnTypeOf(string methodName)
    {
        var taskType = FindMethod(methodName).ReturnType;
        taskType.IsGenericType.Should().BeTrue($"{methodName} 应答必须是 Task<T>");
        return taskType.GetGenericArguments()[0];
    }

    private static string[] RoutesOf(Type serviceType)
        => serviceType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .SelectMany(static m => m.GetCustomAttributes<HttpMethodAttribute>())
            .Select(static a => a.RequestUri!)
            .ToArray();

    private static void JsonNameShouldBe<T>(string propertyName, string expectedJsonName)
        => JsonNameShouldBe(typeof(T), propertyName, expectedJsonName);

    private static void JsonNameShouldBe(Type type, string propertyName, string expectedJsonName)
    {
        var property = type.GetProperty(propertyName);
        property.Should().NotBeNull($"{type.Name}.{propertyName} 必须存在（官方契约面漂移）");
        property!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be(expectedJsonName);
    }

    private static string[] JsonNamesOf<T>() => JsonNamesOf(typeof(T));

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

    /// <summary>支付线四个工程的源码文件（不含 bin/obj）——零声明断言的扫描集。</summary>
    private static string[] CollectPaySourceFiles()
        => new[]
           {
               "Mud.Wechat.Pay",
               "Mud.Wechat.Pay.Abstractions",
               "Mud.Wechat.Pay.DataModels",
               "Mud.Wechat.Pay.Callback",
           }
            .Select(SourceProjectDir)
            .SelectMany(dir => Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories))
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(p => !p.EndsWith(".g.cs", StringComparison.Ordinal))
            .ToArray();

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

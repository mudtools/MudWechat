// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Funds;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「资金结算」域契约守卫（设计方案 v1 P1 / CF 系列：Funds 域 16 端点路由 + 双前缀并存 +
/// 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 错误码常量 + 概念辨析措辞）。
/// </summary>
/// <remarks>
/// <para>
/// <b>概念辨析（设计方案 v1 §4.6，守卫措辞锁定）</b>：本域是小店<b>结算 / 提现 / 流水</b>（小店
/// <c>access_token</c>），<b>不是</b>微信支付 APIv3（<c>Mud.Wechat.Pay</c>，商户私钥签名）也<b>不是</b>
/// 企微企业支付 / 收银台（Work 线）。路由面由 CH-R2 锁定（全 <c>/channels/ec/funds/*</c> 与
/// <c>/shop/funds/*</c>，与 Pay 的 <c>/v3/*</c>、Work 的 <c>/cgi-bin/*</c> 天然无交叠）。
/// </para>
/// <para>
/// <b>双前缀并存（设计方案 v1 §4.3）</b>：《shop/funds/*》为官方历史前缀，照抄原文，禁止「对齐规范」改写。
/// </para>
/// </remarks>
public class ChannelsFundsContractGuards
{
    private const string FundsRegistryGroupName = "Funds";

    /// <summary>Funds 域官方路由表（16 端点：/channels/ec/funds/* 9 + /shop/funds/* 7，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] FundsRoutes =
    {
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetBalanceAsync), typeof(PostAttribute), "/channels/ec/funds/getbalance"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetBankAccountAsync), typeof(PostAttribute), "/channels/ec/funds/getbankacct"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.SetBankAccountAsync), typeof(PostAttribute), "/channels/ec/funds/setbankacct"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.SubmitWithdrawAsync), typeof(PostAttribute), "/channels/ec/funds/submitwithdraw"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetWithdrawListAsync), typeof(PostAttribute), "/channels/ec/funds/getwithdrawlist"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetWithdrawDetailAsync), typeof(PostAttribute), "/channels/ec/funds/getwithdrawdetail"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetFundsFlowListAsync), typeof(PostAttribute), "/channels/ec/funds/getfundsflowlist"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetFundsFlowDetailAsync), typeof(PostAttribute), "/channels/ec/funds/getfundsflowdetail"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.ListOrderFlowAsync), typeof(PostAttribute), "/channels/ec/funds/listorderflow"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetCityListAsync), typeof(PostAttribute), "/shop/funds/getcity"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetProvinceListAsync), typeof(PostAttribute), "/shop/funds/getprovince"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetBankListAsync), typeof(PostAttribute), "/shop/funds/getbanklist"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetSubBranchListAsync), typeof(PostAttribute), "/shop/funds/getsubbranch"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetBankByCardNumberAsync), typeof(PostAttribute), "/shop/funds/getbankbynum"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.GetFundsQrcodeAsync), typeof(PostAttribute), "/shop/funds/qrcode/get"),
        (typeof(IChannelsFundsService), nameof(IChannelsFundsService.CheckFundsQrcodeAsync), typeof(PostAttribute), "/shop/funds/qrcode/check"),
    };

    /// <summary>契约守卫 CF1：Funds 域 16 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void FundsEndpoints_ShouldMatchOfficialRoutes()
    {
        // 16 端点 = /channels/ec/funds/* 9 + /shop/funds/* 7（设计方案 v1 §2.2 计数口径）。
        FundsRoutes.Should().HaveCount(16, "资金账户 3 + 提现 3 + 资金流水 3 + 订单流水 1 + 银行地区 5 + 二维码 2 − 1（listorderflow 与 getfundsflowlist 并组)= 16 端点");
        FundsRoutes.Select(r => r.Route).Distinct().Should().HaveCount(16);

        FundsRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/funds/", StringComparison.Ordinal))
            .Should().Be(9, "9 端点走 /channels/ec/funds/* 主干前缀");
        FundsRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/shop/funds/", StringComparison.Ordinal))
            .Should().Be(7, "7 端点走 /shop/funds/* 官方历史前缀（设计方案 v1 §4.3，禁止改写）");

        foreach (var (iface, method, httpAttribute, route) in FundsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CF2：Funds 域 16 端点全部为 POST；空请求体端点不得携带 [Body] 参数。</summary>
    [Fact]
    public void FundsEndpoints_ShouldAllBePost_WithEmptyBodyRules()
    {
        foreach (var (iface, method, _, _) in FundsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（资金结算无 GET 端点）");
        }

        // 官方「传空的json串即可」的三端点（getbalance / getbankacct / getprovince）不得人为造请求体 DTO。
        foreach (var emptyEndpoints in new[]
                 {
                     nameof(IChannelsFundsService.GetBalanceAsync),
                     nameof(IChannelsFundsService.GetBankAccountAsync),
                     nameof(IChannelsFundsService.GetProvinceListAsync),
                 })
        {
            var target = typeof(IChannelsFundsService).GetMethod(emptyEndpoints)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{emptyEndpoints} 官方规定空请求体（调用接口时传空的json串即可）——不得声明 [Body]");
        }
    }

    /// <summary>契约守卫 CF3：令牌绑定与注册形态。</summary>
    [Fact]
    public void FundsInterface_ShouldBindChannelsToken_AndRegisterUnderFundsGroup()
    {
        var token = typeof(IChannelsFundsService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Funds 域 16 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsFundsService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(FundsRegistryGroupName, "必须挂 Funds 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Funds");
        typeof(ChannelsServiceBuilder).GetMethod("AddFundsApi").Should().NotBeNull("必须提供 AddFundsApi 链式注册方法");
    }

    /// <summary>契约守卫 CF4：Funds 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void FundsDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = FundsJsonContext.Default;

        var domainTypes = typeof(ChannelsGetBalanceResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Funds"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t)
                        && !t.IsAbstract)
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(42,
            "Funds 域 16 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（余额 1 + 结算账户 3 + 提现 6 + 资金流水 5 + 订单流水 9 + 城市 3 + 省份 2 + 银行列表 3 + 支行 3 + 卡号 2 + 二维码 3 + 共用 ticket 1 + 共用银行信息 1 = 42）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Funds 域命名空间，必须登记进 FundsJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(FundsRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Funds");
        }
    }

    /// <summary>契约守卫 CF5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void FundsDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 获取账户余额。
        AssertJsonProperty<ChannelsGetBalanceResponse>("available_amount", "可提现余额");
        AssertJsonProperty<ChannelsGetBalanceResponse>("pending_amount", "待结算余额");
        AssertJsonProperty<ChannelsGetBalanceResponse>("sub_mchid", "二级商户号");

        // 结算账户（获取/修改共用嵌套对象字段）。
        AssertJsonProperty<ChannelsGetBankAcctResponse>("account_info", "结算账户信息");
        AssertJsonProperty<ChannelsSettleAccountInfo>("bank_account_type", "账户类型");
        AssertJsonProperty<ChannelsSettleAccountInfo>("account_bank", "开户银行");
        AssertJsonProperty<ChannelsSettleAccountInfo>("account_number", "银行账号");
        AssertJsonProperty<ChannelsSettleAccountModify>("account_bank4show", "开户银行名称前端展示值（仅修改形态）");

        // 提现。
        AssertJsonProperty<ChannelsSubmitWithdrawRequest>("amount", "提现金额（分）");
        AssertJsonProperty<ChannelsSubmitWithdrawRequest>("bank_memo", "银行附言");
        AssertJsonProperty<ChannelsSubmitWithdrawResponse>("withdraw_id", "提现单号（官方说明互换，照录）");
        AssertJsonProperty<ChannelsSubmitWithdrawResponse>("qrcode_ticket", "二维码 ticket");
        AssertJsonProperty<ChannelsGetWithdrawListRequest>("page_num", "页码");
        AssertJsonProperty<ChannelsGetWithdrawListResponse>("withdraw_ids", "提现单号列表");
        AssertJsonProperty<ChannelsGetWithdrawListResponse>("total_num", "提现单号总数");
        AssertJsonProperty<ChannelsGetWithdrawDetailRequest>("withdraw_id", "提现单号");
        AssertJsonProperty<ChannelsGetWithdrawDetailResponse>("status", "提现状态");

        // 资金流水。
        AssertJsonProperty<ChannelsGetFundsFlowListRequest>("next_key", "分页参数");
        AssertJsonProperty<ChannelsGetFundsFlowListRequest>("transaction_id", "支付单号");
        AssertJsonProperty<ChannelsGetFundsFlowListResponse>("flow_ids", "流水单号列表");
        AssertJsonProperty<ChannelsGetFundsFlowListResponse>("has_more", "是否还有下一页");
        AssertJsonProperty<ChannelsGetFundsFlowDetailRequest>("flow_id", "流水 id");
        AssertJsonProperty<ChannelsGetFundsFlowDetailResponse>("funds_flow", "流水信息");
        AssertJsonProperty<ChannelsFundsFlow>("funds_type", "资金类型");
        AssertJsonProperty<ChannelsFundsFlow>("flow_type", "流水类型");
        AssertJsonProperty<ChannelsFundsFlow>("related_info_list", "流水关联信息");
        AssertJsonProperty<ChannelsFundsFlowRelatedInfo>("related_type", "关联类型");
        AssertJsonProperty<ChannelsFundsFlowRelatedInfo>("group_present_sub_order_id_list", "群送礼关联订单号列表");

        // 订单流水（listorderflow）。
        AssertJsonProperty<ChannelsListOrderFlowRequest>("order_settle_state", "订单结算状态");
        AssertJsonProperty<ChannelsListOrderFlowRequest>("pagination_info", "分页信息");
        AssertJsonProperty<ChannelsListOrderFlowRequest>("create_time_range", "订单创建时间范围（参数表字段名）");
        AssertJsonProperty<ChannelsOrderFlowPagination>("use_page_ctx", "是否使用分页上下文");
        AssertJsonProperty<ChannelsOrderFlowPagination>("page_ctx", "分页上下文");
        AssertJsonProperty<ChannelsListOrderFlowResponse>("data_list", "订单流水列表");
        AssertJsonProperty<ChannelsListOrderFlowResponse>("page_ctx", "分页上下文");
        AssertJsonProperty<ChannelsOrderFlowItem>("mch_settle_amount", "（预计）结算金额");
        AssertJsonProperty<ChannelsOrderFlowItem>("post_settlement_expense", "结算后支出");
        AssertJsonProperty<ChannelsOrderFlowItem>("supplier_platform_commission", "供货商平台结算信息");
        AssertJsonProperty<ChannelsOrderFlowProduct>("sale_price", "销售价格");
        AssertJsonProperty<ChannelsOrderFlowProductParam>("key", "规格名称");

        // 银行·省市·支行。
        AssertJsonProperty<ChannelsGetCityRequest>("province_code", "省份编码");
        AssertJsonProperty<ChannelsGetCityResponse>("data", "城市信息列表");
        AssertJsonProperty<ChannelsCity>("bank_address_code", "开户银行省市编码");
        AssertJsonProperty<ChannelsProvince>("province_name", "省份简称");
        AssertJsonProperty<ChannelsGetBankListRequest>("key_words", "银行关键字");
        AssertJsonProperty<ChannelsGetBankListRequest>("bank_type", "银行类型");
        AssertJsonProperty<ChannelsBankInfo>("bank_code", "银行编码");
        AssertJsonProperty<ChannelsBankInfo>("need_branch", "是否需要填写支行信息");
        AssertJsonProperty<ChannelsGetSubBranchRequest>("city_code", "城市编号");
        AssertJsonProperty<ChannelsGetSubBranchResponse>("bank_alias_code", "银行别名编码");
        AssertJsonProperty<ChannelsSubBranch>("bank_name", "银行名称（参数表字段名）");
        AssertJsonProperty<ChannelsGetBankByNumRequest>("account_number", "银行卡号");

        // 资金二维码。
        AssertJsonProperty<ChannelsQrcodeTicketRequest>("qrcode_ticket", "二维码 ticket");
        AssertJsonProperty<ChannelsGetFundsQrcodeResponse>("qrcode_buf", "二维码 base64 二进制");
        AssertJsonProperty<ChannelsCheckFundsQrcodeResponse>("status", "扫码状态");
        AssertJsonProperty<ChannelsCheckFundsQrcodeResponse>("self_check_err_code", "业务返回错误码");
        AssertJsonProperty<ChannelsCheckFundsQrcodeResponse>("scan_user_type", "扫码者身份");

        // 共用 DTO 裁决：qrcode/get 与 qrcode/check 请求体字段集一致 ⇒ 共用 ChchannelsQrcodeTicketRequest。
        typeof(IChannelsFundsService).GetMethod(nameof(IChannelsFundsService.GetFundsQrcodeAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsQrcodeTicketRequest));
        typeof(IChannelsFundsService).GetMethod(nameof(IChannelsFundsService.CheckFundsQrcodeAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsQrcodeTicketRequest));

        // 共用 DTO 裁决：getbanklist 与 getbankbynum 的元素形态一致 ⇒ 共用 ChannelsBankInfo。
        AssertJsonProperty<ChannelsBankInfo>("account_bank", "开户银行");
        typeof(ChannelsGetBankListResponse).GetProperty(nameof(ChannelsGetBankListResponse.Data))!
            .PropertyType.Should().Be(typeof(List<ChannelsBankInfo>));
        typeof(ChannelsGetBankByNumResponse).GetProperty(nameof(ChannelsGetBankByNumResponse.Data))!
            .PropertyType.Should().Be(typeof(List<ChannelsBankInfo>));
    }

    /// <summary>契约守卫 CF6：Funds 域已核验错误码常量锁定。</summary>
    [Fact]
    public void FundsErrorCodes_ShouldMatchVerifiedOfficialValues()
    {
        ChannelsErrorCodes.QrcodeTokenTooLong.Should().Be(-2);
        ChannelsErrorCodes.WindowForbidden.Should().Be(10022002);
        ChannelsErrorCodes.FundsFlowNotFound.Should().Be(10021302);
        ChannelsErrorCodes.QrcodeTicketInvalid.Should().Be(60208);
        ChannelsErrorCodes.QrcodeTicketExpired.Should().Be(60220);
        ChannelsErrorCodes.OrderFlowParamError.Should().Be(669900000);
        ChannelsErrorCodes.OrderFlowSystemError.Should().Be(669900001);
        ChannelsErrorCodes.BankDataNotFound.Should().Be(9710001);
    }

    /// <summary>契约守卫 CF7：枚举常量字面量锁定（覆盖「措辞防串线」——资金/支付/企微支付概念辨析）。</summary>
    [Fact]
    public void FundsEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsBankAccountTypes.Business.Should().Be("ACCOUNT_TYPE_BUSINESS");
        ChannelsBankAccountTypes.Private.Should().Be("ACCOUNT_TYPE_PRIVATE");

        ChannelsWithdrawStatuses.Init.Should().Be("INIT");
        ChannelsWithdrawStatuses.CreateSuccess.Should().Be("CREATE_SUCCESS");
        ChannelsWithdrawStatuses.Success.Should().Be("SUCCESS");
        ChannelsWithdrawStatuses.Fail.Should().Be("FAIL");
        ChannelsWithdrawStatuses.Refund.Should().Be("REFUND");
        ChannelsWithdrawStatuses.Close.Should().Be("CLOSE");

        ChannelsFundsTypes.OrderPay.Should().Be(1);
        ChannelsFundsTypes.OrderRefund.Should().Be(3);
        ChannelsFundsTypes.Withdraw.Should().Be(4);
        ChannelsFundsTypes.PromoterCommission.Should().Be(10);
        ChannelsFundsTypes.PlatformCommission.Should().Be(11);
        ChannelsFundsTypes.NationalSubsidy.Should().Be(21);

        ChannelsOrderSettleStates.Pending.Should().Be(1);
        ChannelsOrderSettleStates.Settled.Should().Be(60);
        ChannelsOrderSettleStates.Partial.Should().Be(100);
        ChannelsOrderFlowOrderStates.PendingDelivery.Should().Be(20);
        ChannelsOrderFlowPayMethods.Normal.Should().Be(1);
        ChannelsOrderFlowPayMethods.PayLater.Should().Be(2);

        ChannelsFundsQrcodeStatuses.NotScanned.Should().Be(0);
        ChannelsFundsQrcodeStatuses.Confirmed.Should().Be(1);
        ChannelsFundsQrcodeStatuses.Expired.Should().Be(3);
        ChannelsFundsScanUserTypes.Admin.Should().Be(1);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
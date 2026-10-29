// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.ShopFunds;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Match = System.Text.RegularExpressions.Match;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「自定义交易组件 · 资金（/shop/funds/*）」域契约守卫（SHOPF 系列：9 端点路由与动词 +
/// 请求形态（空体 / 单一体）+ 令牌绑定与注册组 + DTO 登记计数与 JSON 上下文 + 官方字段名与类型口径 +
/// 资金敏感面红线 + 与小店线同前缀零交叉 + 扫码凭据单出口与两线分持面的零声明）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 2026-10-18 逐页核验，9 页全部 200、正文含「请求方式 + 请求地址」、均在现行目录树内）</b>：
/// 余额与结算账户 3（<c>getbalance</c> / <c>getbankaccount</c> / <c>setbankaccount</c>，<see cref="IWxaShopFundsAccountService"/>）
/// + 提现与提款二维码 4（<c>submitwithdraw</c> / <c>getwithdrawdetail</c> / <c>scanwithdraw</c> / <c>qrcode/gen</c>，
/// <see cref="IWxaShopFundsWithdrawService"/>）+ 资金流水 2（<c>scanorderflow</c> / <c>getorderflow</c>，
/// <see cref="IWxaShopFundsFlowService"/>）；全部 POST，全部 <c>https://api.weixin.qq.com/shop/funds/…?access_token=</c>。
/// 两棵官方子树（<c>minishopopencomponent2/API/funds/*</c> 6 页与 <c>ministore/wxafunds/API/funds/*</c> 3 页）
/// 的<b>请求地址同一前缀</b> ⇒ 文档分叉不改路由，本域按页面正文的请求地址逐条落地（无一条判非契约）。
/// </para>
/// <para>
/// <b>本域最需要防误读的一处：<c>/shop/funds/*</c> 前缀由两线分持</b>：官方同前缀下另有 7 页（银行与地区 5 +
/// 取二维码图片 / 查扫码状态 2）本仓已由<b>小店线</b> <c>IChannelsFundsService</c> 以
/// <c>ChannelsTokenTypes.AccessToken</c> 声明 ⇒ 认领判据是「页面令牌与文档归属」而非前缀。
/// SHOPF7 用两线路由集不相交把这条边界钉住，SHOPF8 再钉住「本线不得把那 7 条补进来」。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）、端点聚合计数
/// （MP-X5 / <c>ContractBaseline</c>）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；
/// 本文件只锁本域精确契约（<c>P3-R1</c> 只管 <c>/wxaapi/minishop/*</c> 前缀，与本域 <c>/shop/funds/*</c> 无关）。
/// </para>
/// </remarks>
public class WxaShopFundsContractGuards
{
    private const string RegistryGroupName = "ShopFunds";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.ShopFunds";

    /// <summary>资金域官方路由表（9 端点，全部 POST；路由照官方页面「请求地址」原文，禁止改写）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaShopFundsAccountService), nameof(IWxaShopFundsAccountService.GetBalanceAsync), typeof(PostAttribute), "/shop/funds/getbalance"),
        (typeof(IWxaShopFundsAccountService), nameof(IWxaShopFundsAccountService.GetBankAccountAsync), typeof(PostAttribute), "/shop/funds/getbankaccount"),
        (typeof(IWxaShopFundsAccountService), nameof(IWxaShopFundsAccountService.SetBankAccountAsync), typeof(PostAttribute), "/shop/funds/setbankaccount"),

        (typeof(IWxaShopFundsWithdrawService), nameof(IWxaShopFundsWithdrawService.SubmitWithdrawAsync), typeof(PostAttribute), "/shop/funds/submitwithdraw"),
        (typeof(IWxaShopFundsWithdrawService), nameof(IWxaShopFundsWithdrawService.GetWithdrawDetailAsync), typeof(PostAttribute), "/shop/funds/getwithdrawdetail"),
        (typeof(IWxaShopFundsWithdrawService), nameof(IWxaShopFundsWithdrawService.GetWithdrawListAsync), typeof(PostAttribute), "/shop/funds/scanwithdraw"),
        (typeof(IWxaShopFundsWithdrawService), nameof(IWxaShopFundsWithdrawService.GenerateQrcodeAsync), typeof(PostAttribute), "/shop/funds/qrcode/gen"),

        (typeof(IWxaShopFundsFlowService), nameof(IWxaShopFundsFlowService.GetFundsFlowListAsync), typeof(PostAttribute), "/shop/funds/scanorderflow"),
        (typeof(IWxaShopFundsFlowService), nameof(IWxaShopFundsFlowService.GetFundsFlowDetailAsync), typeof(PostAttribute), "/shop/funds/getorderflow"),
    };

    /// <summary>官方请求体为空对象（示例 <c>{}</c>）的两支：不得人为造请求 DTO。</summary>
    private static readonly string[] EmptyBodyMethods =
    {
        nameof(IWxaShopFundsAccountService.GetBalanceAsync),
        nameof(IWxaShopFundsAccountService.GetBankAccountAsync),
    };

    /// <summary>
    /// 同前缀但<b>不属本线</b>的 7 条官方路由（小店线 <c>IChannelsFundsService</c> 已声明）：
    /// 银行与地区 5 + 提款二维码取图 / 查扫码状态 2 ⇒ 小程序线零声明（SHOPF8）。
    /// </summary>
    private static readonly string[] ChannelsOwnedShopFundsRoutes =
    {
        "/shop/funds/getbanklist",
        "/shop/funds/getbankbynum",
        "/shop/funds/getprovince",
        "/shop/funds/getcity",
        "/shop/funds/getsubbranch",
        "/shop/funds/qrcode/get",
        "/shop/funds/qrcode/check",
    };

    /// <summary>本域官方未公布「有效 / 过期时长」数字 ⇒ 不得凭空造有效期字段（SHOPF8 的负锁清单）。</summary>
    private static readonly string[] ForbiddenValidityPropertyTokens = { "expire", "expiry", "validity", "ttl", "timeout" };

    /// <summary>契约守卫 SHOPF1：9 端点路由与动词必须与官方契约一致（全部 POST、前缀恒 /shop/funds/）。</summary>
    [Fact]
    public void ShopFundsEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(9,
            "资金域官方开放面为 9 端点（2026-10-18 逐页核验：余额与结算账户 3 + 提现与二维码 4 + 资金流水 2；"
            + "同前缀另 7 页归小店线，不计入本域）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(9, "路由表不得出现重复条目");

        Routes.Select(static r => r.Route)
            .Count(static r => r.StartsWith("/shop/funds/", StringComparison.Ordinal))
            .Should().Be(9, "9 条路由全部落在官方 /shop/funds/ 前缀（两棵官方子树的请求地址同前缀）");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方页面「请求地址」逐字一致（禁止改写）");
        }

        // 动词分布：官方 9 页正文一律「http请求方式：POST」（无一支 GET），
        // 且本域两支列表端点（scanorderflow / scanwithdraw）也是 POST + JSON 请求体 ⇒ 全 POST。
        Routes.Should().OnlyContain(static r => r.HttpAttribute == typeof(PostAttribute),
            "官方 9 页的请求方式全部为 POST：列表支不得按「查询即 GET」的直觉改写动词或改走 [Query]");
    }

    /// <summary>
    /// 契约守卫 SHOPF2：请求形态 —— <c>getbalance</c> / <c>getbankaccount</c> 官方请求示例即空对象
    /// ⇒ 零 <c>[Body]</c>；其余 7 支恰有一个 <c>[Body]</c> 请求体参数。
    /// </summary>
    [Fact]
    public void ShopFundsEndpoints_ShouldRespectEmptyBodyAndSingleBodyShapes()
    {
        foreach (var method in EmptyBodyMethods)
        {
            var (iface, _, _, route) = Routes.Single(r => r.Method == method);
            var target = iface.GetMethod(method)!;
            target.GetParameters()
                .SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{route} 官方请求参数示例为空对象（无请求侧参数表）——不得人为造请求 DTO");
            target.GetParameters().Should().OnlyContain(static p => p.ParameterType == typeof(CancellationToken),
                $"{route} 只允许 CancellationToken 参数（令牌由接口级 [Token] 注入 Query）");
        }

        EmptyBodyMethods.Should().HaveCount(2, "官方请求体为空的端点为 2（余额 + 查询结算账户）");

        foreach (var (iface, method, _, route) in Routes)
        {
            if (EmptyBodyMethods.Contains(method))
            {
                continue;
            }

            var target = iface.GetMethod(method)!;
            target.GetParameters()
                .Count(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                .Should().Be(1, $"{route} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
        }
    }

    /// <summary>契约守卫 SHOPF3：三接口令牌绑定与注册组（复用 <c>MpTokenTypes.AccessToken</c>，MP-X2 / X3 同口径）。</summary>
    [Fact]
    public void ShopFundsInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup()
    {
        var interfaces = new[]
        {
            typeof(IWxaShopFundsAccountService),
            typeof(IWxaShopFundsWithdrawService),
            typeof(IWxaShopFundsFlowService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方 9 页请求地址均为 ?access_token=）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（改绑小店令牌或新增类型会让单槽 Resolve 返回 null，errcode 自愈静默失效）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3）");
            token.Name.Should().Be("access_token");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 ShopFunds 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");
        }

        // 三接口同组 ⇒ DI 侧由同一条生成入口装载（AddShopFundsWebApiHttpClient，注册委托待主控合并
        // MiniProgramModule.ShopFunds + AddShopFundsApi() 后由 MiniProgramContractGuards 的装配用例兜住）。
        interfaces.Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .Distinct().Should().HaveCount(1, "本域三接口共用一个注册组（AddShopFundsWebApiHttpClient）");
    }

    /// <summary>契约守卫 SHOPF4：DTO 全量登记进 AOT JSON 上下文；命名空间清单锁死（常量类恰 3）。</summary>
    [Fact]
    public void ShopFundsDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaShopFundsBalanceResponse).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("资金域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 19 DTO = 余额与结算账户 5（余额响应 + 余额对象 + 账户响应 + 账户对象 + 修改请求）
        //          + 提现与二维码 9（发起请求/响应 + 详情请求/响应 + 详情对象 + 列表请求/响应 + 二维码请求/响应）
        //          + 资金流水 5（列表请求/响应 + 详情请求/响应 + 流水对象）。
        dtoTypes.Should().HaveCount(19,
            "资金域 9 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方参数表再同批调整本守卫"
            + "（account 5 + withdraw 9 + flow 5 = 19）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 ShopFunds");
        }

        // 生成上下文按反射定位（上下文尚未生成时守卫给出可执行修复指引，而不是让测试工程编译失败）。
        var contextType = assembly.GetType(DtoNamespace + ".ShopFundsJsonContext");
        contextType.Should().NotBeNull(
            "ShopFundsJsonContext 尚未生成：须由主控在收口批统一跑 scripts/AddHttpJsonSerializable.ps1 + "
            + "scripts/GenerateJsonContext.ps1（Generated/ 为生成物，勿手改；并行作业期间本项允许红）");

        var defaultProperty = contextType?.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        if (defaultProperty != null)
        {
            var jsonContext = (JsonSerializerContext)defaultProperty.GetValue(null)!;
            foreach (var type in dtoTypes)
            {
                jsonContext.GetTypeInfo(type).Should().NotBeNull(
                    $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"ShopFunds\")]，"
                    + "必须登记进 ShopFundsJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
            }
        }

        // 命名空间清单锁死：除已登记 DTO、生成上下文与三个官方枚举常量类外不得出现任何新类型。
        var constClasses = new[]
        {
            nameof(WxaShopFundsBankAccountTypes),
            nameof(WxaShopFundsWithdrawStatuses),
            nameof(WxaShopFundsIdentityTypes),
        };

        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && t != contextType && !constClasses.Contains(t.Name))
            .ToList();

        unregistered.Should().BeEmpty(
            "资金域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文、亦非已登记常量类的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 常量类或新 DTO 须同批更新本守卫");

        constClasses.Select(n => assembly.GetType(DtoNamespace + "." + n))
            .Should().OnlyContain(t => t != null,
            "三个官方枚举常量类必须存在（账户类型 / 提现状态 / 二维码验证身份）");
    }

    /// <summary>契约守卫 SHOPF5：官方字段名抽样 + 金额类型口径 + 共用 DTO 裁决 + 官方反直觉点锁定。</summary>
    [Fact]
    public void ShopFundsDataModels_ShouldLockOfficialFieldsAndTypeShapes()
    {
        // 余额面（官方本页无金额单位、字段为 number ⇒ long；balance_info 单对象）。
        AssertJsonProperty<WxaShopFundsBalanceResponse>(nameof(WxaShopFundsBalanceResponse.BalanceInfo), "balance_info", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsBalanceInfo>(nameof(WxaShopFundsBalanceInfo.AvailableAmount), "available_amount", "官方原文拼写（可提现余额）");
        AssertJsonProperty<WxaShopFundsBalanceInfo>(nameof(WxaShopFundsBalanceInfo.PendingAmount), "pending_amount", "官方原文拼写（待结算余额）");

        // 结算账户面（查询与修改共用 AccountInfo；查询页官方未展开子字段表 ⇒ 字段集取修改页参数表）。
        AssertJsonProperty<WxaShopFundsBankAccountResponse>(nameof(WxaShopFundsBankAccountResponse.AccountInfo), "account_info", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsBankAccountResponse>(nameof(WxaShopFundsBankAccountResponse.VerifyFailReason), "verify_fail_reason", "官方原文拼写（绑卡失败原因，与 errcode=0 并存）");
        AssertJsonProperty<WxaShopFundsSetBankAccountRequest>(nameof(WxaShopFundsSetBankAccountRequest.AccountInfo), "account_info", "官方原文拼写（请求体只此一支）");
        AssertJsonProperty<WxaShopFundsBankAccountInfo>(nameof(WxaShopFundsBankAccountInfo.BankAccountType), "bank_account_type", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsBankAccountInfo>(nameof(WxaShopFundsBankAccountInfo.AccountBank), "account_bank", "官方原文拼写（字段名是 account_bank 而非 bank_name）");
        AssertJsonProperty<WxaShopFundsBankAccountInfo>(nameof(WxaShopFundsBankAccountInfo.BankAddressCode), "bank_address_code", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsBankAccountInfo>(nameof(WxaShopFundsBankAccountInfo.BankBranchId), "bank_branch_id", "官方原文拼写（联行号，选填）");
        AssertJsonProperty<WxaShopFundsBankAccountInfo>(nameof(WxaShopFundsBankAccountInfo.AccountNumber), "account_number", "官方原文拼写（银行账号）");

        // 提现面（biz_type + mchid 三支必填、mchid 全小写无下划线；amount 官方明写单位分）。
        AssertJsonProperty<WxaShopFundsSubmitWithdrawRequest>(nameof(WxaShopFundsSubmitWithdrawRequest.MchId), "mchid", "官方原文拼写（全小写、无下划线）");
        AssertJsonProperty<WxaShopFundsSubmitWithdrawRequest>(nameof(WxaShopFundsSubmitWithdrawRequest.BizType), "biz_type", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsSubmitWithdrawRequest>(nameof(WxaShopFundsSubmitWithdrawRequest.BankMemo), "bank_memo", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsSubmitWithdrawResponse>(nameof(WxaShopFundsSubmitWithdrawResponse.OutRequestNo), "out_request_no", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsSubmitWithdrawResponse>(nameof(WxaShopFundsSubmitWithdrawResponse.QrcodeTicket), "qrcode_ticket", "官方原文拼写（无下划线分词）");
        AssertJsonProperty<WxaShopFundsWithdrawDetailResponse>(nameof(WxaShopFundsWithdrawDetailResponse.WithdrawInfo), "withdraw_info", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsWithdrawInfo>(nameof(WxaShopFundsWithdrawInfo.BankNum), "bank_num", "官方原文拼写（银行账户字段名为 bank_num）");
        AssertJsonProperty<WxaShopFundsWithdrawInfo>(nameof(WxaShopFundsWithdrawInfo.CreateTime), "create_time", "官方原文拼写（秒级时间戳）");
        AssertJsonProperty<WxaShopFundsScanWithdrawResponse>(nameof(WxaShopFundsScanWithdrawResponse.OutRequestNos), "out_request_nos", "官方原文拼写（复数 s）");

        // 流水面（请求 funds_flow_no、应答 funds_flow_id：官方同页两键并存，禁止合并）。
        AssertJsonProperty<WxaShopFundsFlowDetailRequest>(nameof(WxaShopFundsFlowDetailRequest.FundsFlowNo), "funds_flow_no", "官方请求参数表拼写（同页请求示例误写 funds_flow_id，以参数表为准）");
        AssertJsonProperty<WxaShopFundsFlowDetailResponse>(nameof(WxaShopFundsFlowDetailResponse.FundsFlow), "funds_flow", "官方原文拼写（单对象非列表）");
        AssertJsonProperty<WxaShopFundsFlowInfo>(nameof(WxaShopFundsFlowInfo.FundsFlowId), "funds_flow_id", "官方回包参数表拼写（流水id）");
        AssertJsonProperty<WxaShopFundsFlowInfo>(nameof(WxaShopFundsFlowInfo.BookkeepingTime), "bookkeeping_time", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsFlowInfo>(nameof(WxaShopFundsFlowInfo.BusiType), "busi_type", "官方原文拼写（busi 而非 business）");
        AssertJsonProperty<WxaShopFundsFlowInfo>(nameof(WxaShopFundsFlowInfo.RequestNo), "request_no", "官方原文拼写（业务单号，含义随 busi_type 变化）");
        AssertJsonProperty<WxaShopFundsFlowListRequest>(nameof(WxaShopFundsFlowListRequest.Date), "date", "官方原文拼写（yyyyMMdd 字符串，非时间戳）");
        AssertJsonProperty<WxaShopFundsFlowListResponse>(nameof(WxaShopFundsFlowListResponse.FundsFlowIds), "funds_flow_ids", "官方原文拼写");
        AssertJsonProperty<WxaShopFundsGenQrcodeRequest>(nameof(WxaShopFundsGenQrcodeRequest.IdentityType), "identity_type", "官方原文拼写");

        // 金额与时间类型口径（官方三套并存，禁止「顺手统一」）：
        typeof(WxaShopFundsBalanceInfo).GetProperty(nameof(WxaShopFundsBalanceInfo.AvailableAmount))!.PropertyType
            .Should().Be(typeof(long?), "余额两字段官方类型为 number ⇒ long?（官方本页未标单位，不得换算）");
        typeof(WxaShopFundsSubmitWithdrawRequest).GetProperty(nameof(WxaShopFundsSubmitWithdrawRequest.Amount))!.PropertyType
            .Should().Be(typeof(long?), "发起提现 amount 官方为 number 且明写「单位：分」⇒ 分口径 long?");
        typeof(WxaShopFundsFlowInfo).GetProperty(nameof(WxaShopFundsFlowInfo.Amount))!.PropertyType
            .Should().Be(typeof(string), "资金流水 amount 官方类型为 string、示例「0.10」（元、两位小数）⇒ 照录 string 不转 decimal");
        typeof(WxaShopFundsFlowInfo).GetProperty(nameof(WxaShopFundsFlowInfo.Balance))!.PropertyType
            .Should().Be(typeof(string), "资金流水 balance 官方类型为 string（语义「可提现+待结算」合计，与 getbalance 口径不同）");
        typeof(WxaShopFundsWithdrawInfo).GetProperty(nameof(WxaShopFundsWithdrawInfo.CreateTime))!.PropertyType
            .Should().Be(typeof(long?), "提现详情 create_time / update_time 官方为 number（秒级时间戳）");
        typeof(WxaShopFundsFlowListRequest).GetProperty(nameof(WxaShopFundsFlowListRequest.PageSize))!.PropertyType
            .Should().Be(typeof(int?), "page_size 官方为 number（本页另给 ≤100 上限，属参数校验）");

        // 官方自相矛盾处的照录：提现列表页回包参数表未列 total_num、示例却有 ⇒ 建成可空。
        typeof(WxaShopFundsScanWithdrawResponse).GetProperty(nameof(WxaShopFundsScanWithdrawResponse.TotalNum))!.PropertyType
            .Should().Be(typeof(long?), "total_num 只出现在官方回包示例（参数表未列）⇒ 可空，宿主不得假定必有");

        // 共用 DTO 裁决：查询结算账户与修改结算账户的账户对象同为 WxaShopFundsBankAccountInfo
        //（官方 AccountInfo 参数表只有一份，定义在「修改结算账户」页）。
        typeof(WxaShopFundsBankAccountResponse).GetProperty(nameof(WxaShopFundsBankAccountResponse.AccountInfo))!.PropertyType
            .Should().Be(typeof(WxaShopFundsBankAccountInfo), "官方 AccountInfo 唯一一份定义在修改结算账户页 ⇒ 查询应答共用同类型");
        typeof(WxaShopFundsSetBankAccountRequest).GetProperty(nameof(WxaShopFundsSetBankAccountRequest.AccountInfo))!.PropertyType
            .Should().Be(typeof(WxaShopFundsBankAccountInfo), "请求侧与应答侧共用 AccountInfo（字段集完全一致）");

        // 字段集边界锁定（防「顺手补齐」）：
        typeof(WxaShopFundsFlowListRequest).GetProperty("StartDate", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("官方流水列表只有单日 date 必填、无起止区间入参");
        typeof(WxaShopFundsGenQrcodeRequest).GetProperty("OutRequestNo", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("官方生成二维码请求体只有 identity_type，提现单号不经本端点绑定");
        typeof(WxaShopFundsSubmitWithdrawResponse).GetProperty("WithdrawId", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("本域提现单号官方键为 out_request_no；withdraw_id 属小店线 /channels/ec/funds/* 形态，不得串用");

        // 官方枚举表逐值锁定（多一个值即凭空建模）。
        typeof(WxaShopFundsBankAccountTypes).GetFields()
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { "ACCOUNT_TYPE_BUSINESS", "ACCOUNT_TYPE_PRIVATE" },
                because: "官方 BankAccountType 表恰两值（对公 / 经营者个人银行卡）");
        typeof(WxaShopFundsWithdrawStatuses).GetFields()
            .Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[]
            {
                "BEFORE_SCAN_QRCODE", "AFTER_SCAN_QRCODE", "CREATE_SUCCESS", "SUCCESS",
                "FAIL", "REFUND", "CLOSE", "INIT",
            }, because: "官方 WithDrawStatus 表恰 8 值（其中只有 SUCCESS 是到账态）");
        typeof(WxaShopFundsIdentityTypes).GetFields()
            .Where(static f => f.IsLiteral)
            .Select(static f => (int)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { 1 },
                because: "官方「枚举值-identity_type」表只公布 1（管理员）；0/2 属查询扫码状态页的 scan_user_type，不是本请求体");

        // 应答基类：本域全部响应继承 WxaResponse（errcode 判错与全线同源），响应对象恰 8 支。
        var responseDtos = typeof(WxaShopFundsBalanceResponse).Assembly.GetTypes()
            .Where(static t => !t.IsNested
                               && t.Namespace == DtoNamespace
                               && t.IsClass
                               && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToList();

        responseDtos.Should().HaveCount(8, "本域官方应答对象为 8（余额 + 结算账户 + 发起提现 + 提现详情 + 提现列表 + 二维码 + 流水列表 + 流水详情）");
        responseDtos.Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
            "资金域响应 DTO 必须继承 WxaResponse（判错走单点判定）");
    }

    /// <summary>
    /// 契约守卫 SHOPF6：资金敏感面红线 —— 银行账号 / 户名 / 提现金额 / 二维码 ticket 所在文件必须写明
    /// 「禁止写入日志、遥测或异常消息」，且本域源码零日志调用、零凭据字符串化。
    /// </summary>
    [Fact]
    public void ShopFundsSensitiveSurface_ShouldDeclareNoLoggingAndCarryRedLine()
    {
        // ① 三支接口文件都必须带个人金融信息红线（资金面与 §8 的收单/回调面同级）。
        var interfaceFiles = Directory.GetFiles(SourcePath("Mud.Wechat.MiniProgram", "Interfaces", "ShopFunds"), "*.cs", SearchOption.AllDirectories)
            .Where(static f => !IsBuildArtifact(f))
            .ToList();

        interfaceFiles.Should().HaveCount(3, "本域接口文件恰 3 支（余额与账户 / 提现与二维码 / 资金流水）——少一支说明扫描口径漂移");

        foreach (var file in interfaceFiles)
        {
            var text = Regex.Replace(File.ReadAllText(file), @"\s+", " ");
            text.Should().Contain("禁止写入日志、遥测或异常消息",
                $"{Path.GetFileName(file)} 必须写明银行账号 / 户名 / 提现金额等个人金融信息不得进日志、遥测或异常消息");
        }

        // ② 本域（接口 + DTO）源码零日志调用：SDK 侧任何 Logger/Console 输出都会把账号与金额带进日志管道。
        var domainSources = interfaceFiles
            .Concat(Directory.GetFiles(SourcePath("Mud.Wechat.MiniProgram", "DataModels", "ShopFunds"), "*.cs", SearchOption.AllDirectories))
            .Where(static f => !IsBuildArtifact(f))
            .ToList();

        domainSources.Should().HaveCountGreaterThan(20, "接口 3 + DTO 22 = 25 份源文件；数量骤减说明扫描口径漂移，本用例会静默空跑");

        var loggingOffenders = domainSources
            .Where(f => File.ReadAllLines(f).Any(line => IsCodeLine(line)
                                                         && (line.Contains("_logger", StringComparison.Ordinal)
                                                             || line.Contains("ILogger", StringComparison.Ordinal)
                                                             || line.Contains("Console.Write", StringComparison.Ordinal))))
            .Select(Path.GetFileName)
            .ToList();

        loggingOffenders.Should().BeEmpty(
            "资金域（余额 / 银行账户 / 提现 / 流水）源码不得出现日志调用——账号、户名、金额与 ticket 一律禁入日志（AGENTS §8）："
            + string.Join(", ", loggingOffenders));

        // ③ 敏感字段仍须逐字照官方键名（脱敏词表按精确名匹配，改名即失去保护）。
        var sensitiveJsonNames = new[] { "account_number", "bank_num", "qrcode_ticket", "verify_fail_reason" };
        var declared = new List<string>();
        var jsonNameRegex = new Regex(@"\[JsonPropertyName\(""(?<n>[a-z_]+)""\)\]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        foreach (var file in domainSources)
        {
            foreach (Match m in jsonNameRegex.Matches(File.ReadAllText(file)))
            {
                declared.Add(m.Groups["n"].Value);
            }
        }

        declared.Should().NotBeEmpty("防「提取口径失效导致真空断言」：必须真的扫出 JsonPropertyName 字面量");

        foreach (var name in sensitiveJsonNames)
        {
            declared.Should().Contain(name, $"{name} 属个人金融信息 / 凭据字段，JSON 名必须与官方完全一致");
        }

        declared.Should().NotContain("accountNumber", "敏感字段名不得驼峰化（脱敏与参数校验都按官方原文拼写匹配）");
    }

    /// <summary>
    /// 契约守卫 SHOPF7：与小店线同前缀<b>零交叉</b> —— 本域 9 条与小店线 <c>IChannelsFundsService</c>
    /// 已声明的 <c>/shop/funds/*</c> 路由字符串集不得相交，且本域路由不含 <c>/channels/</c> 前缀。
    /// </summary>
    [Fact]
    public void ShopFundsRoutes_ShouldNotIntersectChannelsLineSamePrefix()
    {
        var mine = Routes.Select(static r => r.Route).ToList();

        mine.Should().OnlyContain(r => !r.Contains("/channels/", StringComparison.Ordinal),
            "本域 9 条一律是官方 /shop/funds/* 原文路由：/channels/ec/* 属小店线令牌面，混用即跨线串号");

        var channelsFile = Directory.EnumerateFiles(Root, "IChannelsFundsService.cs", SearchOption.AllDirectories)
            .FirstOrDefault(static f => !IsBuildArtifact(f));
        channelsFile.Should().NotBeNull(
            "参照文件 IChannelsFundsService.cs 未找到：小店线工程目录漂移，本守卫的交叉判定会静默空跑");

        var channelsShopFunds = new HashSet<string>(
            RouteLiteralsInFile(channelsFile!, "/shop/funds/"),
            StringComparer.Ordinal);

        channelsShopFunds.Should().Contain("/shop/funds/getcity",
            "交叉判定的正对照：必须真的扫出小店线已声明的同前缀路由（否则零交集断言恒真）");
        channelsShopFunds.Should().NotBeEmpty("小店线同前缀路由集非空——为空即本用例空跑");

        var overlap = mine.Intersect(channelsShopFunds, StringComparer.Ordinal).ToList();
        overlap.Should().BeEmpty(
            "同一 /shop/funds/* 前缀被两线分持（本线 access_token / 小店线 Channels access_token），"
            + "路由字符串相交即两端点抢同一路由、令牌归属互斥：" + string.Join(", ", overlap));

        ChannelsOwnedShopFundsRoutes.Should().HaveCount(7,
            "同前缀归小店线的官方页为 7（银行与地区 5 + 取二维码图片 / 查扫码状态 2）");
        channelsShopFunds.Should().BeSubsetOf(ChannelsOwnedShopFundsRoutes.ToHashSet(StringComparer.Ordinal)
                .Union(mine),
            because: "小店线在本前缀下的声明须落在「7 条归他线 + 9 条归本线」的清单内，新增同前缀路由必须重新逐页取证");
    }

    /// <summary>
    /// 契约守卫 SHOPF8：扫码凭据单出口 + 两线分持面在本线零声明。
    /// <c>qrcode_ticket</c> 只允许由发起提现与生成二维码两支应答承载；官方未公布 ticket 有效期数字 ⇒
    /// 本域不得出现有效期类字段；小店线所属 7 条同前缀路由在 <c>Src/MiniProgram</c> 代码行零声明。
    /// </summary>
    [Fact]
    public void ShopFundsQrcodeTicket_ShouldHaveSingleSource_AndSamePrefixRoutesStayUndeclared()
    {
        var assembly = typeof(WxaShopFundsGenQrcodeResponse).Assembly;

        var ticketBearers = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace && t.IsClass && !t.IsAbstract)
            .Where(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "qrcode_ticket"))
            .Select(static t => t.Name)
            .ToList();

        ticketBearers.Should().BeEquivalentTo(new[]
        {
            nameof(WxaShopFundsSubmitWithdrawResponse),
            nameof(WxaShopFundsGenQrcodeResponse),
        }, because: "官方只有这两支应答返回 qrcode_ticket；第三处出现即凭空造凭据载体");

        // 有效期数字官方空缺 ⇒ 负锁。
        var expiryProps = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .SelectMany(static t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .Where(p => ForbiddenValidityPropertyTokens.Any(k => p.Name.Contains(k, StringComparison.OrdinalIgnoreCase)))
            .Select(static p => p.DeclaringType!.Name + "." + p.Name)
            .ToList();

        expiryProps.Should().BeEmpty(
            "官方 9 页均未给 ticket / 二维码的有效期数字（失效只以错误码 60220 / 60208 与 status=3 表达）"
            + "⇒ 本域不得凭空建有效期字段：" + string.Join(", ", expiryProps));

        // 小店线 7 条同前缀路由在小程序线代码行零声明（注释行不算——本线接口以注释声明该边界是刻意留档）。
        var declaredHere = RouteLiteralsUnder("/shop/funds/", Path.Combine("Src", "MiniProgram"));
        declaredHere.Should().Contain("/shop/funds/qrcode/gen",
            "SHOPF8 的正对照：必须真的扫出本域已声明的同前缀路由（否则零声明断言静默恒真）");

        var offenders = ChannelsOwnedShopFundsRoutes.Intersect(declaredHere, StringComparer.Ordinal).ToList();
        offenders.Should().BeEmpty(
            "银行与地区 5 条 + 取二维码图片 / 查扫码状态 2 条虽在小程序文档树内在册，本仓令牌面裁决归小店线"
            + "（Channels access_token）⇒ 小程序线声明之即令牌串号：" + string.Join(", ", offenders));
    }

    // ------------------------------------------------------------------
    // helpers
    // ------------------------------------------------------------------

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    /// <summary>代码行判定（注释行不算）：本域接口以 <c>///</c> 注释刻意留档跨线边界，粗口径会假红。</summary>
    private static bool IsCodeLine(string line)
    {
        var trimmed = line.TrimStart();
        return !(trimmed.StartsWith("//", StringComparison.Ordinal)
                 || trimmed.StartsWith("/*", StringComparison.Ordinal)
                 || trimmed.StartsWith("*", StringComparison.Ordinal));
    }

    /// <summary>从单个源文件中提取以 <paramref name="prefix"/> 开头的路由字面量（剥离注释行）。</summary>
    private static IEnumerable<string> RouteLiteralsInFile(string file, string prefix)
    {
        var code = string.Join("\n", File.ReadAllLines(file).Where(static l => IsCodeLine(l)));
        var regex = new Regex("\"(" + Regex.Escape(prefix) + "[A-Za-z0-9_\\-./]+)\"",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        foreach (Match m in regex.Matches(code))
        {
            yield return m.Groups[1].Value;
        }
    }

    private static bool IsBuildArtifact(string file)
    {
        var normalized = file.Replace('\\', '/');
        return normalized.Contains("/obj/", StringComparison.Ordinal)
               || normalized.Contains("/bin/", StringComparison.Ordinal);
    }

    /// <summary>扫描根下<b>代码行</b>里出现的以 <paramref name="prefix"/> 开头的路由字面量。</summary>
    private static HashSet<string> RouteLiteralsUnder(string prefix, string relativeRoot)
    {
        var root = Path.Combine(Root, relativeRoot);
        var regex = new Regex("\"(" + Regex.Escape(prefix) + "[A-Za-z0-9_\\-./]+)\"", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var set = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            if (IsBuildArtifact(file))
            {
                continue;
            }

            var code = File.ReadAllLines(file)
                .Where(static l => IsCodeLine(l))
                .Aggregate(new StringBuilder(), static (sb, l) => sb.Append(l).Append('\n'))
                .ToString();

            foreach (Match m in regex.Matches(code))
            {
                set.Add(m.Groups[1].Value);
            }
        }

        return set;
    }

    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
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

    /// <summary>按工程名定位源工程目录，再拼接子路径（目录再迁移时守卫不随之漂移）。</summary>
    private static string SourcePath(params string[] segments)
    {
        var projectName = segments[0];
        var projectDir = Directory.EnumerateFiles(Root, projectName + ".csproj", SearchOption.AllDirectories)
            .Where(static f => !IsBuildArtifact(f))
            .Select(static f => Path.GetDirectoryName(f))
            .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {projectName}.csproj");

        return Path.Combine(new[] { projectDir }.Concat(segments.Skip(1)).ToArray());
    }
}

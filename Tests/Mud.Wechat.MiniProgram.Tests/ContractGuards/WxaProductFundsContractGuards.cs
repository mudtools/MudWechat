// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.ProductFunds;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「基础交易工具（原标准版交易组件 V1）· 资金管理」域契约守卫（PF 系列：7 端点路由与动词 +
/// 令牌绑定与注册组 + 参数形态与裸应答裁决 + 官方字段名与两套形态负锁 + 敏感金额整型负锁 +
/// DTO 登记计数 + 跨线不交叉）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 <c>minishopopencomponent/API/funds</c> 分区 7 支，2026-10 逐页取证，全部在册、无判非契约条目）</b>：
/// 获取账户余额 / 获取资金流水 / 获取资金流水列表 / 获取提现记录 / 获取提现记录列表 / 修改结算账户 / 商户提现，
/// 均由单一可注入接口 <see cref="IWxaProductFundsService"/> 承载（本域无 IsAbstract 父接口，7 端点全 POST）。
/// </para>
/// <para>
/// <b>路由族统一 <c>/product/funds/*</c></b>：属小程序令牌面（基础交易工具 V1），与小店线 <c>/channels/ec/*</c> 零交叠（PF6）。
/// </para>
/// <para>
/// <b>逐页取证锁定的四处反直觉点</b>：
/// ① <c>order_flow.status</c> 官方「回包参数说明」标 number、枚举给 1..4，但两页回包示例与 scanorderflow「string 映射」列给的是
/// 字符串（ORDER_FLOW_STATUS_*）⇒ 实际线格式为字符串，建模 <c>string?</c>；
/// ② 两套提现应答形态不同：单条 <c>getwithdrawdetail</c> 平铺、时间键 <c>create_time</c>/<c>update_time</c>，列表 <c>scanwithdraw</c> 走 <c>list</c>、时间键 <c>wxpay_*</c> 且只有 <c>total_num</c>；
/// ③ 两套流水列表键名不同：流水列表 <c>order_flow</c>（含 <c>total_page</c>+<c>total_num</c>）对提现列表 <c>list</c>（仅 <c>total_num</c>）；
/// ④ <c>getbalance</c> 官方请求体为空对象 <c>{}</c> ⇒ 无 <c>[Body]</c> 参数（形态同无忧退货 refund）。
/// </para>
/// <para>
/// <b>敏感面（个人金融信息）</b>：银行账户（<c>account_number</c>/<c>bank_num</c>）、开户名/开户行、账户类型、提现金额、
/// 结算流水、二级商户号（<c>sub_mchid</c>）⇒ 各端点 XML remarks 已逐条写明「禁入日志、遥测或异常消息」。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：端点聚合计数（<c>ContractBaseline.MiniProgram.Endpoints</c>）、跨线路由零重叠（MP-X1）、
/// 带令牌接口计数（MP-X2）与 QT1 白名单由 <c>MiniProgramContractGuards</c> / <c>MpQueryTokenWhitelistGuard</c> 承担；本文件只锁本域精确契约。
/// </para>
/// </remarks>
public class WxaProductFundsContractGuards
{
    private const string RegistryGroupName = "ProductFunds";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.ProductFunds";

    /// <summary>本域官方路由前缀（7 支全部落在此族内）。</summary>
    private const string RoutePrefix = "/product/funds/";

    /// <summary>
    /// 资金管理域官方路由表（7 端点：路由与动词照官方原文，全 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaProductFundsService), nameof(IWxaProductFundsService.GetBalanceAsync), typeof(PostAttribute), "/product/funds/getbalance"),
        (typeof(IWxaProductFundsService), nameof(IWxaProductFundsService.GetOrderFlowAsync), typeof(PostAttribute), "/product/funds/getorderflow"),
        (typeof(IWxaProductFundsService), nameof(IWxaProductFundsService.ScanOrderFlowAsync), typeof(PostAttribute), "/product/funds/scanorderflow"),
        (typeof(IWxaProductFundsService), nameof(IWxaProductFundsService.GetWithdrawDetailAsync), typeof(PostAttribute), "/product/funds/getwithdrawdetail"),
        (typeof(IWxaProductFundsService), nameof(IWxaProductFundsService.ScanWithdrawAsync), typeof(PostAttribute), "/product/funds/scanwithdraw"),
        (typeof(IWxaProductFundsService), nameof(IWxaProductFundsService.SetBankAccountAsync), typeof(PostAttribute), "/product/funds/setbankaccount"),
        (typeof(IWxaProductFundsService), nameof(IWxaProductFundsService.SubmitWithdrawAsync), typeof(PostAttribute), "/product/funds/submitwithdraw"),
    };

    private static readonly Type[] DomainInterfaces =
    {
        typeof(IWxaProductFundsService),
    };

    /// <summary>官方返回体只有 errcode / errmsg 的端点（全仓口径：直接返回基类 <see cref="WxaResponse"/>，不建空壳应答）。</summary>
    private static readonly string[] BareResponseMethods =
    {
        nameof(IWxaProductFundsService.SetBankAccountAsync),
    };

    /// <summary>官方无业务请求体（请求体为空对象 {}）的端点：只有 CancellationToken、无 [Body] 参数。</summary>
    private static readonly string[] EmptyBodyMethods =
    {
        nameof(IWxaProductFundsService.GetBalanceAsync),
    };

    /// <summary>契约守卫 PF1：7 端点路由与动词必须与官方契约一致，声明面与路由表全集相等。</summary>
    [Fact]
    public void ProductFundsEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(7, "资金管理域官方开放面为 7 端点（官方 minishopopencomponent/API/funds 分区逐页取证）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(7, "路由表不得出现重复条目");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（照官方原文拼写，禁止改写）");

            // 本域 7 支官方一律 POST（含四支查询与两支列表——POST 查询是本域的动词口径，勿顺手改 GET）。
            attr.Should().BeOfType<PostAttribute>($"{route} 官方调用方式为 POST");
            httpAttribute.Should().Be(typeof(PostAttribute), $"{route} 路由表的动词登记须与官方一致");
        }

        Routes.Should().OnlyContain(static r => r.Route.StartsWith(RoutePrefix, StringComparison.Ordinal),
            "本域 7 支全部落在 /product/funds/ 族内");
        Routes.Should().OnlyContain(static r => r.Route == r.Route.ToLowerInvariant(),
            "本域官方路由一律全小写连写（getbalance/getorderflow/scanorderflow/… 无下划线，不得驼峰化）");

        // 声明面全集自证：Interfaces/ProductFunds/ 里出现的每条路由字面量都必须在路由表内，反向亦成立。
        var declared = DeclaredRoutesInProductFundsInterfaces();
        declared.Should().BeEquivalentTo(Routes.Select(static r => r.Route),
            "Interfaces/ProductFunds/ 的声明面必须与 PF1 路由表逐条相等（多一条即漏登记，少一条即路由漂移）");
    }

    /// <summary>契约守卫 PF2：接口令牌绑定、注册组（权限集无标注的小程序 access_token 面）。</summary>
    [Fact]
    public void ProductFundsInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup()
    {
        DomainInterfaces.Should().HaveCount(1, "资金管理域由单接口 IWxaProductFundsService 承载 7 端点（本域无 IsAbstract 父接口）");

        foreach (var iface in DomainInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方每页请求地址均以 access_token 为凭据）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（MP-X2 同口径，改绑小店/企微令牌会破坏单槽 Resolve）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约把 access_token 强制放 Query（MP-X3 同口径）");
            token.Name.Should().Be("access_token");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 ProductFunds 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序令牌底座（AddMpApp 硬前置）");
        }

        // DI 面（本波次共享文件未合入，走回退断言：注册组与本域一致 + 方法规模锁定）。
        DomainInterfaces
            .Select(static i => i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length)
            .Sum().Should().Be(Routes.Length, "接口声明方法总数须与 PF1 路由表一致（7）");
    }

    /// <summary>
    /// 契约守卫 PF3：参数形态 —— 六支「单 JSON 请求体 + 零业务 Query」、getbalance 无请求体（空对象 {}）；
    /// 六支返回本域应答 DTO、修改结算账户返回基类 <see cref="WxaResponse"/>。
    /// </summary>
    [Fact]
    public void ProductFundsEndpoints_ShouldUseSingleBodyOrEmptyBody()
    {
        foreach (var (iface, method, _, route) in Routes)
        {
            var target = iface.GetMethod(method)!;
            var parameters = target.GetParameters();

            parameters.Should().NotContain(static p => p.GetCustomAttribute<QueryAttribute>() != null,
                $"{route} 本域除 access_token 外无官方 Query 参数，业务参数须走 [Body]");
            parameters.Should().NotContain(static p => p.GetCustomAttribute<MultipartFormAttribute>() != null,
                $"{route} 本域官方无 multipart 通路");

            if (EmptyBodyMethods.Contains(method))
            {
                // getbalance 官方请求体为空对象 {} ⇒ 恰一个参数（CancellationToken）、零 [Body]。
                parameters.Should().ContainSingle(static p => p.ParameterType == typeof(CancellationToken),
                    $"{route} 官方无业务请求体，方法签名只应剩 CancellationToken");
                parameters.Count(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                    .Should().Be(0, $"{route} 官方请求体为空对象 {{}} ⇒ 不得建 [Body] 请求 DTO");
            }
            else
            {
                parameters.Count(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                    .Should().Be(1, $"{route} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
                parameters.Should().HaveCount(2, $"{route} 形态为 (request, cancellationToken = default)");
                parameters[1].ParameterType.Should().Be(typeof(CancellationToken), $"{route} 第二参数须为 CancellationToken");
                parameters[1].HasDefaultValue.Should().BeTrue($"{route} 的 CancellationToken 须有 default 缺省值");
            }

            if (BareResponseMethods.Contains(method))
            {
                ResultTypeOf(iface, method).Should().Be(typeof(WxaResponse),
                    $"{route} 官方返回体只有 errcode / errmsg ⇒ 复用基类 WxaResponse（全仓口径，不建空壳应答）");
            }
            else
            {
                ResultTypeOf(iface, method).Namespace.Should().Be(DtoNamespace,
                    $"{route} 有业务应答字段 ⇒ 必须返回本域应答 DTO");
            }
        }

        // 六支带请求体、一支（getbalance）空体；六支应答 DTO、一支裸 WxaResponse。
        Routes.Count(static r => BareResponseMethods.Contains(r.Method)).Should().Be(1, "本域裸应答端点为 1（setbankaccount）");
        Routes.Count(static r => ResultTypeOf(r.Interface, r.Method) == typeof(WxaResponse)).Should().Be(1, "setbankaccount 官方应答无业务字段");
        Routes.Select(static r => BodyTypeOf(r.Interface, r.Method)).Where(static t => t != typeof(void))
            .Should().OnlyContain(t => t.Namespace == DtoNamespace, "带请求体的六支其请求体类型全部落本域 DTO 命名空间");
    }

    /// <summary>
    /// 契约守卫 PF4：官方字段名原文 + 两套形态负锁 + status 字符串裁决 + 敏感金额一律整型（禁浮点）。
    /// </summary>
    [Fact]
    public void ProductFundsDataModels_ShouldLockOfficialFieldsAndShapes()
    {
        // status 名实不一致裁决：官方说明表标 number，但回包示例与 string 映射列给字符串 ⇒ 线格式为字符串。
        AssertPropertyType<WxaProductFundsOrderFlow>(nameof(WxaProductFundsOrderFlow.Status), typeof(string),
            "官方回包示例 status 为 ORDER_FLOW_STATUS_* 字符串（说明表 number 系文档不一致，实际以字符串落地）");

        // 两套提现应答形态不同：单条平铺（create_time/update_time）对列表（list + wxpay_*，无 total_page）。
        AssertJsonProperty<WxaProductFundsWithdrawDetailResponse>(nameof(WxaProductFundsWithdrawDetailResponse.CreateTime), "create_time", "官方单条侧原文拼写");
        AssertJsonProperty<WxaProductFundsWithdrawDetailResponse>(nameof(WxaProductFundsWithdrawDetailResponse.BankNum), "bank_num", "官方单条侧原文拼写（银行账户，敏感）");
        AssertJsonProperty<WxaProductFundsWithdrawRecord>(nameof(WxaProductFundsWithdrawRecord.WxpayCreateTime), "wxpay_create_time", "官方列表侧原文拼写（与单条侧 create_time 不同名，勿互抄）");
        typeof(WxaProductFundsWithdrawDetailResponse).GetProperty("List", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("单条提现应答为平铺单对象、无 list 包装");
        typeof(WxaProductFundsWithdrawRecord).GetProperty("BankNum", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("列表提现条目官方无 bank_num/bank_name（仅单条侧有）");
        AssertJsonProperty<WxaProductFundsScanWithdrawResponse>(nameof(WxaProductFundsScanWithdrawResponse.List), "list", "官方提现列表键名是 list");
        typeof(WxaProductFundsScanWithdrawResponse).GetProperty("TotalPage", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("scanwithdraw 官方只有 total_num，无 total_page");

        // 两套流水列表键名差异：流水列表 order_flow（含 total_page+total_num）对提现列表 list（仅 total_num）。
        AssertJsonProperty<WxaProductFundsScanOrderFlowResponse>(nameof(WxaProductFundsScanOrderFlowResponse.OrderFlows), "order_flow", "官方流水列表键名是 order_flow（非 list）");
        AssertJsonProperty<WxaProductFundsScanOrderFlowResponse>(nameof(WxaProductFundsScanOrderFlowResponse.TotalPage), "total_page", "官方流水列表侧有 total_page");
        AssertJsonProperty<WxaProductFundsOrderFlowResponse>(nameof(WxaProductFundsOrderFlowResponse.OrderFlow), "order_flow", "单笔流水应答是 order_flow 单对象包装");

        // 结算账户包装 + 敏感字段名（account_bank4show 照官方原文含数字 4）。
        AssertJsonProperty<WxaProductFundsSetBankAccountRequest>(nameof(WxaProductFundsSetBankAccountRequest.AccountInfo), "account_info", "官方请求体为 account_info 单对象包装");
        AssertJsonProperty<WxaProductFundsBankAccountInfo>(nameof(WxaProductFundsBankAccountInfo.AccountNumber), "account_number", "官方原文拼写（银行账号，敏感必填）");
        AssertJsonProperty<WxaProductFundsBankAccountInfo>(nameof(WxaProductFundsBankAccountInfo.AccountBank4Show), "account_bank4show", "官方原文拼写（前端展示值，含数字 4）");
        typeof(WxaProductFundsBankAccountInfo).GetProperty("AccountInfo", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("account_info 包装只在请求体根类，嵌套类不得自带同名键");

        // 敏感面字段名原文 + getbalance 三键。
        AssertJsonProperty<WxaProductFundsBalanceResponse>(nameof(WxaProductFundsBalanceResponse.AvailableAmount), "available_amount", "官方原文拼写");
        AssertJsonProperty<WxaProductFundsBalanceResponse>(nameof(WxaProductFundsBalanceResponse.PendingAmount), "pending_amount", "官方原文拼写");
        AssertJsonProperty<WxaProductFundsBalanceResponse>(nameof(WxaProductFundsBalanceResponse.SubMerchantId), "sub_mchid", "官方原文拼写（二级商户号，敏感）");
        AssertJsonProperty<WxaProductFundsOrderFlowRequest>(nameof(WxaProductFundsOrderFlowRequest.OrderId), "order_id", "官方原文拼写");
        AssertJsonProperty<WxaProductFundsSubmitWithdrawRequest>(nameof(WxaProductFundsSubmitWithdrawRequest.Amount), "amount", "官方原文拼写（提现金额，单位分）");
        AssertJsonProperty<WxaProductFundsSubmitWithdrawResponse>(nameof(WxaProductFundsSubmitWithdrawResponse.QrcodeTicket), "qrcode_ticket", "官方原文拼写（资金凭证）");

        // 金额单位负锁：本域唯一官方明写「单位分」的是 submitwithdraw.amount；其余金额端点官方未标注单位，
        // 不得据惯例断言 ⇒ 以「全部金额字段一律整型」做形态负锁（禁浮点）。
        AssertPropertyType<WxaProductFundsSubmitWithdrawRequest>(nameof(WxaProductFundsSubmitWithdrawRequest.Amount), typeof(long?), "官方标注单位分 ⇒ 整型（禁 double/decimal）");
        var moneyPropertyNames = new[]
        {
            "amount", "available_amount", "pending_amount", "price", "order_price", "discount_fee",
            "service_fee", "handling_fee", "refund_fee",
        };
        var floatingMoney = GetDomainDtoTypes()
            .SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .Where(p => moneyPropertyNames.Contains(p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? string.Empty, StringComparer.Ordinal))
            .Where(p => p.PropertyType == typeof(double) || p.PropertyType == typeof(double?)
                        || p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?))
            .Select(p => $"{p.DeclaringType!.Name}.{p.Name}")
            .ToList();
        floatingMoney.Should().BeEmpty(
            "本域所有金额字段必须为整型（分）⇒ 出现 double/decimal 即破坏单位口径（PF4 金额负锁）："
            + string.Join(", ", floatingMoney));
    }

    /// <summary>契约守卫 PF5：15 支 DTO 全量登记 ProductFundsJsonContext（SerializerClassName 一致）；命名空间清单锁死。</summary>
    [Fact]
    public void ProductFundsDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaProductFundsBalanceResponse).Assembly;
        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("资金管理域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 15 DTO = 请求 6 + 应答 6 + 嵌套/共用 3（order_flow / withdraw_record / bank_account_info）。
        dtoTypes.Should().HaveCount(15,
            "资金管理域 DTO 总数漂移须先核对官方文档再同批调整本守卫（请求 6 + 应答 6 + 嵌套 3 = 15）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 ProductFunds");
        }

        // ProductFundsJsonContext 由主控收口时统一生成（GenerateJsonContext.ps1），生成前本断言预期红。
        var contextType = assembly.GetType(DtoNamespace + ".ProductFundsJsonContext");
        contextType.Should().NotBeNull(
            "ProductFundsJsonContext 尚未生成：待主控跑 scripts/GenerateJsonContext.ps1（波次内子代理禁止跑该脚本）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");
        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"ProductFunds\")]，必须登记进 ProductFundsJsonContext（AOT 生死线）");
        }

        // 命名空间清单锁死：本域无常量类，除 15 DTO 与生成上下文外不得出现新类型。
        var unregistered = domainTypes.Where(t => !dtoTypes.Contains(t) && t != contextType).ToList();
        unregistered.Should().BeEmpty(
            "资金管理域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)));

        // 应答 DTO 必须继承 WxaResponse（errcode 判错与全线同源）。
        assembly.GetTypes()
            .Where(t => t.Namespace == DtoNamespace && t.IsClass && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
                "本域响应 DTO 必须继承 WxaResponse");
    }

    /// <summary>契约守卫 PF6：跨线不交叉——本域路由不含 /channels/ 前缀（属小程序令牌面，与小店线零混用）。</summary>
    [Fact]
    public void ProductFundsRoutes_ShouldNotCrossChannelsLine()
    {
        Routes.Should().NotContain(static r => r.Route.Contains("/channels/", StringComparison.Ordinal),
            "/product/funds/* 属小程序令牌面（基础交易工具 V1），与小店线 /channels/ec/* 不得混用");

        // 官方 7 页全部在册（200 + 正文有请求小节 + 位于 minishopopencomponent/API/funds 目录树）⇒ 本域无判非契约条目；
        // 反向自证：本仓任何 /product/funds/ 声明都必须在 PF1 路由表内（防后续按旧清单回填脱树路由）。
        DeclaredRoutesInProductFundsInterfaces().Should().NotContain(static r => !r.StartsWith(RoutePrefix, StringComparison.Ordinal),
            "Interfaces/ProductFunds/ 不得出现 /product/funds/ 之外的路由（本域路由族单一）");
    }

    // ------------------------------------------------------------------
    // 内部辅助
    // ------------------------------------------------------------------

    /// <summary>取端点方法的应答类型（<c>Task&lt;T&gt;</c> 解包为 <c>T</c>）。</summary>
    private static Type ResultTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        method.Should().NotBeNull($"{interfaceType.Name}.{methodName} 不存在（契约面漂移）");

        var returnType = method!.ReturnType;
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetGenericArguments()[0]
            : returnType;
    }

    /// <summary>取端点方法 <c>[Body]</c> 参数的声明类型；无 [Body] 参数（空体端点）返回 <see cref="void"/>。</summary>
    private static Type BodyTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var body = method!.GetParameters().SingleOrDefault(static p => p.GetCustomAttribute<BodyAttribute>() != null);
        return body?.ParameterType ?? typeof(void);
    }

    /// <summary>扫描 <c>Src/MiniProgram/Mud.Wechat.MiniProgram/Interfaces/ProductFunds/</c> 剥离注释后的路由字面量集合。</summary>
    private static HashSet<string> DeclaredRoutesInProductFundsInterfaces()
    {
        var root = Path.Combine(GetSolutionRoot(), "Src", "MiniProgram", "Mud.Wechat.MiniProgram", "Interfaces", "ProductFunds");
        Directory.Exists(root).Should().BeTrue($"扫描根必须存在（{root}）——目录漂移会让 PF1 的声明面自证静默恒真");

        var literalRegex = new Regex(@"""(/[A-Za-z0-9_\-./]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var set = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var code = string.Join("\n", Regex.Replace(File.ReadAllText(file), @"/\*[\s\S]*?\*/", " ")
                .Split('\n')
                .Where(static l => !l.TrimStart().StartsWith("//", StringComparison.Ordinal)));

            foreach (System.Text.RegularExpressions.Match m in literalRegex.Matches(code))
            {
                set.Add(m.Groups[1].Value);
            }
        }

        // 正对照：扫出的集合必须覆盖路由表全集，否则是扫描口径失效（而非「本域真的没有多余路由」）。
        foreach (var control in new[] { "/product/funds/getbalance", "/product/funds/submitwithdraw" })
        {
            set.Should().Contain(control, $"PF1 的声明面扫描必须能捕获本域在册路由（{control}）——否则全集断言静默恒真");
        }

        return set;
    }

    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }

    private static List<Type> GetDomainDtoTypes()
    {
        var assembly = typeof(WxaProductFundsBalanceResponse).Assembly;
        return assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace && t.IsClass && !t.IsAbstract)
            .ToList();
    }

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    private static void AssertPropertyType<T>(string propertyName, Type expectedType, string because = "")
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");
        property!.PropertyType.Should().Be(expectedType,
            $"{typeof(T).Name}.{propertyName} 的类型必须是 {expectedType.Name}{(because.Length > 0 ? "（" + because + "）" : string.Empty)}");
    }
}

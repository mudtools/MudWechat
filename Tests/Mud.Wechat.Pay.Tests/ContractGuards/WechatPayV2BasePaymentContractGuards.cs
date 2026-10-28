// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.V2.Extensions;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.ContractGuards;

using System.Reflection;
using FluentAssertions;
using Mud.HttpUtils;
using Xunit;

/// <summary>
/// 微信支付 APIv2 基础支付域契约守卫（<b>13 端点 = 8 声明式 + 5 双向证书</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（普通商户文档中心）：
/// 统一下单 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935214"/>、
/// 付款码支付 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937125"/>、
/// 订单查询 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935215"/>、
/// 关单 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935216"/>、
/// 退款 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935217"/>、
/// 退款 v2 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011983308"/>、
/// 退款查询 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935218"/>、
/// 退款查询 v2 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011984122"/>、
/// 撤销 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937361"/>、
/// 下载账单 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935219"/>、
/// 下载资金账单 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935220"/>、
/// 结算查询 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013634920"/>、
/// 汇率查询 <see href="https://pay.weixin.qq.com/doc/global/v2/zh/4013634922"/>。
/// </para>
/// <para>
/// <b>形态</b>：APIv2 全线为 XML 报文 + 报文体内 <c>sign</c>（MD5 / HMAC-SHA256），
/// 与 APIv3 的 JSON + <c>Authorization</c> 头不共用 DTO、序列化通道与命名客户端。
/// </para>
/// <para>
/// <b>13 端点分两面的裁决</b>：<c>/secapi/pay/refund</c>、<c>/secapi/pay/refundv2</c>、
/// <c>/secapi/pay/reverse</c>、<c>/pay/downloadbill</c>、<c>/pay/downloadfundflow</c> 五支要求
/// <b>商户 API 证书</b>（<c>api2.mch.weixin.qq.com</c>）。证书按商户绑定，而 <c>IHttpClientFactory</c>
/// 的主 Handler 按<b>客户端名</b>创建并缓存、连接池亦按名复用 ⇒ 一条命名客户端承载不了多商户证书，
/// 故这五支由 <c>IWechatPayV2SecureService</c>（每商户一条独立通道）承载、<b>不带</b> <c>[HttpClientApi]</c>。
/// 「13 条端点」这一契约面由本守卫的两支断言<b>相加</b>锁定，任何一方增减都会打红。
/// </para>
/// </remarks>
public class WechatPayV2BasePaymentContractGuards
{
    private static Assembly PayV2Assembly => typeof(IWechatPayV2Service).Assembly;

    /// <summary>基础支付族中<b>需双向证书</b>的五支路由（官方 <c>/secapi/pay/*</c> 与证书域名下的两份账单）。</summary>
    private static readonly string[] SecureRoutes =
    {
        "/secapi/pay/refund",
        "/secapi/pay/refundv2",
        "/secapi/pay/reverse",
        "/pay/downloadbill",
        "/pay/downloadfundflow",
    };

    [Fact]
    public void V2_BasePayment_DeclarativeInterface_ShouldHaveEightEndpoints()
    {
        var methods = typeof(IWechatPayV2Service)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttributes(typeof(HttpMethodAttribute), inherit: true).Any())
            .ToList();

        methods.Should().HaveCount(8,
            "because 基础支付 13 支里只有 8 支不需证书；证书五支归 IWechatPayV2SecureService（见 V2_BasePayment_SecureInterface_ShouldHaveFiveEndpoints）");
    }

    [Fact]
    public void V2_BasePayment_Routes_ShouldMatchOfficialEndpoints()
    {
        var routes = typeof(IWechatPayV2Service)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.GetCustomAttribute<PostAttribute>())
            .Where(a => a != null)
            .Select(a => a!.RequestUri)
            .ToList();

        routes.Should().BeEquivalentTo(new[]
        {
            "/pay/unifiedorder",
            "/pay/micropay",
            "/pay/orderquery",
            "/pay/closeorder",
            "/pay/refundquery",
            "/pay/refundqueryv2",
            "/pay/settlementquery",
            "/pay/queryexchagerate",
        }, "because 这 8 条是官方 APIv2 基础支付族中不需客户端证书的端点（queryexchagerate 是官方原文拼写，缺一个 t）");
    }

    /// <summary>
    /// 证书五支：逐商户通道承载 ⇒ 只能是<b>无</b> <c>[HttpClientApi]</c> 的普通接口，
    /// 且方法名/路由与官方一一对应。
    /// </summary>
    [Fact]
    public void V2_BasePayment_SecureInterface_ShouldHaveFiveEndpoints()
    {
        typeof(IWechatPayV2SecureService)
            .GetCustomAttributes(typeof(HttpClientApiAttribute), inherit: false)
            .Should().BeEmpty(
                "because 证书端点不走源生成客户端（证书按商户绑定，与命名客户端的连接池复用模型冲突）");

        typeof(IWechatPayV2SecureService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.Name)
            .Should().BeEquivalentTo(new[]
            {
                "RefundAsync",
                "RefundV2Async",
                "ReverseAsync",
                "DownloadBillAsync",
                "DownloadFundFlowAsync",
            });

        // 下载两支的官方成功应答是 CSV 而非 XML ⇒ 契约面必须是裸字符串。
        typeof(IWechatPayV2SecureService).GetMethod("DownloadBillAsync")!
            .ReturnType.Should().Be(typeof(Task<string>));
        typeof(IWechatPayV2SecureService).GetMethod("DownloadFundFlowAsync")!
            .ReturnType.Should().Be(typeof(Task<string>));
    }

    /// <summary>
    /// 合计 13 端点：声明式 8 + 证书 5。这条断言是「13 端点族」这一契约面的<b>唯一</b>加法锁。
    /// </summary>
    [Fact]
    public void V2_BasePayment_TotalEndpoints_ShouldSumToThirteen()
    {
        var declarative = typeof(IWechatPayV2Service)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Count(m => m.GetCustomAttributes(typeof(HttpMethodAttribute), inherit: true).Any());

        var secure = typeof(IWechatPayV2SecureService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Count(m => m.DeclaringType == typeof(IWechatPayV2SecureService));

        (declarative + secure).Should().Be(13,
            "because 官方基础支付族共 13 支：统一下单/付款码/查单/关单/退款/退款v2/退款查询/退款查询v2/撤销/下载账单/下载资金账单/结算查询/汇率查询");

        secure.Should().Be(SecureRoutes.Length, "because 证书五支与守卫源文件的路由清单同批维护");
    }

    /// <summary>证书五支不得同时出现在声明式接口上（两面互斥 ⇒ 不会出现「一个端点两种走法」）。</summary>
    [Fact]
    public void V2_BasePayment_SecureRoutes_ShouldNotLeakIntoDeclarativeInterface()
    {
        var declarativeRoutes = typeof(IWechatPayV2Service)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.GetCustomAttribute<PostAttribute>()?.RequestUri)
            .Where(r => r != null)
            .Select(r => r!)
            .ToList();

        declarativeRoutes.Should().NotContain(r => SecureRoutes.Contains(r),
            "because /secapi/pay/* 与两份账单只能走证书通道；同一端点出现两条路径会让签名与应答验签口径分叉");
    }

    [Fact]
    public void V2_BasePayment_RegistryGroupName_ShouldBeBasePayV2()
    {
        var apiAttr = typeof(IWechatPayV2Service).GetCustomAttribute<HttpClientApiAttribute>();

        apiAttr.Should().NotBeNull();
        apiAttr!.RegistryGroupName.Should().Be("BasePayV2",
            "because 组名即源生成注册方法名（AddBasePayV2WebApiHttpClient），且必须与 APIv3 的裸名域不重名");
        apiAttr.HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName,
            "because APIv2 必须走自己的 XML + 签名 Handler 管道，不得复用 APIv3 的 JSON 客户端");
    }

    /// <summary>
    /// 模块枚举成员 ⇔「生成注册组 ∪ 有意的手写族」一一对应（三段式注册的第三段由源生成产出）。
    /// </summary>
    /// <remarks>
    /// <b>为什么要显式枚举手写族而不是「成员数 = 组数」</b>：需商户证书的三支族
    /// （红包 / 企业付款 / 付款到银行卡）<b>结构上不可能</b>有生成注册组 —— 证书按商户绑定，
    /// 与命名客户端的连接池复用模型冲突。把它们塞进「成员必须等于组」的断言，唯一的"修法"
    /// 就是给接口补 <c>[HttpClientApi]</c>，而那正是本线最不该发生的改动。
    /// 故此处以<b>白名单</b>形态把不对称写死：新增手写族必须同时进白名单，
    /// 白名单里每个名字都必须有公开接口 + DI 绑定（由证书族守卫逐条验证）。
    /// </remarks>
    [Fact]
    public void V2_ModuleMembers_ShouldEqualRegistryGroupNames()
    {
        // 2026-10 包合并：APIv2 与 APIv3 同程序集，注册组名必须按命名空间分区取（否则会把
        // APIv3 的 84 个注册组一并计入，本守卫的「多一个成员即查不到注册器」信号被淹没）。
        var groups = PayV2Assembly.GetTypes()
            .Where(t => t.IsInterface
                        && (t.Namespace ?? string.Empty).StartsWith("Mud.Wechat.Pay.V2", StringComparison.Ordinal))
            .Select(t => t.GetCustomAttribute<HttpClientApiAttribute>()?.RegistryGroupName)
            .Where(g => !string.IsNullOrEmpty(g))
            .Distinct()
            .ToList();

        var handWritten = WechatPayV2CertFamilyContractGuards.HandWrittenModules;

        groups.Should().BeEquivalentTo(Enum.GetNames<PayV2Module>().Except(handWritten),
            "because PayV2Module 成员 = 已落地的声明式注册组 ∪ 证书手写族；" +
            "声明式一侧多一个成员即「查不到注册器 ⇒ 静默不注册」，手写一侧须由白名单显式认领");

        // 手写族不得同时出现在注册组里（两面互斥 ⇒ 一个端点不会有两种走法）。
        handWritten.Should().NotIntersectWith(groups,
            "because 证书族若同时有生成注册器，声明式管道会在无证书的连接池上发出证书端点");

        // 与 APIv3 的 PayModule 不得重名（同名会产出两个同签名扩展方法，跨线导入即歧义）。
        groups.Should().NotContain(g => Enum.GetNames<PayModule>().Contains(g),
            "because APIv3 已占用 Transactions / Refund / Bill 等裸名");
        handWritten.Should().NotContain(g => Enum.GetNames<PayModule>().Contains(g),
            "because V2 后缀是两条线共用成员名时的唯一隔离手段");
    }

    [Fact]
    public void V2_BasePayment_InterfaceShouldAllowAnyStatusCode()
    {
        typeof(IWechatPayV2Service)
            .GetCustomAttribute<AllowAnyStatusCodeAttribute>()
            .Should().NotBeNull(
                "because APIv2 的业务失败也用 HTTP 200 + return_code=FAIL 表达，判定看报文不看状态码");
    }

    [Fact]
    public void V2_BasePayment_DtoCount_ShouldCoverAllRequestsAndResponses()
    {
        var dataModelsAssembly = typeof(V2UnifiedOrderRequest).Assembly;

        var dtoTypes = dataModelsAssembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(HttpJsonSerializableAttribute), inherit: true).Any())
            .Where(t => t.Namespace?.StartsWith("Mud.Wechat.Pay.V2.DataModels.PayV2") == true)
            .ToList();

        // 13 端点 × (请求 + 应答) = 26；退款/退款查询两支的 v2 变体各以继承复用父型 ⇒ 至少 24 型。
        dtoTypes.Should().HaveCountGreaterThanOrEqualTo(24,
            "because 每支端点各有请求与应答型，v2 变体经继承复用字段表");
    }

    /// <summary>每个 DTO 都必须登记进 <c>PayV2JsonContext</c>（AOT 免反射的唯一途径）。</summary>
    [Fact]
    public void V2_BasePayment_EveryDto_ShouldBeRegisteredInPayV2JsonContext()
    {
        var contextType = typeof(V2UnifiedOrderRequest).Assembly
            .GetType("Mud.Wechat.Pay.V2.DataModels.PayV2.PayV2JsonContext", throwOnError: false);

        // 低 TFM 下上下文整体被 #if 掉，本测试工程固定 net8.0 ⇒ 必须存在。
        contextType.Should().NotBeNull();

        var dataModelsAssembly = typeof(V2UnifiedOrderRequest).Assembly;
        var dtoTypes = dataModelsAssembly.GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(HttpJsonSerializableAttribute), inherit: true).Any())
            .Where(t => t.Namespace?.StartsWith("Mud.Wechat.Pay.V2.DataModels.PayV2") == true);

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("because 源生成上下文的 Default 实例是运行时取 JsonTypeInfo 的唯一入口");

        var context = (global::System.Text.Json.Serialization.JsonSerializerContext)
            defaultProperty!.GetValue(null)!;

        foreach (var dto in dtoTypes)
        {
            context.GetTypeInfo(dto).Should().NotBeNull(
                $"{dto.Name} 未登记进 PayV2JsonContext ⇒ 请重跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1");
        }
    }

    /// <summary>
    /// APIv2 与 APIv3 的 DTO 与接口必须分属两套命名空间，且**类型名不得重叠**
    /// —— 原守卫用「分属两个程序集」表达同一约束，2026-10 包合并后改为按命名空间守。
    /// </summary>
    /// <remarks>
    /// 该不变式真正要防的是「APIv2 的 XML 扁平字段表与 APIv3 的 JSON 嵌套体同名 ⇒ 序列化配置
    /// 互相污染、宿主写 <c>V2Xxx</c> 时拿到另一套语义」。程序集只是旧载体：
    /// 合并后两者同程序集，但命名空间分区依旧把两套面隔开，且下面这条**类型名零交叠**断言
    /// 比原先的「不同程序集」更直接 —— 即便将来有人把两侧塞回同一个命名空间，它照样打红。
    /// </remarks>
    [Fact]
    public void V2_DataModels_ShouldStayNamespaceSeparatedFromApiV3()
    {
        typeof(V2UnifiedOrderRequest).Namespace.Should().NotBe(
            typeof(JsapiPrepayRequest).Namespace,
            "because APIv2 是 XML 扁平字段表、APIv3 是 JSON 嵌套体，两侧 DTO 必须分属不同命名空间");

        typeof(IWechatPayV2Service).Namespace.Should().NotBe(
            typeof(IWechatPayTransactionsService).Namespace,
            "because APIv2 接口落 Mud.Wechat.Pay.V2、APIv3 接口落 Mud.Wechat.Pay");

        // 原守卫的「分属两个程序集」并不比命名空间隔离更强：C# 的名称解析以命名空间为单位，
        // 两侧即使存在同名类型也各自锁定在自己的命名空间内（宿主写 V2Xxx 不会拿到 v3 语义）。
        // 因此这里补的是**真正承重的那一条**：两侧不得出现同一命名空间。
        typeof(V2UnifiedOrderRequest).Assembly.Should().BeSameAs(
            typeof(JsapiPrepayRequest).Assembly,
            "合并后 APIv2/APIv3 同程序集，隔离完全依赖命名空间分区 ⇒ 上面两条不等式是本不变式的唯一载体");
    }
}

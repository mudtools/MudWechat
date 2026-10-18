// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.DataModels.AbnormalFundProcessing;

namespace Mud.Wechat.Pay.Tests.ContractGuards;

/// <summary>
/// P1-2 平台/服务商解决方案「在途异常资金处理」域契约守卫 —— <b>3 端点</b>：付款单查询 + 付款指令发起 + 指令执行结果查询。
/// </summary>
/// <remarks>
/// <para><b>官方只开放查询与指令两支</b>：第三支是指令执行结果查询 ⇒ 三支应答<b>不同形</b>，各建一型。</para>
/// <para><b>本域路由前缀为 <c>/v3/abnormal-fund-processing/</c></b>，全部是【普通商户】面。</para>
/// <para><b>红线口径</b>：本域全部端点零 <c>[Token]</c>（支付线凭据是商户 RSA 私钥签名）、
/// 全量绑 <c>[AllowAnyStatusCode]</c>（APIv3 的 4xx 错误体须原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>）。</para>
/// </remarks>
public class WechatPayAbnormalFundProcessingContractGuards
{
    private const string RegistryGroupName = "AbnormalFundProcessing";

    private const string DomainNamespace = "Mud.Wechat.Pay.DataModels.AbnormalFundProcessing";

    private static readonly Type Service = typeof(IWechatPayAbnormalFundProcessingService);

    /// <summary>AFP-B1：拓扑 —— 一个注册组一个接口、3 个方法、3 条 (方法, 路由)、动词集 GET / POST；路由前缀 /v3/abnormal-fund-processing。</summary>
    [Fact]
    public void AbnormalFundProcessingTopology_ShouldBeOneInterfaceOnOneGroup()
    {
        var api = Service.GetCustomAttributes(false)
            .Single(static a => a.GetType().Name == "HttpClientApiAttribute");
        api.GetType().GetProperty("RegistryGroupName")!.GetValue(api)
            .Should().Be(RegistryGroupName, "组名即源生成注册入口名（改组名即改 Add{组}WebApiHttpClient）");
        api.GetType().GetProperty("HttpClient")!.GetValue(api)
            .Should().Be(WechatPayHttpClientNames.TypeName, "必须绑支付专用命名客户端（商户签名通道）");

        Service.GetCustomAttributes(false)
            .Should().Contain(static a => a.GetType().Name == "AllowAnyStatusCodeAttribute",
                "缺 [AllowAnyStatusCode] ⇒ APIv3 业务失败的 4xx 错误体无落点");
        Service.GetCustomAttributes(false)
            .Should().NotContain(static a => a.GetType().Name == "TokenAttribute",
                "支付线无 access_token（红线 PAY-B1）");
        Service.Namespace.Should().Be("Mud.Wechat.Pay", "支付线接口一律扁平命名空间");

        Methods().Should().HaveCount(3, "在途异常资金处理声明式方法数");
        DistinctVerbRoutes().Should().HaveCount(3, "本域无同路由多方法分身");
        Verbs().Should().BeEquivalentTo(new[] { "GetAttribute", "PostAttribute" },
            "动词集照官方页面逐支登记：本域写操作动词不统一，合并即造出官方没有的端点");
        Routes().Should().OnlyContain(static r => r.StartsWith("/v3/abnormal-fund-processing", StringComparison.Ordinal),
            "本域路由全在 /v3/abnormal-fund-processing 前缀下");

        // 正对照：动词与路由扫描按接口真实推导
        VerbsOf(typeof(IWechatPayMerchantFundService)).Should().BeEquivalentTo(new[] { "GetAttribute" });
        RoutesOf(typeof(IWechatPayMerchantFundService)).Should().HaveCount(4);
    }

    /// <summary>AFP-B2：3 支端点的路由、path 键与 query 名册（含官方必填口径）逐支锁定。</summary>
    [Fact]
    public void AbnormalFundProcessingEndpoints_ShouldMatchOfficialRoutesAndParameterRosters()
    {
        RouteOf(nameof(IWechatPayAbnormalFundProcessingService.QueryAbnormalFundReceiptAsync)).Should().Be("/v3/abnormal-fund-processing/receipts/{receiptId}");
        PathKeysOf(nameof(IWechatPayAbnormalFundProcessingService.QueryAbnormalFundReceiptAsync)).Should().BeEquivalentTo(new[] { "receiptId" });
        // 在途异常资金付款单查询｜官方页 partner/4015408031｜身份 普通商户

        RouteOf(nameof(IWechatPayAbnormalFundProcessingService.CreateAbnormalFundTransferInstructionAsync)).Should().Be("/v3/abnormal-fund-processing/receipts/{receiptId}/transfer-instructions");
        PathKeysOf(nameof(IWechatPayAbnormalFundProcessingService.CreateAbnormalFundTransferInstructionAsync)).Should().BeEquivalentTo(new[] { "receiptId" });
        // 在途异常资金付款指令发起｜官方页 partner/4015408022｜身份 普通商户

        RouteOf(nameof(IWechatPayAbnormalFundProcessingService.QueryAbnormalFundTransferInstructionAsync)).Should().Be("/v3/abnormal-fund-processing/receipts/{receiptId}/transfer-instructions/{outInstructionNo}");
        PathKeysOf(nameof(IWechatPayAbnormalFundProcessingService.QueryAbnormalFundTransferInstructionAsync)).Should().BeEquivalentTo(new[] { "receiptId", "outInstructionNo" });
        // 在途异常资金付款指令执行结果查询｜官方页 partner/4015408022｜身份 普通商户
    }

    /// <summary>AFP-B3：官方字段表覆盖 —— DTO 属性名须与官方字段逐字对应（含大小写），不可手改或归并。</summary>
    [Fact]
    public void AbnormalFundProcessingDtoFields_ShouldCoverOfficialFieldTables()
    {
        // Receipt 响应（查询付款单）
        ShouldHaveJsonProp<AbnormalFundProcessingReceiptResponse>("receipt_id", "微信付款单号");
        ShouldHaveJsonProp<AbnormalFundProcessingReceiptResponse>("receipt_state", "在途异常资金付款状态");

        // InstructionCreate 请求（发起指令）
        ShouldHaveJsonProp<AbnormalFundProcessingInstructionCreateRequest>("out_instruction_no", "商户指令单号");
        ShouldHaveJsonProp<AbnormalFundProcessingInstructionCreateRequest>("transfer_mode", "付款方式");

        // Instruction 响应（查询指令）
        ShouldHaveJsonProp<AbnormalFundProcessingInstructionResponse>("instruction_state", "指令执行状态");
    }

    // ---- helpers -------------------------------------------------------------

    private static MethodInfo FindMethod(string methodName)
        => Service.GetMethods().FirstOrDefault(m => m.Name == methodName)
           ?? throw new InvalidOperationException($"IWechatPayAbnormalFundProcessingService.{methodName} 必须存在");

    private static string RouteOf(string methodName)
    {
        var attribute = FindMethod(methodName).GetCustomAttribute<HttpMethodAttribute>();
        attribute.Should().NotBeNull($"{methodName} 必须声明动词特性");
        return attribute!.RequestUri!;
    }

    private static IReadOnlyList<string> PathKeysOf(string methodName)
    {
        var method = FindMethod(methodName);
        var keys = new List<string>();
        foreach (var p in method.GetParameters())
        {
            foreach (var attr in p.GetCustomAttributes(false))
            {
                if (attr.GetType().Name == "PathAttribute")
                    keys.Add(p.Name ?? "");
            }
        }
        return keys;
    }

    private static IReadOnlyList<MethodInfo> Methods()
        => Service.GetMethods().OrderBy(static m => m.Name).ToList();

    private static IReadOnlyList<string> Verbs()
        => Methods().SelectMany(MethodVerbs).Distinct().OrderBy(static v => v).ToList();

    private static IReadOnlyList<string> Routes()
        => Methods().Select(m => RouteOf(m.Name)).OrderBy(static r => r).ToList();

    private static IReadOnlyList<string> DistinctVerbRoutes()
        => Methods().Select(m => $"{MethodVerbs(m).FirstOrDefault()} {RouteOf(m.Name)}").Distinct().OrderBy(static r => r).ToList();

    private static IReadOnlyList<string> VerbsOf(Type t)
        => t.GetMethods().SelectMany(MethodVerbs).Distinct().OrderBy(static v => v).ToList();

    private static IReadOnlyList<string> RoutesOf(Type t)
        => t.GetMethods().Select(m =>
        {
            var attr = m.GetCustomAttribute<HttpMethodAttribute>();
            return attr?.RequestUri ?? "";
        }).OrderBy(static r => r).ToList();

    private static IEnumerable<string> MethodVerbs(MethodInfo m)
    {
        foreach (var attr in m.GetCustomAttributes(false))
        {
            var name = attr.GetType().Name;
            if (name is "GetAttribute" or "PostAttribute" or "PutAttribute" or "DeleteAttribute" or "PatchAttribute")
                yield return name;
        }
    }

    private static void ShouldHaveJsonProp<T>(string jsonName, string reason)
    {
        var props = typeof(T).GetProperties();
        var found = false;
        foreach (var p in props)
        {
            var attr = p.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (attr != null && attr.Name == jsonName)
            {
                found = true;
                break;
            }
        }
        found.Should().BeTrue($"DTO {typeof(T).Name} 须有官方字段 <c>{jsonName}</c>（{reason}）");
    }
}

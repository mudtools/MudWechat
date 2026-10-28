// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.V2.DataModels.PayV2;
using Mud.Wechat.Pay.V2.Extensions;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.ContractGuards;

using System.Reflection;
using FluentAssertions;
using Mud.HttpUtils;
using Xunit;

/// <summary>
/// 微信支付 APIv2「委托代扣（PAP）」双族契约守卫（<b>11 端点 = 商户侧 6 + 服务商侧 5</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：商户侧 签约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987638"/>、
/// 扣款 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987639"/>、
/// 预签约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987641"/>、
/// H5 签约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987642"/>、
/// 查签约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987640"/>、
/// 解约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987643"/>；
/// 服务商侧 扣款 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988375"/>、
/// 预签约 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988377"/>、
/// H5 签约 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988378"/>、
/// 查签约 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988379"/>、
/// 解约（与商户侧同路径）。</para>
/// <para>
/// <b>形态</b>：官方逐页标注「需商户证书：<b>否</b>」⇒ 全 11 支走声明式管道（XML + 体内 <c>sign</c>），
/// 不建证书通道。<b>两支 <c>h5entrustweb</c> 是 GET</b>（跳转链接，参数在 Query），签名由
/// <c>WechatPayV2SigningHandler</c> 的 Query 分支追加 <c>sign</c> 参数完成 —— 这是 APIv2 全线唯一
/// 使用 GET 的族，故本守卫额外锁定「GET 端点不得带请求 DTO」。
/// </para>
/// <para>
/// <b>签名算法缺省的不对称</b>：POST 族 <c>sign_type</c> 缺省为 <b>MD5</b>，而两支
/// <c>h5entrustweb</c> 页官方缺省为 <b>HMAC-SHA256</b> ⇒ 接口以默认参数显式钉住 HMAC-SHA256，
/// 该默认值属契约面，改动即打红。
/// </para>
/// </remarks>
public class WechatPayV2PapayContractGuards
{
    private static readonly string[] MerchantRoutes =
    {
        "/pay/contractorder",
        "/pay/pappayapply",
        "/papay/preentrustweb",
        "/papay/h5entrustweb",
        "/papay/querycontract",
        "/papay/deletecontract",
    };

    private static readonly string[] PartnerRoutes =
    {
        "/pay/partner/pappayapply",
        "/papay/partner/preentrustweb",
        "/papay/partner/h5entrustweb",
        "/papay/partner/querycontract",
        "/papay/deletecontract",
    };

    [Fact]
    public void V2_Papay_MerchantInterface_ShouldHaveSixEndpoints()
    {
        RoutesOf<IWechatPayV2PapayService>().Should().BeEquivalentTo(MerchantRoutes,
            "because 商户侧委托代扣官方开放 6 支：签约/扣款/预签约/H5签约/查签约/解约");

        typeof(IWechatPayV2PapayService)
            .GetCustomAttributes(typeof(AllowAnyStatusCodeAttribute), inherit: false)
            .Should().NotBeEmpty("because APIv2 业务失败也是 HTTP 200 + return_code=FAIL");
    }

    [Fact]
    public void V2_Papay_PartnerInterface_ShouldHaveFiveEndpoints()
    {
        RoutesOf<IWechatPayV2PartnerPapayService>().Should().BeEquivalentTo(PartnerRoutes,
            "because 服务商侧官方 5 支：扣款/预签约/H5签约/查签约/解约（签约入口 /pay/contractorder 不向服务商开放）");

        typeof(IWechatPayV2PartnerPapayService)
            .GetCustomAttributes(typeof(AllowAnyStatusCodeAttribute), inherit: false)
            .Should().NotBeEmpty();
    }

    /// <summary>合计 11 端点的加法锁：族总量单一来源，两侧任一支增减都会打红。</summary>
    [Fact]
    public void V2_Papay_TotalEndpoints_ShouldSumToEleven()
    {
        (RoutesOf<IWechatPayV2PapayService>().Count + RoutesOf<IWechatPayV2PartnerPapayService>().Count)
            .Should().Be(11, "because P1 批次的委托代扣双族官方共 11 支，且全部不需商户证书");
    }

    [Fact]
    public void V2_Papay_RegistryGroupNames_ShouldMatchModuleMembers()
    {
        typeof(IWechatPayV2PapayService).GetCustomAttribute<HttpClientApiAttribute>()!
            .RegistryGroupName.Should().Be(nameof(PayV2Module.PapayV2));
        typeof(IWechatPayV2PartnerPapayService).GetCustomAttribute<HttpClientApiAttribute>()!
            .RegistryGroupName.Should().Be(nameof(PayV2Module.PartnerPapayV2));

        foreach (var iface in new[] { typeof(IWechatPayV2PapayService), typeof(IWechatPayV2PartnerPapayService) })
        {
            iface.GetCustomAttribute<HttpClientApiAttribute>()!
                .HttpClient.Should().Be(WechatPayV2HttpClientNames.TypeName,
                    "because APIv2 端点必须走 XML + 签名 Handler 管道，不得复用 APIv3 客户端");
        }
    }

    /// <summary>两支 H5 签约 GET：无请求 DTO、参数全为 <c>[Query]</c>，且 <c>sign_type</c> 默认钉为 HMAC-SHA256。</summary>
    [Fact]
    public void V2_Papay_H5EntrustWeb_ShouldBeQueryOnlyWithHmacDefault()
    {
        foreach (var iface in new[] { typeof(IWechatPayV2PapayService), typeof(IWechatPayV2PartnerPapayService) })
        {
            var method = iface.GetMethod("H5EntrustWebAsync")!;

            method.GetCustomAttribute<GetAttribute>().Should().NotBeNull();
            method.GetCustomAttribute<PostAttribute>().Should().BeNull();

            var parameters = method.GetParameters();
            var queryNames = parameters
                .Select(p => p.GetCustomAttribute<QueryAttribute>()?.Name)
                .ToList();
            var bodyParameterNames = parameters
                .Where(p => p.GetCustomAttribute<BodyAttribute>() != null)
                .Select(p => p.Name)
                .ToList();

            bodyParameterNames.Should().BeEmpty(
                "because APIv2 的 GET 端点参数在 Query，声明式管道不得为跳转链接造一个无人消费的信封 DTO");
            queryNames.Should().Contain("sign_type");

            var signType = parameters[queryNames.IndexOf("sign_type"!)];
            signType.HasDefaultValue.Should().BeTrue();
            signType.DefaultValue.Should().Be("HMAC-SHA256",
                "because 官方 h5entrustweb 页的 sign_type 缺省即 HMAC-SHA256，与 POST 族的 MD5 缺省不同");

            var version = parameters[queryNames.IndexOf("version"!)];
            version.DefaultValue.Should().Be("1.0", "because 官方 version 固定值 1.0");
        }
    }

    /// <summary>商户侧与服务商侧 H5 签约的必填 Query 集必须一致（服务商侧多 sub_mch_id 与风控参数）。</summary>
    [Fact]
    public void V2_Papay_H5EntrustWeb_ShouldDeclareOfficialRiskParameterNames()
    {
        var names = QueryNamesOf<IWechatPayV2PartnerPapayService>("H5EntrustWebAsync");

        // 官方 PAP 风控入参名照抄，不做驼峰「修正」；这些是用户身份信息，SDK 侧不落日志、不入库。
        names.Should().Contain(new[] { "sub_appid", "deviceid", "mobile", "email", "qq", "openid", "creid", "outerid" });
        names.Should().Contain("sub_mch_id", "because 服务商侧必须带子商户号");
    }

    /// <summary>解约两支官方同路径 ⇒ 只能各用一套 DTO，靠路径共用不构成 DTO 共用。</summary>
    [Fact]
    public void V2_Papay_DeleteContract_ShouldShareRouteButNotDto()
    {
        RoutesOf<IWechatPayV2PapayService>().Should().Contain("/papay/deletecontract");
        RoutesOf<IWechatPayV2PartnerPapayService>().Should().Contain("/papay/deletecontract");

        var merchantRequest = typeof(IWechatPayV2PapayService).GetMethod("DeleteContractAsync")!
            .GetParameters()[0].ParameterType;
        var partnerRequest = typeof(IWechatPayV2PartnerPapayService).GetMethod("DeleteContractAsync")!
            .GetParameters()[0].ParameterType;

        merchantRequest.Should().Be(typeof(V2DeleteContractRequest));
        partnerRequest.Should().Be(typeof(V2PartnerDeleteContractRequest));
        merchantRequest.Should().NotBe(partnerRequest,
            "because 服务商侧字段表含 sub_mch_id，与商户侧共表会让「必填」在两族间互相污染");
    }

    /// <summary>PAP 双族 DTO 全部落 <c>PayV2</c> 序列化类 ⇒ 汇入同一个 <c>PayV2JsonContext</c>。</summary>
    [Fact]
    public void V2_Papay_Dtos_ShouldBeRegisteredInPayV2JsonContext()
    {
        var papayTypeNames = typeof(V2DeleteContractRequest).Assembly.GetTypes()
            .Where(static t => (t.Namespace ?? string.Empty).StartsWith("Mud.Wechat.Pay.V2", StringComparison.Ordinal))
            .Where(t => t.Namespace == "Mud.Wechat.Pay.V2.DataModels.PayV2")
            .Where(t => t.GetCustomAttributes(typeof(HttpJsonSerializableAttribute), inherit: true).Any())
            .Select(t => t.Name)
            .ToList();

        // 11 端点 ⇒ 20 型（两支 h5entrustweb 是 GET，只有应答型）。名单即契约：改名/漏登记都会打红。
        var expected = new[]
        {
            nameof(V2ContractOrderRequest), nameof(V2ContractOrderResponse),
            nameof(V2PapPayApplyRequest), nameof(V2PapPayApplyResponse),
            nameof(V2PreEntrustWebRequest), nameof(V2PreEntrustWebResponse),
            nameof(V2H5EntrustWebResponse),
            nameof(V2QueryContractRequest), nameof(V2QueryContractResponse),
            nameof(V2DeleteContractRequest), nameof(V2DeleteContractResponse),
            nameof(V2PartnerPapPayApplyRequest), nameof(V2PartnerPapPayApplyResponse),
            nameof(V2PartnerPreEntrustWebRequest), nameof(V2PartnerPreEntrustWebResponse),
            nameof(V2PartnerH5EntrustWebResponse),
            nameof(V2PartnerQueryContractRequest), nameof(V2PartnerQueryContractResponse),
            nameof(V2PartnerDeleteContractRequest), nameof(V2PartnerDeleteContractResponse),
        };

        foreach (var name in expected)
        {
            papayTypeNames.Should().Contain(name, $"because 委托代扣双族的 {name} 必须存在于 PayV2 命名空间");
        }

        var papayTypes = typeof(V2DeleteContractRequest).Assembly.GetTypes()
            .Where(static t => (t.Namespace ?? string.Empty).StartsWith("Mud.Wechat.Pay.V2", StringComparison.Ordinal))
            .Where(t => t.Namespace == "Mud.Wechat.Pay.V2.DataModels.PayV2")
            .Where(t => expected.Contains(t.Name, StringComparer.Ordinal))
            .ToList();

        papayTypes.Should().HaveCount(20, "because 11 端点的请求/应答型中，两支 GET 无请求 DTO ⇒ 恰好 20 型");

        var contextType = typeof(V2DeleteContractRequest).Assembly
            .GetType("Mud.Wechat.Pay.V2.DataModels.PayV2.PayV2JsonContext", throwOnError: false);
        contextType.Should().NotBeNull();

        var context = (global::System.Text.Json.Serialization.JsonSerializerContext)
            contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!.GetValue(null)!;

        foreach (var type in papayTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 未登记进 PayV2JsonContext ⇒ 重跑 scripts/GenerateJsonContext.ps1");
        }
    }

    /// <summary>11 端点全部不需要证书 ⇒ 不得有人把它们塞进 <c>IWechatPayV2SecureService</c>。</summary>
    [Fact]
    public void V2_Papay_Routes_ShouldNotLeakIntoSecureService()
    {
        var securePaths = typeof(IWechatPayV2SecureService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.Name)
            .ToList();

        securePaths.Should().NotContain(n => n.Contains("Contract", StringComparison.Ordinal)
                                             || n.Contains("Pap", StringComparison.Ordinal),
            "because 委托代扣官方逐页标注「需商户证书：否」，进证书通道即凭空多一条连接池与一套验签口径");
    }

    private static List<string> RoutesOf<TInterface>()
        where TInterface : class
        => typeof(TInterface)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Where(r => r != null)
            .Select(r => r!)
            .ToList();

    private static List<string> QueryNamesOf<TInterface>(string methodName)
        where TInterface : class
        => typeof(TInterface).GetMethod(methodName)!
            .GetParameters()
            .Select(p => p.GetCustomAttribute<QueryAttribute>()?.Name)
            .Where(n => n != null)
            .Select(n => n!)
            .ToList();
}

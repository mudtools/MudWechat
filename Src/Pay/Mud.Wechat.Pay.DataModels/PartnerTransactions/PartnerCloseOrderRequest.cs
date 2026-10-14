// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商关闭订单请求体（HTTP 204 无应答体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012739019"/>
/// （2026-10-14 核验；另有 5 个产品族分身页）。<b>POST /v3/pay/partner/transactions/out-trade-no/{out_trade_no}/close</b>。</para>
/// <para><b>本请求体只有两个字段</b>：<c>sp_mchid</c> 与 <c>sub_mchid</c> —— 与直连域 <c>CloseOrderRequest</c>
/// （只有 <c>mchid</c>）同形，即<b>关单不需要任何 AppID</b>。不得把下单用的 <c>sp_appid</c>/<c>sub_appid</c>
/// 顺手带进本请求体，官方字段表内无此字段（守卫 PT-B1 锁定 JSON 名集合）。</para>
/// <para><b>官方错误码表（逐条核验）</b>：<c>400 PARAM_ERROR</c>、<c>401 SIGN_ERROR</c>、
/// <c>500 SYSTEM_ERROR</c>、<c>400 MCH_NOT_EXISTS</c>、<c>403 RULE_LIMIT</c>、<c>403 TRADE_ERROR</c>、
/// <c>429 FREQUENCY_LIMITED</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerCloseOrderRequest
{
    /// <summary>服务商户号（<c>sp_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }
}

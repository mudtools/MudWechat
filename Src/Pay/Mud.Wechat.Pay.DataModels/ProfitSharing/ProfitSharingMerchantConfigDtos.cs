// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.ProfitSharing;

/// <summary>
/// 查询商户分账配置应答（<c>GET /v3/profitsharing/merchant-configs/{sub_mchid}</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012466864"/>
/// （2026-10-18 逐字段核验；支持商户：<b>普通服务商</b>）。</para>
/// <para><b>用途</b>：分账前查询子商户的<b>最大分账比例</b>（<c>max_ratio</c>，整型，
/// 官方口径为百分比数值）—— 分账请求的<b>分出方分账比例</b>不得超过该上限，超限会被业务拒绝。</para>
/// <para><b>本域唯一以子商户号走 path 的查询</b>：本端点的服务对象是<b>子商户</b>
/// （服务商代其发起分账），<c>sub_mchid</c> 来自服务商侧调用上下文，<b>不是</b>商户自身的 <c>mchid</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProfitSharing")]
public class ProfitSharingMerchantConfigResponse : WechatPayResponse
{
    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）：本次查询的子商户。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>最大分账比例（<c>max_ratio</c>，必填整型）：该子商户允许的最大分账比例（百分比数值）。</summary>
    [JsonPropertyName("max_ratio")]
    public int? MaxRatio { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 上架策略（官方 <c>audit_strategy</c> 对象：隐藏商品信息上架 / 可上架相似品 / 命中低风险规则可上架）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAuditStrategy
{
    /// <summary>获取或设置隐藏商品信息上架（官方 <c>hide_err_field_flag</c>：1 打开，2 关闭；set 时 0 不修改）。</summary>
    [JsonPropertyName("hide_err_field_flag")]
    public int? HideErrFieldFlag { get; set; }

    /// <summary>获取或设置可上架相似品（官方 <c>hit_duplicated_flag</c>：1 打开，2 关闭；set 时 0 不修改）。</summary>
    [JsonPropertyName("hit_duplicated_flag")]
    public int? HitDuplicatedFlag { get; set; }

    /// <summary>获取或设置命中低风险规则可上架（官方 <c>hit_low_risk_rule_flag</c>：1 打开，2 关闭；set 时 0 不修改）。</summary>
    [JsonPropertyName("hit_low_risk_rule_flag")]
    public int? HitLowRiskRuleFlag { get; set; }
}

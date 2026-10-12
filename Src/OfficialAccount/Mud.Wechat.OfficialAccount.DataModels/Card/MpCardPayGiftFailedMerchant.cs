// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 规则创建失败的商户项（官方 <c>fail_mchid_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardPayGiftFailedMerchant
{
    /// <summary>获取或设置商户号（官方 <c>mchid</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>获取或设置错误码（官方 <c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public int ErrorCode { get; set; }

    /// <summary>获取或设置错误信息（官方 <c>errmsg</c>，可选）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }

    /// <summary>获取或设置占用规则 ID（官方 <c>occupy_rule_id</c>，可选）。</summary>
    [JsonPropertyName("occupy_rule_id")]
    public int? OccupyRuleId { get; set; }

    /// <summary>获取或设置占用方 AppId（官方 <c>occupy_appid</c>，可选）。</summary>
    [JsonPropertyName("occupy_appid")]
    public string? OccupyAppId { get; set; }
}

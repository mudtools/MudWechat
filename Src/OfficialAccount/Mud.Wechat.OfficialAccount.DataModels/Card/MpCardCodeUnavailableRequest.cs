// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 「设置券码不可用」请求（官方 <c>code/unavailable</c>）。
/// </summary>
/// <remarks><b>覆盖删除语义</b>：置为失效后该券码不可再核销（官方约束：一般用于风险防控）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeUnavailableRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，自定义 code 的卡券必填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置失效原因（官方 <c>reason</c>，可选）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

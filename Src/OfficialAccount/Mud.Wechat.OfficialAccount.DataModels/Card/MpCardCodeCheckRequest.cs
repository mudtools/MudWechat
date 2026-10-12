// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 「查验卡券码」请求（官方 <c>code/checkcode</c>；B2c 落地券码运维族）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeCheckRequest
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置待查验的券码列表（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public List<string> CodeList { get; set; } = new();
}

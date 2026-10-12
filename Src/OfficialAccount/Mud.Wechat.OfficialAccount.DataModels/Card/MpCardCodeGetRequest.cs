// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 查询券码状态（<c>POST /card/code/get</c>）请求体。
/// </summary>
/// <remarks>
/// <para><c>check_consume</c> 为「是否同时校验可否核销」开关（选填），置真时应答携带 <c>can_consume</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeGetRequest
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，选填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置是否校验可核销性（官方 <c>check_consume</c>，选填）。</summary>
    [JsonPropertyName("check_consume")]
    public bool? CheckConsume { get; set; }
}

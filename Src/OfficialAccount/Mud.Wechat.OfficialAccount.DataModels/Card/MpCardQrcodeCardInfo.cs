// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>二维码单卡信息（官方 <c>action_info.card</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardQrcodeCardInfo
{
    /// <summary>获取或设置卡券模板编号（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置券码（官方 <c>code</c>，选填；礼品码场景使用）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置指定领取用户的 openid（官方 <c>openid</c>，选填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置是否唯一码（官方 <c>is_unique_code</c>，选填）。</summary>
    [JsonPropertyName("is_unique_code")]
    public bool? IsUniqueCode { get; set; }

    /// <summary>获取或设置外部自定义 id（官方 <c>outer_id</c>，选填）。</summary>
    [JsonPropertyName("outer_id")]
    public string? OuterId { get; set; }

    /// <summary>获取或设置外部自定义串（官方 <c>outer_str</c>，选填）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }
}

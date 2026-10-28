// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 礼品卡货架主题内的图片项（官方 <c>pic_item_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftThemePictureItem
{
    /// <summary>获取或设置背景图片 URL（官方 <c>background_pic_url</c>，必填）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string BackgroundPicUrl { get; set; } = string.Empty;

    /// <summary>获取或设置卡券 exterior 图片 ID（官方 <c>outer_img_id</c>，可选）。</summary>
    [JsonPropertyName("outer_img_id")]
    public string? OuterImgId { get; set; }

    /// <summary>获取或设置默认祝福语（官方 <c>default_gifting_msg</c>，可选）。</summary>
    [JsonPropertyName("default_gifting_msg")]
    public string? DefaultGiftingMsg { get; set; }
}

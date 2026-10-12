// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>商品定制信息（官方 <c>custom_info</c> 对象，<c>dropship/get</c> 与 <c>supplyorder/get</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsCustomInfo
{
    /// <summary>获取或设置定制图片（官方 <c>custom_img_url</c>，<c>custom_type = 2</c> 时返回）。</summary>
    [JsonPropertyName("custom_img_url")]
    public string? CustomImgUrl { get; set; }

    /// <summary>获取或设置定制文字（官方 <c>custom_word</c>，<c>custom_type = 1</c> 时返回）。</summary>
    [JsonPropertyName("custom_word")]
    public string? CustomWord { get; set; }

    /// <summary>获取或设置定制类型（官方 <c>custom_type</c>，见 <see cref="ChannelsCustomTypes"/>）。</summary>
    [JsonPropertyName("custom_type")]
    public int? CustomType { get; set; }

    /// <summary>获取或设置定制预览图片（官方 <c>custom_preview_img_url</c>，开启了定制预览时返回）。</summary>
    [JsonPropertyName("custom_preview_img_url")]
    public string? CustomPreviewImgUrl { get; set; }
}

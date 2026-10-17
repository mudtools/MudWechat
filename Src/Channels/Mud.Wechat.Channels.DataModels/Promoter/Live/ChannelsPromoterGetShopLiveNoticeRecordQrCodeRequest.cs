// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>为推客生成小店关联账号直播预约的推广二维码请求（官方 <c>promoter/get_shop_live_notice_record_qr_code</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetShopLiveNoticeRecordQrCodeRequest
{
    /// <summary>获取或设置小店的 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppId { get; set; }

    /// <summary>获取或设置关联账号的 id（官方 <c>promoter_id</c>，必填，从 <c>promoter/get_bind_shop_promoter_list</c> 获取）。</summary>
    [JsonPropertyName("promoter_id")]
    public string? PromoterId { get; set; }

    /// <summary>获取或设置直播预约 id（官方 <c>notice_id</c>，必填）。</summary>
    [JsonPropertyName("notice_id")]
    public string? NoticeId { get; set; }

    /// <summary>获取或设置推客的 appid（官方 <c>sharer_appid</c>，必填）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }

    /// <summary>获取或设置关联账号类型（官方 <c>promoter_type</c>，必填，需要是视频号）。</summary>
    [JsonPropertyName("promoter_type")]
    public int PromoterType { get; set; }
}

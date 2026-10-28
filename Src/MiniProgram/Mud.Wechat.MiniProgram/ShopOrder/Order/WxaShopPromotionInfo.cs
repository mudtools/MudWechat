// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 推广员与分享员信息（官方 <c>PromotionInfo</c> 表）。
/// </summary>
/// <remarks>
/// <para><b><c>promoter_openid</c> 官方标注「废弃，请勿使用」</b>：字段照录以兼容存量回包，
/// 归因请用 <c>promoter_id</c>；不得因「废弃」二字删键（会让存量报文丢字段）。</para>
/// <para><b>分享员与推广员是两个身份</b>：<c>sharer_openid</c> 走 <c>/shop/order/get_list_by_sharer</c>，
/// <c>promoter_id</c> / <c>promoter_openid</c> 走 <c>/shop/order/get_list_by_finder</c>，两支的入参集不互通。</para>
/// <para><b>敏感值</b>：<c>sharer_openid</c> / <c>promoter_openid</c> 为用户标识，不得写入日志、遥测或异常消息。</para>
/// <para><c>live_start_time</c> 仅两支分享员/推广员列表的示例出现（Order 表未列），官方原文示例给 <c>yyyy-MM-dd HH:mm:ss</c>。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPromotionInfo
{
    /// <summary>promoter_id（官方类型 <c>string</c>）：推广员唯一 ID。</summary>
    [JsonPropertyName("promoter_id")]
    public string? PromoterId { get; set; }

    /// <summary>promoter_openid（官方类型 <c>string</c>）：推广员 openid，官方标注「废弃，请勿使用」（敏感标识，不得进日志）。</summary>
    [JsonPropertyName("promoter_openid")]
    public string? PromoterOpenId { get; set; }

    /// <summary>finder_nickname（官方类型 <c>string</c>）：推广员视频号昵称。</summary>
    [JsonPropertyName("finder_nickname")]
    public string? FinderNickname { get; set; }

    /// <summary>sharer_openid（官方类型 <c>string</c>）：分享员 openid（敏感标识，不得进日志）。</summary>
    [JsonPropertyName("sharer_openid")]
    public string? SharerOpenId { get; set; }

    /// <summary>live_start_time（官方示例键，类型 <c>string</c>）：直播间开始时间。</summary>
    [JsonPropertyName("live_start_time")]
    public string? LiveStartTime { get; set; }
}

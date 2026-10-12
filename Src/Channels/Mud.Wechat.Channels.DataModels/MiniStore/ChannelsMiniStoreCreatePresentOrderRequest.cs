// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>创建并发送礼物（<c>presentorder/create</c>）请求体。</summary>
/// <remarks>祝福语（wishmessage）最大 100 字符；idempotent_id 填写时相同幂等 id 的并发请求返回 6011104。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreCreatePresentOrderRequest
{
    /// <summary>获取或设置活动 id（官方 <c>activity_id</c>，必填）。</summary>
    [JsonPropertyName("activity_id")]
    public long ActivityId { get; set; }

    /// <summary>获取或设置收礼者用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置合作小店 appid（官方 <c>shop_appid</c>，必填）。</summary>
    [JsonPropertyName("shop_appid")]
    public string ShopAppId { get; set; } = string.Empty;

    /// <summary>获取或设置礼物祝福语信息（官方 <c>wishmessage</c>，必填，最大 100 字符）。</summary>
    [JsonPropertyName("wishmessage")]
    public string WishMessage { get; set; } = string.Empty;

    /// <summary>获取或设置指定活动商品（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置指定活动 sku（官方 <c>sku_id</c>，必填）。</summary>
    [JsonPropertyName("sku_id")]
    public long SkuId { get; set; }

    /// <summary>获取或设置幂等 id（官方 <c>idempotent_id</c>，选填）。</summary>
    [JsonPropertyName("idempotent_id")]
    public string? IdempotentId { get; set; }
}

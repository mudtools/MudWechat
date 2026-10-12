// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调载荷 DTO —— 商品族（商品审核 / 上下架 / 更新，嵌套对象键为官方 PascalCase 原文）。
// 事件键：product_spu_audit、product_spu_listing、product_spu_update。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Callback。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>商品上下架事件主体（官方 <c>ProductSpuListing</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public sealed class ChannelsProductSpuListingInfo
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置状态（官方 <c>status</c>）：5 上架；11 自主下架；13 系统下架；14 保证金违规下架；15 品牌到期下架；20 封禁下架。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>获取或设置下架原因，非必填字段（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置商品类型（官方 <c>product_type</c>）：1 小店自营商品；4 在售赠品；5 非卖商品。</summary>
    [JsonPropertyName("product_type")]
    public long? ProductType { get; set; }
}

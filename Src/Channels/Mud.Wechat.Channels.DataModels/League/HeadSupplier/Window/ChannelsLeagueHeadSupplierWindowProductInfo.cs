// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>橱窗商品内容（官方 <c>product_detail.product_info</c>，<c>headsupplier/window/getdetail</c>）。
/// 本接口是橱窗视角的最小商品快照（标题 + 主图 + 类目 + 推广链接），与机构商品详情的
/// <c>product_info</c>（sku / 物流 / 服务保障全量）字段集差异巨大 ⇒ 分建；<c>desc_info</c> 与 <c>cats</c> / <c>cats_v2</c> 同形 ⇒ 复用既有类型。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierWindowProductInfo
{
    /// <summary>获取或设置商品标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置商品副标题（官方 <c>sub_title</c>，添加时未录入则回包可能不含该字段）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>获取或设置商品主图（官方 <c>head_imgs</c>，最多 9 张、每张不超过 2MB）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string>? HeadImgs { get; set; }

    /// <summary>获取或设置商品详情信息（官方 <c>desc_info</c>，与 Product 域同形 ⇒ 复用）。</summary>
    [JsonPropertyName("desc_info")]
    public Mud.Wechat.Channels.DataModels.Product.ChannelsProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置旧类目树（官方 <c>cats</c>，元素仅 <c>cat_id</c>，与 Product 域同形 ⇒ 复用）。</summary>
    [JsonPropertyName("cats")]
    public List<Mud.Wechat.Channels.DataModels.Product.ChannelsProductCategory>? Cats { get; set; }

    /// <summary>获取或设置新多级类目树（官方 <c>cats_v2</c>，元素仅 <c>cat_id</c> ⇒ 与 <c>cats</c> 同一类型）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<Mud.Wechat.Channels.DataModels.Product.ChannelsProductCategory>? CatsV2 { get; set; }

    /// <summary>获取或设置商品推广链接（官方 <c>product_promotion_link</c>，用于小程序跳转小店场景添加商品时传递跟佣信息）。</summary>
    [JsonPropertyName("product_promotion_link")]
    public string? ProductPromotionLink { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopSpu;

/// <summary>更新商品请求（<c>POST /shop/spu/update</c>；<c>update_without_audit</c> 同形）。</summary>
[HttpJsonSerializable(SerializerClassName = "ShopSpu")]
public class WxaShopSpuUpdateRequest
{
    /// <summary>交易组件平台内部商品 ID。官方参数表与 <c>out_product_id</c> 均标「是」且互斥 ⇒ 实际操作按二选一提供。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>商家自定义商品 ID。官方参数表与 <c>product_id</c> 均标「是」且互斥 ⇒ 实际操作按二选一提供。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>标题（必填），3~60 字符；不得仅为数字、字母或特殊字符组合。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>绑定的小程序商品路径（必填）。</summary>
    [JsonPropertyName("path")]
    public string? Path { get; set; }

    /// <summary>商品立即购买链接。</summary>
    [JsonPropertyName("direct_path")]
    public string? DirectPath { get; set; }

    /// <summary>主图（必填），最多 9 张。</summary>
    [JsonPropertyName("head_img")]
    public List<string>? HeadImg { get; set; }

    /// <summary>商品资质图片，最多 5 张。</summary>
    [JsonPropertyName("qualification_pics")]
    public List<string>? QualificationPics { get; set; }

    /// <summary>商品详情图文信息。</summary>
    [JsonPropertyName("desc_info")]
    public WxaShopSpuDescInfo? DescInfo { get; set; }

    /// <summary>第三级类目 ID（必填）。</summary>
    [JsonPropertyName("third_cat_id")]
    public long? ThirdCatId { get; set; }

    /// <summary>品牌 ID（必填）。</summary>
    [JsonPropertyName("brand_id")]
    public long? BrandId { get; set; }

    /// <summary>预留字段，用于版本控制。</summary>
    [JsonPropertyName("info_version")]
    public string? InfoVersion { get; set; }

    /// <summary>SKU 数组（必填，整体覆盖）。</summary>
    [JsonPropertyName("skus")]
    public List<WxaShopSpuWriteSku>? Skus { get; set; }

    /// <summary>商品使用场景（必填）：1 视频号；3 订单中心。</summary>
    [JsonPropertyName("scene_group_list")]
    public List<int>? SceneGroupList { get; set; }
}

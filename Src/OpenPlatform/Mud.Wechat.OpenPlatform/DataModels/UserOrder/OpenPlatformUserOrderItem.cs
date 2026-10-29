// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.UserOrder;

/// <summary>
/// 购物商品项（官方 <c>item_list</c> 元素；上传购物详情与上传物流信息两族共用）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShoppingInfo.html"/></para>
/// <para>上传物流信息族的商品项官方仅要求 <c>merchant_item_id</c>；其余字段留空即不序列化
/// （上下文配置 <c>DefaultIgnoreCondition = WhenWritingNull</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "UserOrder")]
public class OpenPlatformUserOrderItem
{
    /// <summary>商户侧商品 ID（官方必填 <c>merchant_item_id</c>）。</summary>
    [JsonPropertyName("merchant_item_id")]
    public string? MerchantItemId { get; set; }

    /// <summary>商品名称（官方 <c>name</c>；上传购物详情时必填）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>商品描述（官方选填 <c>description</c>）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>商品单价（官方 <c>unit_price</c>，单位：分；上传购物详情时必填）。</summary>
    [JsonPropertyName("unit_price")]
    public int? UnitPrice { get; set; }

    /// <summary>购买数量（官方 <c>quantity</c>；上传购物详情时必填）。</summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    /// <summary>商品图片 URL 列表（官方选填 <c>image_url</c>）。</summary>
    [JsonPropertyName("image_url")]
    public List<string>? ImageUrl { get; set; }

    /// <summary>商户交易商品详情页链接（官方选填 <c>item_detail_jump_link</c>）。</summary>
    [JsonPropertyName("item_detail_jump_link")]
    public OpenPlatformUserOrderJumpLink? ItemDetailJumpLink { get; set; }
}

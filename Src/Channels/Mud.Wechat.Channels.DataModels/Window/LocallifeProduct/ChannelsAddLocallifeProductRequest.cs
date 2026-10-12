// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>
/// 添加团购优惠（<c>addlocalproduct</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>product_type</c> 与 <c>code_source_type</c> 创建后不支持修改；本接口不支持直接设置库存，
/// 创建完成后须调用库存管理接口。<c>attr_kv_map</c> 为字符串键值 map，值为 JSON 字符串化属性，
/// 键集参照官方「代金券属性KV / 兑换券属性KV」小节（如 use_date_rule、use_time、detail_imgs、set_meals、
/// exclusive_date、voucher_amount、bring_out_type、appointment、need_phone_num、additional_rule、
/// available_store_desc、settle_type、refund_policy）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsAddLocallifeProductRequest
{
    /// <summary>获取或设置外部系统 ID（官方 <c>out_product_id</c>，选填；仅作存储字段，商家需自行维护与平台 product_id 的对应关系）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置券类型（官方 <c>product_type</c>，必填，见 <see cref="ChannelsLocallifeProductTypes"/>；创建后不支持修改）。</summary>
    [JsonPropertyName("product_type")]
    public int ProductType { get; set; }

    /// <summary>获取或设置名称（官方 <c>product_name</c>，必填；3–60 字符，中/英文/数字各算 1 字符，不得仅为数字或英文，不得含非法字符）。</summary>
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>获取或设置三级类目 ID（官方 <c>category_id</c>，string(uint64)，必填；经「获取所有的类目」接口获取）。</summary>
    [JsonPropertyName("category_id")]
    public string CategoryId { get; set; } = string.Empty;

    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>，string(uint64)，必填；经「获取生效中的品牌资质列表」接口获取）。</summary>
    [JsonPropertyName("brand_id")]
    public string BrandId { get; set; } = string.Empty;

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>，必填；须通过「上传图片」接口获取）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>，选填；参考类目信息中的 product_qua_list）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsLocallifeProductQuaInfo>? ProductQuaInfos { get; set; }

    /// <summary>获取或设置核销跳转出口（官方 <c>verify_page</c>，选填；verify_at_store=0 时必填，不满足体验规范的商家将被断播处罚）。</summary>
    [JsonPropertyName("verify_page")]
    public ChannelsLocallifeWxAppPath? VerifyPage { get; set; }

    /// <summary>获取或设置是否支持到店核销（官方 <c>verify_at_store</c>，选填；1=支持（默认），0=不支持。填 0 时订单页不展示核销二维码，且必须填 verify_page）。</summary>
    [JsonPropertyName("verify_at_store")]
    public int? VerifyAtStore { get; set; }

    /// <summary>获取或设置券码来源（官方 <c>code_source_type</c>，必填，见 <see cref="ChannelsLocallifeCodeSourceTypes"/>；创建后不支持修改）。</summary>
    [JsonPropertyName("code_source_type")]
    public int CodeSourceType { get; set; }

    /// <summary>获取或设置商品属性 KV（官方 <c>attr_kv_map</c>，必填；字符串键值 map，值为 JSON 字符串化属性）。</summary>
    [JsonPropertyName("attr_kv_map")]
    public Dictionary<string, string> AttrKvMap { get; set; } = new();

    /// <summary>获取或设置售卖单元列表（官方 <c>skus</c>，必填；本接口不支持直接设库存，需另调库存接口）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsLocallifeSkuRequest> Skus { get; set; } = new();
}

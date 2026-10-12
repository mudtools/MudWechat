// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>
/// 团购优惠商品数据（官方 <c>online_data</c>/<c>draft_data</c> 对象，<c>getlocalproduct</c>——官方文档明示两者结构相同 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾照录</b>：响应参数表把 <c>product_id</c>/<c>category_id</c>/<c>brand_id</c> 标注为 number、
/// 返回示例为字符串（如 "15000000134007"）——SDK 按返回示例与同域 add/update 请求参数表 string(uint64) 建模为字符串。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeProductData
{
    /// <summary>获取或设置平台生成的商品 ID（官方 <c>product_id</c>；官方参数表 number、返回示例为字符串——矛盾照录，建模为字符串）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置外部系统 ID（官方 <c>out_product_id</c>；只作为商品字段存储，不用做索引，商家需维护其与 product_id 的对应关系）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>获取或设置券类型（官方 <c>product_type</c>，见 <see cref="ChannelsLocallifeProductTypes"/>）。</summary>
    [JsonPropertyName("product_type")]
    public int? ProductType { get; set; }

    /// <summary>获取或设置名称（官方 <c>product_name</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>获取或设置三级类目 ID（官方 <c>category_id</c>；官方参数表 number、返回示例为字符串——矛盾照录，建模为字符串）。</summary>
    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    /// <summary>获取或设置品牌 ID（官方 <c>brand_id</c>；官方参数表 number、返回示例为字符串——矛盾照录，建模为字符串）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置主图列表（官方 <c>head_imgs</c>）。</summary>
    [JsonPropertyName("head_imgs")]
    public List<string>? HeadImgs { get; set; }

    /// <summary>获取或设置核销跳转出口（官方 <c>verify_page</c>）。</summary>
    [JsonPropertyName("verify_page")]
    public ChannelsLocallifeWxAppPath? VerifyPage { get; set; }

    /// <summary>获取或设置券码来源（官方 <c>code_source_type</c>，见 <see cref="ChannelsLocallifeCodeSourceTypes"/>）。</summary>
    [JsonPropertyName("code_source_type")]
    public int? CodeSourceType { get; set; }

    /// <summary>
    /// 获取或设置商品属性 KV（官方 <c>attr_kv_map</c>）。官方响应参数表列出已知键：<c>use_date_rule</c> 有效期规则、
    /// <c>total_price</c> 兑换券门市价（分）、<c>available_store_desc</c> 可用门店说明（≤100 字）、<c>use_time</c> 可用时段、
    /// <c>exclusive_date</c> 不可使用日期、<c>voucher_amount</c> 代金券面额、<c>bring_out_type</c> 堂食外带、
    /// <c>appointment</c> 是否需要预约、<c>limit_buy_rule</c> 单用户购买限制、<c>order_limit_num</c> 单笔订单限购数量（0–25）、
    /// <c>additional_rule</c> 其他文本规则（≤1000 字）、<c>need_phone_num</c> 是否需要用户手机号、<c>settle_type</c> 结算对象、
    /// <c>refund_policy</c> 退款策略、<c>detail_imgs</c> 详情图片（≤10 张）、<c>set_meals</c> 套餐内容（≤10 个子套餐分组）；
    /// 值为 JSON 字符串化属性。
    /// </summary>
    [JsonPropertyName("attr_kv_map")]
    public Dictionary<string, string>? AttrKvMap { get; set; }

    /// <summary>获取或设置售卖单元列表（官方 <c>skus</c>；仅返回示例列出 sku_id/sale_price/stock_num 三字段）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsLocallifeSku>? Skus { get; set; }

    /// <summary>获取或设置商品草稿状态（官方 <c>draft_status</c>，见 <see cref="ChannelsLocallifeDraftStatuses"/>）。</summary>
    [JsonPropertyName("draft_status")]
    public int? DraftStatus { get; set; }

    /// <summary>获取或设置商品线上状态（官方 <c>online_status</c>，见 <see cref="ChannelsLocallifeOnlineStatuses"/>）。</summary>
    [JsonPropertyName("online_status")]
    public int? OnlineStatus { get; set; }

    /// <summary>获取或设置商品审核结果（官方 <c>audit_result</c>）。</summary>
    [JsonPropertyName("audit_result")]
    public ChannelsLocallifeAuditResult? AuditResult { get; set; }

    /// <summary>获取或设置是否支持到店核销（官方 <c>verify_at_store</c>；1=支持，0=不支持。选 0 时订单页不展示核销二维码且必须填 verify_page）。</summary>
    [JsonPropertyName("verify_at_store")]
    public int? VerifyAtStore { get; set; }

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsLocallifeProductQuaInfo>? ProductQuaInfos { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 添加商品（<c>product/add</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>POST</b> + 请求体；草稿 / 线上双份数据语义——添加后只影响草稿，
/// 需调上架接口并审核通过后草稿才覆盖线上数据正式生效；sku 数量超过 25 个时接口会异步更新。
/// </para>
/// <para>
/// 图片相关参数（head_img / desc_info.imgs / product_qua_infos[].qua_url[] / skus[].thumb_img 等）
/// 请务必使用「上传图片」接口（resp_type=1）并回填返回的 img_url（前缀 mmecimage.cn/p/），
/// 不接受其他任何格式的图片 url。
/// </para>
/// <para>
/// <c>cats_v2</c> 新多级类目树与 <c>cats</c> 旧三级类目树兼容：设置了 <c>cats_v2</c> 优先读取 <c>cats_v2</c>。
/// </para>
/// <para>
/// 未建模的官方可选对象：<c>size_chart</c>（尺码表）、<c>supply_source</c>（供货货品来源）——
/// 官方参数表未列出其子字段，SDK 不发明字段；如需使用请按官方全量字段自定义提交。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsAddProductRequest
{
    /// <summary>获取或设置商家自定义商品 ID（官方 <c>out_product_id</c>，最多 128 字符；添加成功后无法修改）。</summary>
    [JsonPropertyName("out_product_id")]
    public string? OutProductId { get; set; }

    /// <summary>
    /// 获取或设置标题（官方 <c>title</c>，必填：至少 1 个有效字符、最多 60 字符；
    /// 不得仅为数字或英文，允许的特殊字符集见官方文档）。
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置副标题（官方 <c>sub_title</c>，最多 18 字符；该字段已废弃）。</summary>
    [JsonPropertyName("sub_title")]
    public string? SubTitle { get; set; }

    /// <summary>获取或设置商品短标题（官方 <c>short_title</c>，最多 20 字符；用于礼物单等场景，缺省显示 title）。</summary>
    [JsonPropertyName("short_title")]
    public string? ShortTitle { get; set; }

    /// <summary>
    /// 获取或设置主图列表（官方 <c>head_imgs</c>，必填：最少 3 张（食品饮料和生鲜类目最少 4 张）、最多 9 张，不得重复）。
    /// </summary>
    [JsonPropertyName("head_imgs")]
    public List<string> HeadImgs { get; set; } = new();

    /// <summary>获取或设置发货方式（官方 <c>deliver_method</c>，必填：0 快递发货，1 无需快递-手机号发货，3 无需快递-可选发货账号类型）。</summary>
    [JsonPropertyName("deliver_method")]
    public int DeliverMethod { get; set; }

    /// <summary>获取或设置发货账号类型（官方 <c>deliver_acct_type</c>：1 微信 openid，2 QQ 号，3 手机号，4 邮箱；仅 <c>deliver_method=3</c> 时有意义）。</summary>
    [JsonPropertyName("deliver_acct_type")]
    public List<int>? DeliverAcctType { get; set; }

    /// <summary>获取或设置商品详情（官方 <c>desc_info</c>：<c>imgs</c> + <c>desc</c>）。</summary>
    [JsonPropertyName("desc_info")]
    public ChannelsProductDescInfo? DescInfo { get; set; }

    /// <summary>获取或设置商品类目（旧类目树，官方 <c>cats</c>，大小恒等于 3；商品上架后不可修改一级类目）。</summary>
    [JsonPropertyName("cats")]
    public List<ChannelsProductCategory>? Cats { get; set; }

    /// <summary>获取或设置商品类目（新类目树，官方 <c>cats_v2</c>；存在时优先于 <c>cats</c>）。</summary>
    [JsonPropertyName("cats_v2")]
    public List<ChannelsProductCategory>? CatsV2 { get; set; }

    /// <summary>获取或设置商品参数（官方 <c>attrs</c>；部分类目有必填参数）。</summary>
    [JsonPropertyName("attrs")]
    public List<ChannelsProductAttr>? Attrs { get; set; }

    /// <summary>获取或设置商家编码（官方 <c>spu_code</c>）。</summary>
    [JsonPropertyName("spu_code")]
    public string? SpuCode { get; set; }

    /// <summary>获取或设置品牌 id（官方 <c>brand_id</c>，无品牌为 <c>"2100000000"</c>）。</summary>
    [JsonPropertyName("brand_id")]
    public string? BrandId { get; set; }

    /// <summary>获取或设置商品资质列表（官方 <c>product_qua_infos</c>，取代已废弃的 <c>qualifications</c>）。</summary>
    [JsonPropertyName("product_qua_infos")]
    public List<ChannelsProductQuaInfo>? ProductQuaInfos { get; set; }

    /// <summary>获取或设置运费信息（官方 <c>express_info</c>）。</summary>
    [JsonPropertyName("express_info")]
    public ChannelsProductExpressInfo? ExpressInfo { get; set; }

    /// <summary>获取或设置售后说明（官方 <c>aftersale_desc</c>，最多 200 UTF 字符）。</summary>
    [JsonPropertyName("aftersale_desc")]
    public string? AftersaleDesc { get; set; }

    /// <summary>获取或设置限购信息（官方 <c>limited_info</c>）。</summary>
    [JsonPropertyName("limited_info")]
    public ChannelsProductLimitedInfo? LimitedInfo { get; set; }

    /// <summary>获取或设置额外服务（官方 <c>extra_service</c>，必填；不符合平台规则会被平台优化至合规值）。</summary>
    [JsonPropertyName("extra_service")]
    public ChannelsProductExtraService? ExtraService { get; set; }

    /// <summary>获取或设置商品 SKU 列表（官方 <c>skus</c>，必填：长度最少 1、最大 500）。</summary>
    [JsonPropertyName("skus")]
    public List<ChannelsProductSku> Skus { get; set; } = new();

    /// <summary>获取或设置添加完成后是否立即上架（官方 <c>listing</c>：1 是，0 否，默认 0）。</summary>
    [JsonPropertyName("listing")]
    public int? Listing { get; set; }

    /// <summary>获取或设置售后 / 退货地址（官方 <c>after_sale_info</c>）。</summary>
    [JsonPropertyName("after_sale_info")]
    public ChannelsProductAfterSaleInfo? AfterSaleInfo { get; set; }

    /// <summary>获取或设置是否在店铺首页隐藏（官方 <c>hide_in_window</c>：0 不隐藏，1 隐藏）。</summary>
    [JsonPropertyName("hide_in_window")]
    public int? HideInWindow { get; set; }

    /// <summary>获取或设置发布模式（官方 <c>release_mode</c>：0 普通模式，1 极简模式）。</summary>
    [JsonPropertyName("release_mode")]
    public int? ReleaseMode { get; set; }

    /// <summary>获取或设置商品待开售信息（官方 <c>timing_onsale_info</c>）。</summary>
    [JsonPropertyName("timing_onsale_info")]
    public ChannelsTimingOnsaleInfo? TimingOnsaleInfo { get; set; }

    /// <summary>获取或设置商品头图视频（官方 <c>head_videos</c>，最多 1 个）。</summary>
    [JsonPropertyName("head_videos")]
    public ChannelsProductHeadVideo? HeadVideos { get; set; }

    /// <summary>获取或设置 spu 维度配置全部 sku 预售规则（官方 <c>spu_deliver_info</c>）。</summary>
    [JsonPropertyName("spu_deliver_info")]
    public ChannelsSpuDeliverInfo? SpuDeliverInfo { get; set; }

    /// <summary>获取或设置 3:4 主图列表（官方 <c>vertical_head_imgs</c>，最多 1 张，尺寸比例 3:4）。</summary>
    [JsonPropertyName("vertical_head_imgs")]
    public List<string>? VerticalHeadImgs { get; set; }

    /// <summary>获取或设置白底图列表（官方 <c>white_bg_imgs</c>，最多 1 张，尺寸比例 1:1）。</summary>
    [JsonPropertyName("white_bg_imgs")]
    public List<string>? WhiteBgImgs { get; set; }
}

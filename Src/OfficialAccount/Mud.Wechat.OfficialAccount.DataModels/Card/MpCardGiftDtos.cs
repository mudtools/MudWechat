// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 「设置礼品卡货架维护」请求（官方 <c>giftcard/maintain/set</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftMaintainSetRequest
{
    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>，可选；<see cref="All"/> 为 false 时必填）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置是否对全部货架生效（官方 <c>all</c>，可选）。</summary>
    [JsonPropertyName("all")]
    public bool? All { get; set; }

    /// <summary>获取或设置是否进入维护状态（官方 <c>maintain</c>，必填）。</summary>
    [JsonPropertyName("maintain")]
    public bool Maintain { get; set; }
}

/// <summary>
/// 维护状态下的货架项（官方 <c>list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftMaintainPage
{
    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>；官方返回数字，字符串承载）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置货架维护状态（官方 <c>page_control_type</c>）。</summary>
    [JsonPropertyName("page_control_type")]
    public string? PageControlType { get; set; }

    /// <summary>获取或设置系统侧货架维护状态（官方 <c>system_page_control_type</c>）。</summary>
    [JsonPropertyName("system_page_control_type")]
    public string? SystemPageControlType { get; set; }
}

/// <summary>
/// 维护状态控制信息（官方 <c>control_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftMaintainControl
{
    /// <summary>获取或设置商户侧维护类型（官方 <c>biz_control_type</c>）。</summary>
    [JsonPropertyName("biz_control_type")]
    public string? BizControlType { get; set; }

    /// <summary>获取或设置系统侧维护类型（官方 <c>system_biz_control_type</c>）。</summary>
    [JsonPropertyName("system_biz_control_type")]
    public string? SystemBizControlType { get; set; }

    /// <summary>获取或设置维护中的货架列表（官方 <c>list</c>）。</summary>
    [JsonPropertyName("list")]
    public MpCardGiftMaintainPage[]? PageList { get; set; }
}

/// <summary>
/// 「设置礼品卡货架维护」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftMaintainSetResponse : MpResponse
{
    /// <summary>获取或设置维护状态控制信息（官方 <c>control_info</c>）。</summary>
    [JsonPropertyName("control_info")]
    public MpCardGiftMaintainControl? ControlInfo { get; set; }
}

/// <summary>
/// 「拉取礼品卡订单列表」请求（官方 <c>giftcard/order/batchget</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrderBatchGetRequest
{
    /// <summary>获取或设置查询起始时间（官方 <c>begin_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置查询结束时间（官方 <c>end_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置排序方式（官方 <c>sort_type</c>；ASC / DESC）。</summary>
    [JsonPropertyName("sort_type")]
    public string SortType { get; set; } = "ASC";

    /// <summary>获取或设置起始偏移（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置拉取数量（官方 <c>count</c>，必填；官方默认 10）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; } = 10;
}

/// <summary>
/// 礼品卡订单项（官方 <c>order</c>；order/get 与 order/batchget 共用形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrder
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置微信支付订单号（官方 <c>trans_id</c>）。</summary>
    [JsonPropertyName("trans_id")]
    public string? TransactionId { get; set; }

    /// <summary>获取或设置购买者 OpenId（官方 <c>open_id</c>）。</summary>
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置收礼方 OpenId（官方 <c>accepter_openid</c>，可选）。</summary>
    [JsonPropertyName("accepter_openid")]
    public string? AccepterOpenId { get; set; }

    /// <summary>获取或设置订单总金额（官方 <c>total_price</c>，单位分）。</summary>
    [JsonPropertyName("total_price")]
    public int TotalPrice { get; set; }

    /// <summary>获取或设置是否聊天室订单（官方 <c>IsChatRoom</c>；<b>官方 JSON 键首字母大写为原始笔误形态，照抄</b>）。</summary>
    [JsonPropertyName("IsChatRoom")]
    public bool? IsChatRoom { get; set; }

    /// <summary>获取或设置订单内卡券列表（官方 <c>card_list</c>）。</summary>
    [JsonPropertyName("card_list")]
    public MpCardGiftOrderCard[]? CardList { get; set; }

    /// <summary>获取或设置支付完成时间（官方 <c>pay_finish_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("pay_finish_time")]
    public long? PayFinishTime { get; set; }

    /// <summary>获取或设置下单时间（官方 <c>create_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>获取或设置生成订单时的自定义参数（官方 <c>outer_str</c>，可选）。</summary>
    [JsonPropertyName("outer_str")]
    public string? OuterStr { get; set; }
}

/// <summary>
/// 礼品卡订单内的卡券项（官方 <c>card_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrderCard
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置券码（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置卡券单价（官方 <c>price</c>，单位分）。</summary>
    [JsonPropertyName("price")]
    public int Price { get; set; }

    /// <summary>获取或设置卡券封面（官方 <c>background_pic_url</c>）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string? BackgroundPicUrl { get; set; }

    /// <summary>获取或设置卡券 exterior 图片 ID（官方 <c>outer_img_id</c>，可选）。</summary>
    [JsonPropertyName("outer_img_id")]
    public string? OuterImgId { get; set; }

    /// <summary>获取或设置默认祝福语（官方 <c>default_gifting_msg</c>，可选）。</summary>
    [JsonPropertyName("default_gifting_msg")]
    public string? DefaultGiftingMsg { get; set; }

    /// <summary>获取或设置收礼方 OpenId（官方 <c>accepter_openid</c>，可选）。</summary>
    [JsonPropertyName("accepter_openid")]
    public string? AccepterOpenId { get; set; }
}

/// <summary>
/// 「拉取礼品卡订单列表」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrderBatchGetResponse : MpResponse
{
    /// <summary>获取或设置订单列表（官方 <c>order_list</c>）。</summary>
    [JsonPropertyName("order_list")]
    public MpCardGiftOrder[]? OrderList { get; set; }

    /// <summary>获取或设置订单总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }
}

/// <summary>
/// 「查询礼品卡订单」请求（官方 <c>giftcard/order/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrderGetRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>
/// 「查询礼品卡订单」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrderGetResponse : MpResponse
{
    /// <summary>获取或设置订单详情（官方 <c>order</c>）。</summary>
    [JsonPropertyName("order")]
    public MpCardGiftOrder? Order { get; set; }
}

/// <summary>
/// 「礼品卡订单退款」请求（官方 <c>giftcard/order/refund</c>）。
/// </summary>
/// <remarks><b>7 天有效期（官方约束）</b>：下单超过 7 天的订单不可退款；退款为资金逆向操作，不可逆。</remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrderRefundRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>
/// 礼品卡货架主题内的卡券项（官方 <c>item_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftThemeCardItem
{
    /// <summary>获取或设置卡券 ID（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置卡券标题（官方 <c>title</c>，可选）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置卡券图片（官方 <c>pic_url</c>，可选）。</summary>
    [JsonPropertyName("pic_url")]
    public string? PicUrl { get; set; }

    /// <summary>获取或设置卡券描述（官方 <c>desc</c>，可选）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }
}

/// <summary>
/// 礼品卡货架主题内的图片项（官方 <c>pic_item_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftThemePictureItem
{
    /// <summary>获取或设置背景图片 URL（官方 <c>background_pic_url</c>，必填）。</summary>
    [JsonPropertyName("background_pic_url")]
    public string BackgroundPicUrl { get; set; } = string.Empty;

    /// <summary>获取或设置卡券 exterior 图片 ID（官方 <c>outer_img_id</c>，可选）。</summary>
    [JsonPropertyName("outer_img_id")]
    public string? OuterImgId { get; set; }

    /// <summary>获取或设置默认祝福语（官方 <c>default_gifting_msg</c>，可选）。</summary>
    [JsonPropertyName("default_gifting_msg")]
    public string? DefaultGiftingMsg { get; set; }
}

/// <summary>
/// 礼品卡货架主题（官方 <c>theme_list</c> 项；9 字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftTheme
{
    /// <summary>获取或设置主题图片 URL（官方 <c>theme_pic_url</c>，必填）。</summary>
    [JsonPropertyName("theme_pic_url")]
    public string ThemePicUrl { get; set; } = string.Empty;

    /// <summary>获取或设置主题标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置标题颜色（官方 <c>title_color</c>，必填）。</summary>
    [JsonPropertyName("title_color")]
    public string TitleColor { get; set; } = string.Empty;

    /// <summary>获取或设置卡券列表（官方 <c>item_list</c>，必填）。</summary>
    [JsonPropertyName("item_list")]
    public List<MpCardGiftThemeCardItem> ItemList { get; set; } = new();

    /// <summary>获取或设置图片列表（官方 <c>pic_item_list</c>，必填）。</summary>
    [JsonPropertyName("pic_item_list")]
    public List<MpCardGiftThemePictureItem> PictureItemList { get; set; } = new();

    /// <summary>获取或设置分类索引（官方 <c>category_index</c>，必填）。</summary>
    [JsonPropertyName("category_index")]
    public int CategoryIndex { get; set; }

    /// <summary>获取或设置是否优先展示规格标题（官方 <c>show_sku_title_first</c>，可选）。</summary>
    [JsonPropertyName("show_sku_title_first")]
    public bool? ShowSkuTitleFirst { get; set; }

    /// <summary>获取或设置是否为 Banner 主题（官方 <c>is_banner</c>，可选）。</summary>
    [JsonPropertyName("is_banner")]
    public bool? IsBanner { get; set; }
}

/// <summary>
/// 礼品卡货架分类（官方 <c>category_list</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftCategory
{
    /// <summary>获取或设置分类标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}

/// <summary>
/// 礼品卡货架自定义单元格（官方 <c>cell_1</c> / <c>cell_2</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftCustomCell
{
    /// <summary>获取或设置单元格标题（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置跳转链接（官方 <c>url</c>，必填）。</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// 礼品卡货架（官方 <c>page</c>；add 形态 12 字段，update 追加 page_id）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPage
{
    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>；仅 update 形态使用）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }

    /// <summary>获取或设置货架 Banner 图（官方 <c>banner_pic_url</c>，必填）。</summary>
    [JsonPropertyName("banner_pic_url")]
    public string BannerPicUrl { get; set; } = string.Empty;

    /// <summary>获取或设置货架标题（官方 <c>page_title</c>，必填）。</summary>
    [JsonPropertyName("page_title")]
    public string PageTitle { get; set; } = string.Empty;

    /// <summary>获取或设置是否支持多主题（官方 <c>support_multi</c>，可选）。</summary>
    [JsonPropertyName("support_multi")]
    public bool? SupportMulti { get; set; }

    /// <summary>获取或设置是否支持自购（官方 <c>support_buy_for_self</c>，可选）。</summary>
    [JsonPropertyName("support_buy_for_self")]
    public bool? SupportBuyForSelf { get; set; }

    /// <summary>获取或设置主题列表（官方 <c>theme_list</c>，可选）。</summary>
    [JsonPropertyName("theme_list")]
    public List<MpCardGiftTheme>? ThemeList { get; set; }

    /// <summary>获取或设置分类列表（官方 <c>category_list</c>，可选）。</summary>
    [JsonPropertyName("category_list")]
    public List<MpCardGiftCategory>? CategoryList { get; set; }

    /// <summary>获取或设置货架地址（官方 <c>address</c>，必填）。</summary>
    [JsonPropertyName("address")]
    public string Address { get; set; } = string.Empty;

    /// <summary>获取或设置客服电话（官方 <c>service_phone</c>，必填）。</summary>
    [JsonPropertyName("service_phone")]
    public string ServicePhone { get; set; } = string.Empty;

    /// <summary>获取或设置商户描述（官方 <c>biz_description</c>，必填）。</summary>
    [JsonPropertyName("biz_description")]
    public string BizDescription { get; set; } = string.Empty;

    /// <summary>获取或设置是否需要发票（官方 <c>need_receipt</c>，可选）。</summary>
    [JsonPropertyName("need_receipt")]
    public bool? NeedReceipt { get; set; }

    /// <summary>获取或设置自定义单元格 1（官方 <c>cell_1</c>，可选）。</summary>
    [JsonPropertyName("cell_1")]
    public MpCardGiftCustomCell? Cell1 { get; set; }

    /// <summary>获取或设置自定义单元格 2（官方 <c>cell_2</c>，可选）。</summary>
    [JsonPropertyName("cell_2")]
    public MpCardGiftCustomCell? Cell2 { get; set; }
}

/// <summary>
/// 「创建礼品卡货架」请求（官方 <c>giftcard/page/add</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPageAddRequest
{
    /// <summary>获取或设置货架信息（官方 <c>page</c>，必填）。</summary>
    [JsonPropertyName("page")]
    public MpCardGiftPage Page { get; set; } = new();
}

/// <summary>
/// 「创建礼品卡货架」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPageAddResponse : MpResponse
{
    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>；官方返回数字，字符串承载）。</summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }
}

/// <summary>
/// 「拉取礼品卡货架列表」请求（官方 <c>giftcard/page/batchget</c>；官方契约请求体为空 JSON）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPageBatchGetRequest
{
}

/// <summary>
/// 「拉取礼品卡货架列表」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPageBatchGetResponse : MpResponse
{
    /// <summary>获取或设置货架 ID 列表（官方 <c>page_id_list</c>）。</summary>
    [JsonPropertyName("page_id_list")]
    public string[]? PageIdList { get; set; }
}

/// <summary>
/// 「查询礼品卡货架」请求（官方 <c>giftcard/page/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPageGetRequest
{
    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>，必填）。</summary>
    [JsonPropertyName("page_id")]
    public string PageId { get; set; } = string.Empty;
}

/// <summary>
/// 「查询礼品卡货架」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPageGetResponse : MpResponse
{
    /// <summary>获取或设置货架信息（官方 <c>page</c>）。</summary>
    [JsonPropertyName("page")]
    public MpCardGiftPage? Page { get; set; }
}

/// <summary>
/// 「更新礼品卡货架」请求（官方 <c>giftcard/page/update</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPageUpdateRequest
{
    /// <summary>获取或设置货架信息（官方 <c>page</c>，必填；须携带 page_id）。</summary>
    [JsonPropertyName("page")]
    public MpCardGiftPage Page { get; set; } = new();
}

/// <summary>
/// 「绑定子商户」请求（官方 <c>giftcard/pay/submch/bind</c>；礼品卡支付强耦合面）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPaySubMerchantBindRequest
{
    /// <summary>获取或设置子商户号（官方 <c>sub_mch_id</c>，必填）。</summary>
    [JsonPropertyName("sub_mch_id")]
    public string SubMchId { get; set; } = string.Empty;

    /// <summary>获取或设置小程序 <c>appid</c>（官方 <c>wxa_appid</c>，必填）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string WxaAppId { get; set; } = string.Empty;
}

/// <summary>
/// 「添加白名单」请求（官方 <c>giftcard/pay/whitelist/add</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPayWhitelistAddRequest
{
    /// <summary>获取或设置子商户号（官方 <c>sub_mch_id</c>，必填）。</summary>
    [JsonPropertyName("sub_mch_id")]
    public string SubMchId { get; set; } = string.Empty;
}

/// <summary>
/// 「添加白名单」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftPayWhitelistAddResponse : MpResponse
{
    /// <summary>获取或设置白名单页面地址（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 「设置礼品卡小程序跳转」请求（官方 <c>giftcard/wxa/set</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftWxaSetRequest
{
    /// <summary>获取或设置小程序 <c>appid</c>（官方 <c>wxa_appid</c>，必填）。</summary>
    [JsonPropertyName("wxa_appid")]
    public string WxaAppId { get; set; } = string.Empty;

    /// <summary>获取或设置货架 ID（官方 <c>page_id</c>，必填）。</summary>
    [JsonPropertyName("page_id")]
    public string PageId { get; set; } = string.Empty;
}

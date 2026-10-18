// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AdvancedApi;

/// <summary>
/// 平台能力服务订单列表查询请求体（<c>/cgi-bin/advanced_api/list_order</c>，官方 100260）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class ListAdvancedApiOrdersRequest
{
    /// <summary>
    /// 获取或设置客户企业 id（<c>custom_corpid</c>）。
    /// </summary>
    [JsonPropertyName("custom_corpid")]
    public string? CustomCorpId { get; set; }

    /// <summary>
    /// 获取或设置下单时间范围的开始时间（<c>start_time</c>）。
    /// <para>官方约束：<b>不能单独指定</b>，须与 <see cref="EndTime"/> 同时给出；区间语义为 [start_time, end_time)。</para>
    /// </summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置下单时间范围的结束时间（<c>end_time</c>）。
    /// <para>官方约束：<b>不能单独指定</b>，须与 <see cref="StartTime"/> 同时给出。</para>
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置购买的高级接口类型（<c>advanced_api_type</c>）：目前仅支持「会话内容数据接口」，type 为 1。
    /// </summary>
    [JsonPropertyName("advanced_api_type")]
    public int? AdvancedApiType { get; set; }

    /// <summary>
    /// 获取或设置分页游标（<c>cursor</c>，由上一次调用返回，首次调用可不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置返回的最大记录数（<c>limit</c>，整型，最大 1000，默认 500）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

/// <summary>
/// 平台能力服务订单列表响应体（官方 100260）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class ListAdvancedApiOrdersResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置订单列表（<c>order_list</c>）。
    /// </summary>
    [JsonPropertyName("order_list")]
    public IList<AdvancedApiOrderBrief>? OrderList { get; set; }

    /// <summary>
    /// 获取或设置下一页游标（<c>next_cursor</c>）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置是否有更多记录（<c>has_more</c>）：0 没有、1 有。
    /// <para>官方为<b>整型</b>而非布尔，照抄原文。</para>
    /// </summary>
    [JsonPropertyName("has_more")]
    public int? HasMore { get; set; }
}

/// <summary>
/// 订单列表项（<c>order_list</c> 元素，官方 100260）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class AdvancedApiOrderBrief
{
    /// <summary>
    /// 获取或设置订单 id（<c>order_id</c>）。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 获取或设置订单类型（<c>order_type</c>）：0 新购、1 增购、2 续期、3 升级。
    /// </summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>
    /// 获取或设置订单状态（<c>order_status</c>）：0 待支付、1 已支付、2 已取消、
    /// 3 未支付订单已过期、4 申请退款中、5 退款成功、6 退款被拒绝。
    /// </summary>
    [JsonPropertyName("order_status")]
    public int? OrderStatus { get; set; }

    /// <summary>
    /// 获取或设置订单创建时间（<c>create_time</c>）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}

/// <summary>
/// 订单详情查询请求体（<c>/cgi-bin/advanced_api/get_order</c>，官方 100261）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class GetAdvancedApiOrderRequest
{
    /// <summary>
    /// 获取或设置订单 id（<c>order_id</c>，必填）。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 订单详情响应体（官方 100261）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class GetAdvancedApiOrderResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置订单详情（<c>order</c>，官方 Order 结构）。
    /// </summary>
    [JsonPropertyName("order")]
    public AdvancedApiOrderDetail? Order { get; set; }
}

/// <summary>
/// 订单详情（官方 100261 的 Order 结构）。
/// </summary>
/// <remarks>
/// <b>官方字段语义陷阱</b>：详情里的 <c>custom_corpid</c> 返回<b>加密的 corpid</b>
/// （下单请求传的是明文客户企业 id），二者形态不同，不得据此做请求↔响应等值比对。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class AdvancedApiOrderDetail
{
    /// <summary>
    /// 获取或设置购买的高级接口类型（<c>advanced_api_type</c>）：目前仅「会话内容数据接口」，type 为 1。
    /// </summary>
    [JsonPropertyName("advanced_api_type")]
    public int? AdvancedApiType { get; set; }

    /// <summary>
    /// 获取或设置订单号（<c>order_id</c>）。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 获取或设置订单类型（<c>order_type</c>）：0 新购、1 增购、2 续期、3 升级。
    /// </summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>
    /// 获取或设置订单状态（<c>order_status</c>）：0 待支付、1 已支付、2 已取消、
    /// 3 未支付订单已过期、4 申请退款中、5 退款成功、6 退款被拒绝。
    /// </summary>
    [JsonPropertyName("order_status")]
    public int? OrderStatus { get; set; }

    /// <summary>
    /// 获取或设置客户企业 id（<c>custom_corpid</c>）。
    /// <para>官方语义：此处返回<b>加密的 corpid</b>。</para>
    /// </summary>
    [JsonPropertyName("custom_corpid")]
    public string? CustomCorpId { get; set; }

    /// <summary>
    /// 获取或设置订单创建时间（<c>create_time</c>）。
    /// </summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置下单用户 id（<c>buyer_userid</c>）。
    /// </summary>
    [JsonPropertyName("buyer_userid")]
    public string? BuyerUserId { get; set; }

    /// <summary>
    /// 获取或设置应付金额（<c>paid_price</c>）。
    /// </summary>
    [JsonPropertyName("paid_price")]
    public long? PaidPrice { get; set; }

    /// <summary>
    /// 获取或设置会话内容数据接口订单详情（<c>chat_archive_api</c>）。
    /// <para>官方语义：<c>advanced_api_type</c> 为 1 时返回该字段。</para>
    /// </summary>
    [JsonPropertyName("chat_archive_api")]
    public AdvancedApiOrderArchiveDetail? ChatArchiveApi { get; set; }
}

/// <summary>
/// 订单详情中的「会话内容数据接口」信息（官方 100261 的 ChatArchiveApi 结构）。
/// </summary>
/// <remarks>
/// <b>与下单请求体同键名不同字段集</b>：详情面多出 <c>purchase_duration_days</c> / <c>end_time</c> /
/// <c>original_price</c>（官方注明原价金额仅升级订单返回），下单面的 <c>old_edition</c> 等选择项不回传
/// ⇒ 两类 DTO 不共用。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class AdvancedApiOrderArchiveDetail
{
    /// <summary>
    /// 获取或设置版本（<c>edition</c>）：2 内外部会话、3 内外部会话及语音通话。
    /// </summary>
    [JsonPropertyName("edition")]
    public int? Edition { get; set; }

    /// <summary>
    /// 获取或设置购买人数（<c>purchase_count</c>）。
    /// </summary>
    [JsonPropertyName("purchase_count")]
    public int? PurchaseCount { get; set; }

    /// <summary>
    /// 获取或设置使用时长（<c>purchase_duration_days</c>，单位：天）。
    /// </summary>
    [JsonPropertyName("purchase_duration_days")]
    public int? PurchaseDurationDays { get; set; }

    /// <summary>
    /// 获取或设置生效时间（<c>take_effect_time</c>）。
    /// </summary>
    [JsonPropertyName("take_effect_time")]
    public long? TakeEffectTime { get; set; }

    /// <summary>
    /// 获取或设置到期时间（<c>end_time</c>）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置原价金额（<c>original_price</c>）。
    /// <para>官方语义：目前仅升级订单返回。</para>
    /// </summary>
    [JsonPropertyName("original_price")]
    public long? OriginalPrice { get; set; }
}

/// <summary>
/// 企业高级接口已购信息查询请求体（<c>/cgi-bin/advanced_api/get_corp_buy_info</c>，官方 100271）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class GetAdvancedApiCorpBuyInfoRequest
{
    /// <summary>
    /// 获取或设置购买的高级接口类型（<c>advanced_api_type</c>，必填）：
    /// 目前仅支持「会话内容数据接口」，固定为 1。
    /// </summary>
    [JsonPropertyName("advanced_api_type")]
    public int? AdvancedApiType { get; set; }

    /// <summary>
    /// 获取或设置客户企业 id（<c>custom_corpid</c>，必填）。
    /// </summary>
    [JsonPropertyName("custom_corpid")]
    public string? CustomCorpId { get; set; }
}

/// <summary>
/// 企业高级接口已购信息响应体（官方 100271）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class GetAdvancedApiCorpBuyInfoResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置「会话内容数据接口」的购买信息（<c>chat_archive_api_buy_info</c>）。
    /// </summary>
    [JsonPropertyName("chat_archive_api_buy_info")]
    public AdvancedApiCorpBuyInfo? ChatArchiveApiBuyInfo { get; set; }
}

/// <summary>
/// 「会话内容数据接口」购买信息（官方 100271）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class AdvancedApiCorpBuyInfo
{
    /// <summary>
    /// 获取或设置已购版本列表（<c>edition_list</c>）。
    /// </summary>
    [JsonPropertyName("edition_list")]
    public IList<AdvancedApiBoughtEdition>? EditionList { get; set; }
}

/// <summary>
/// 已购版本（<c>edition_list</c> 元素，官方 100271）。
/// </summary>
/// <remarks>
/// <b>版本枚举比下单面更宽</b>：官方此处含 <c>1</c> 内部会话（历史购买的），下单面 <c>edition</c> 仅 2/3 ⇒ 照抄原文、不做归一。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class AdvancedApiBoughtEdition
{
    /// <summary>
    /// 获取或设置订单购买的版本（<c>edition</c>）：1 内部会话（历史购买的）、2 内外部会话、3 内外部会话及语音通话。
    /// </summary>
    [JsonPropertyName("edition")]
    public int? Edition { get; set; }

    /// <summary>
    /// 获取或设置购买人数（<c>purchase_count</c>）。
    /// </summary>
    [JsonPropertyName("purchase_count")]
    public int? PurchaseCount { get; set; }

    /// <summary>
    /// 获取或设置生效时间（<c>begin_time</c>）。
    /// </summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>
    /// 获取或设置到期时间（<c>end_time</c>）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}

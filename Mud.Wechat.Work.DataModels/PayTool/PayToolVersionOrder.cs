// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.PayTool;

/// <summary>
/// 应用版本付费订单信息（<c>order_list</c> 元素，<c>/cgi-bin/service/get_order_list</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方「获取订单列表」与「获取订单详情」两端的订单字段<b>完全同一</b>，故共用本类型；
/// 差别仅在承载位置（列表为 <c>order_list</c> 数组元素、详情为响应根级字段）。
/// </para>
/// <para>
/// <b>官方契约陷阱</b>：字段名官方为全小写 <c>orderid</c>（非 <c>order_id</c>）、
/// <c>suiteid</c>（非 <c>suite_id</c>），本 SDK 照抄官方原文。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolVersionOrder : WechatWorkResponse
{
    /// <summary>获取或设置订单号。</summary>
    [JsonPropertyName("orderid")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 获取或设置订单状态：<c>0</c>-待支付 / <c>1</c>-已支付 / <c>2</c>-已取消 / <c>3</c>-支付过期 /
    /// <c>4</c>-申请退款中 / <c>5</c>-退款成功 / <c>6</c>-退款被拒绝。
    /// </summary>
    [JsonPropertyName("order_status")]
    public int? OrderStatus { get; set; }

    /// <summary>
    /// 获取或设置订单类型：<c>0</c>-新购应用 / <c>1</c>-扩容应用人数 /
    /// <c>2</c>-续期应用时间 / <c>3</c>-变更版本。
    /// </summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置客户企业的 corpid。</summary>
    [JsonPropertyName("paid_corpid")]
    public string? PaidCorpid { get; set; }

    /// <summary>
    /// 获取或设置下单操作人员 userid。
    /// <para>官方口径：部分情况没有该字段（如服务商代下单、服务商下的免支付订单等）。</para>
    /// </summary>
    [JsonPropertyName("operator_id")]
    public string? OperatorId { get; set; }

    /// <summary>获取或设置应用 id（官方字段名为全小写 suiteid）。</summary>
    [JsonPropertyName("suiteid")]
    public string? SuiteId { get; set; }

    /// <summary>获取或设置套件应用 id（仅旧套件有该字段）。</summary>
    [JsonPropertyName("appid")]
    public int? AppId { get; set; }

    /// <summary>获取或设置购买版本 ID。</summary>
    [JsonPropertyName("edition_id")]
    public string? EditionId { get; set; }

    /// <summary>获取或设置购买版本名字。</summary>
    [JsonPropertyName("edition_name")]
    public string? EditionName { get; set; }

    /// <summary>获取或设置应付价格（单位分）。</summary>
    [JsonPropertyName("price")]
    public long? Price { get; set; }

    /// <summary>获取或设置购买的人数。</summary>
    [JsonPropertyName("user_count")]
    public int? UserCount { get; set; }

    /// <summary>获取或设置购买的时长（单位为天）。</summary>
    [JsonPropertyName("order_period")]
    public int? OrderPeriod { get; set; }

    /// <summary>获取或设置下单时间（UNIX 时间戳）。</summary>
    [JsonPropertyName("order_time")]
    public long? OrderTime { get; set; }

    /// <summary>获取或设置付款时间（UNIX 时间戳）。</summary>
    [JsonPropertyName("paid_time")]
    public long? PaidTime { get; set; }

    /// <summary>获取或设置购买生效期的开始时间（UNIX 时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置购买生效期的结束时间（UNIX 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置下单来源：<c>0</c>-企业下单 / <c>1</c>-服务商代下单 /
    /// <c>2</c>-代理商代下单 / <c>3</c>-服务商收银台下单。
    /// </summary>
    [JsonPropertyName("order_from")]
    public int? OrderFrom { get; set; }

    /// <summary>获取或设置下单方 corpid。</summary>
    [JsonPropertyName("operator_corpid")]
    public string? OperatorCorpid { get; set; }

    /// <summary>获取或设置服务商分成金额（单位分）。</summary>
    [JsonPropertyName("service_share_amount")]
    public long? ServiceShareAmount { get; set; }

    /// <summary>获取或设置平台分成金额（单位分）。</summary>
    [JsonPropertyName("platform_share_amount")]
    public long? PlatformShareAmount { get; set; }

    /// <summary>获取或设置代理商分成金额（单位分）。</summary>
    [JsonPropertyName("dealer_share_amount")]
    public long? DealerShareAmount { get; set; }

    /// <summary>
    /// 获取或设置渠道商信息（仅当有渠道商报备后才会有此字段，见 <see cref="PayToolVersionDealerCorpInfo"/>）。
    /// </summary>
    [JsonPropertyName("dealer_corp_info")]
    public PayToolVersionDealerCorpInfo? DealerCorpInfo { get; set; }
}

/// <summary>
/// 渠道商信息（<c>dealer_corp_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayTool")]
public class PayToolVersionDealerCorpInfo
{
    /// <summary>获取或设置代理商 corpid。</summary>
    [JsonPropertyName("corpid")]
    public string? Corpid { get; set; }

    /// <summary>获取或设置代理商的企业简称。</summary>
    [JsonPropertyName("corp_name")]
    public string? CorpName { get; set; }
}
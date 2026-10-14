// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.MerchantService;

/// <summary>
/// 投诉单列表应答（官方 <c>GET /v3/merchant-service/complaints-v2</c> 的应答体）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 逐字段核验）。
/// </para>
/// <para>
/// <b><c>limit</c> 与 <c>offset</c> 在应答侧是必填回显</b>（官方 true），而 <c>total_count</c> 官方注明
/// 「<b>当 offset=0 时返回</b>」⇒ 总数只首页可得，翻页时不得假定存在。
/// </para>
/// <para>
/// <b>本域分页是 <c>offset</c>/<c>limit</c>，没有游标</b>：与协商历史页共用同一对参数名，
/// 但两页的 <c>limit</c> 上界<b>不同</b>（列表 50、协商历史 300，见接口 XML 注释）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintListResponse : WechatPayResponse
{
    /// <summary>投诉单列表（<c>data</c>，选填 array[object]）。</summary>
    [JsonPropertyName("data")]
    public MerchantServiceComplaintListItem[]? Data { get; set; }

    /// <summary>分页大小（<c>limit</c>，官方<b>必填</b>回显 integer）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>分页开始位置（<c>offset</c>，官方<b>必填</b>回显 integer）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>投诉单总数（<c>total_count</c>，选填 integer）：<b>仅 <c>offset=0</c> 时返回</b>。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>
/// 投诉单列表项（官方 <c>data[]</c>，<b>20</b> 个字段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（商户面）
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691285"/>（服务商面），2026-10-14 两面逐行比对。
/// </para>
/// <para>
/// <b>两面唯一差异 = <see cref="ComplaintedMchId"/></b>：服务商面标<b>必填返回</b>、商户面<b>整行不存在</b>；
/// 其余四十六行（含全部子树）<b>逐字符相同</b> ⇒ 本域按「应答侧多一个可空键」处理为<b>超集共用一型</b>，
/// 而不是把同一条 GET 拆成两个方法。这与 <c>MerchantRiskManage</c> 的「同路由分建两形态」不同，
/// 那里的差异落在<b>请求必填性</b>上（必须两方法才表达得出），判据始终是官方表差在哪一侧。
/// </para>
/// <para>
/// <b>⚠️ <c>payer_openid</c> 不在本页</b>：只有详情面返回（见 <see cref="MerchantServiceComplaintDetail"/>）
/// ⇒ 需要 openid 做客服触达时，列表里拿不到，必须逐单调详情（守卫 MC-B6）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintListItem
{
    /// <summary>投诉单号（<c>complaint_id</c>，必填 string(64)）：本域全部写支的定位键。</summary>
    [JsonPropertyName("complaint_id")]
    public string? ComplaintId { get; set; }

    /// <summary>投诉时间（<c>complaint_time</c>，必填 string(32)，rfc3339）。</summary>
    [JsonPropertyName("complaint_time")]
    public string? ComplaintTime { get; set; }

    /// <summary>投诉详情（<c>complaint_detail</c>，必填 string(300)）：用户原话。</summary>
    [JsonPropertyName("complaint_detail")]
    public string? ComplaintDetail { get; set; }

    /// <summary>投诉单状态（<c>complaint_state</c>，必填 string(30)），取值见 <see cref="MerchantServiceComplaintStates"/>。</summary>
    [JsonPropertyName("complaint_state")]
    public string? ComplaintState { get; set; }

    /// <summary>
    /// 被诉商户号（<c>complainted_mchid</c>，<b>仅服务商面返回</b>，必填 string(64)）：商户面为 <c>null</c>。
    /// </summary>
    [JsonPropertyName("complainted_mchid")]
    public string? ComplaintedMchId { get; set; }

    /// <summary>
    /// 投诉人联系方式（<c>payer_phone</c>，选填 <b>string(512)</b>）：官方以<b>密文</b>形态返回，
    /// 长度 512 远大于手机号 ⇒ 不得按「11 位手机号」解析或校验。
    /// </summary>
    [JsonPropertyName("payer_phone")]
    public string? PayerPhone { get; set; }

    /// <summary>投诉单关联订单信息（<c>complaint_order_info</c>，选填 array[object]）。</summary>
    [JsonPropertyName("complaint_order_info")]
    public MerchantServiceComplaintOrderInfo[]? ComplaintOrderInfo { get; set; }

    /// <summary>投诉单是否已全额退款（<c>complaint_full_refunded</c>，必填 boolean）。</summary>
    [JsonPropertyName("complaint_full_refunded")]
    public bool? ComplaintFullRefunded { get; set; }

    /// <summary>是否有用户新留言（<c>incoming_user_response</c>，必填 boolean）。</summary>
    [JsonPropertyName("incoming_user_response")]
    public bool? IncomingUserResponse { get; set; }

    /// <summary>用户投诉次数（<c>user_complaint_times</c>，必填 integer）。</summary>
    [JsonPropertyName("user_complaint_times")]
    public long? UserComplaintTimes { get; set; }

    /// <summary>
    /// 投诉资料凭证（<c>complaint_media_list</c>，选填 array[object]）：<b>本域取凭证的推荐入口</b>。
    /// </summary>
    [JsonPropertyName("complaint_media_list")]
    public MerchantServiceComplaintMedia[]? ComplaintMediaList { get; set; }

    /// <summary>问题描述（<c>problem_description</c>，必填 string(256)）。</summary>
    [JsonPropertyName("problem_description")]
    public string? ProblemDescription { get; set; }

    /// <summary>问题类型（<c>problem_type</c>，选填 string），取值见 <see cref="MerchantServiceProblemTypes"/>。</summary>
    [JsonPropertyName("problem_type")]
    public string? ProblemType { get; set; }

    /// <summary>申请退款金额（<c>apply_refund_amount</c>，选填 integer，单位<b>分</b>）。</summary>
    [JsonPropertyName("apply_refund_amount")]
    public long? ApplyRefundAmount { get; set; }

    /// <summary>用户标签列表（<c>user_tag_list</c>，选填 array[string]），取值见 <see cref="MerchantServiceUserTags"/>。</summary>
    [JsonPropertyName("user_tag_list")]
    public string[]? UserTagList { get; set; }

    /// <summary>投诉单关联的支付分服务单（<c>service_order_info</c>，选填 array[object]）。</summary>
    [JsonPropertyName("service_order_info")]
    public MerchantServiceComplaintServiceOrderInfo[]? ServiceOrderInfo { get; set; }

    /// <summary>补充信息（<c>additional_info</c>，选填）：用户发起投诉时选择的补充资料。</summary>
    [JsonPropertyName("additional_info")]
    public MerchantServiceComplaintAdditionalInfo? AdditionalInfo { get; set; }

    /// <summary>是否正在申请平台协助（<c>in_platform_service</c>，选填 boolean）。</summary>
    [JsonPropertyName("in_platform_service")]
    public bool? InPlatformService { get; set; }

    /// <summary>
    /// 是否需即时服务处理（<c>need_immediate_service</c>，选填 boolean）：<b>为真时应走
    /// <c>response-immediate-service</c> 支而非普通回复支</b>（消息树见 <see cref="MerchantServiceNormalMessage"/>）。
    /// </summary>
    [JsonPropertyName("need_immediate_service")]
    public bool? NeedImmediateService { get; set; }

    /// <summary>是否是智能体投诉（<c>is_agent_mode</c>，选填 boolean）。</summary>
    [JsonPropertyName("is_agent_mode")]
    public bool? IsAgentMode { get; set; }
}

/// <summary>
/// 投诉单详情（官方 <c>GET /v3/merchant-service/complaints-v2/{complaint_id}</c> 的应答体，<b>无包装层</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533436"/>（商户面）
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012691648"/>（服务商面），2026-10-14 两面逐行比对。
/// </para>
/// <para>
/// <b>与列表项的差别只有两处</b>：① 多一个 <see cref="PayerOpenId"/>（<b>只在详情面出现</b>）；
/// ② 字段直接挂在应答根上、不套 <c>data[]</c> ⇒ 本类型<b>独立成树</b>而不与
/// <see cref="MerchantServiceComplaintListItem"/> 继承复用（官方两表行数 39 与 20，形态亦不同）。
/// 面差异同列表：<c>complainted_mchid</c> 仅服务商面必填返回 ⇒ 超集可空共用一型。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintDetail : WechatPayResponse
{
    /// <summary>投诉单号（<c>complaint_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("complaint_id")]
    public string? ComplaintId { get; set; }

    /// <summary>投诉时间（<c>complaint_time</c>，必填 string(32)，rfc3339）。</summary>
    [JsonPropertyName("complaint_time")]
    public string? ComplaintTime { get; set; }

    /// <summary>投诉详情（<c>complaint_detail</c>，必填 string(300)）。</summary>
    [JsonPropertyName("complaint_detail")]
    public string? ComplaintDetail { get; set; }

    /// <summary>投诉单状态（<c>complaint_state</c>，必填 string(30)），取值见 <see cref="MerchantServiceComplaintStates"/>。</summary>
    [JsonPropertyName("complaint_state")]
    public string? ComplaintState { get; set; }

    /// <summary>被诉商户号（<c>complainted_mchid</c>，<b>仅服务商面返回</b>）：商户面为 <c>null</c>。</summary>
    [JsonPropertyName("complainted_mchid")]
    public string? ComplaintedMchId { get; set; }

    /// <summary>投诉人联系方式（<c>payer_phone</c>，选填 string(512)，密文形态）。</summary>
    [JsonPropertyName("payer_phone")]
    public string? PayerPhone { get; set; }

    /// <summary>
    /// 投诉人 openid（<c>payer_openid</c>，选填 string(128)）：<b>详情面独有的键</b>，
    /// 列表与协商历史两面均不返回。
    /// </summary>
    [JsonPropertyName("payer_openid")]
    public string? PayerOpenId { get; set; }

    /// <summary>投诉单关联订单信息（<c>complaint_order_info</c>，选填 array[object]）。</summary>
    [JsonPropertyName("complaint_order_info")]
    public MerchantServiceComplaintOrderInfo[]? ComplaintOrderInfo { get; set; }

    /// <summary>投诉单是否已全额退款（<c>complaint_full_refunded</c>，必填 boolean）。</summary>
    [JsonPropertyName("complaint_full_refunded")]
    public bool? ComplaintFullRefunded { get; set; }

    /// <summary>是否有用户新留言（<c>incoming_user_response</c>，必填 boolean）。</summary>
    [JsonPropertyName("incoming_user_response")]
    public bool? IncomingUserResponse { get; set; }

    /// <summary>用户投诉次数（<c>user_complaint_times</c>，必填 integer）。</summary>
    [JsonPropertyName("user_complaint_times")]
    public long? UserComplaintTimes { get; set; }

    /// <summary>投诉资料凭证（<c>complaint_media_list</c>，选填 array[object]）。</summary>
    [JsonPropertyName("complaint_media_list")]
    public MerchantServiceComplaintMedia[]? ComplaintMediaList { get; set; }

    /// <summary>问题描述（<c>problem_description</c>，必填 string(256)）。</summary>
    [JsonPropertyName("problem_description")]
    public string? ProblemDescription { get; set; }

    /// <summary>问题类型（<c>problem_type</c>，选填 string），取值见 <see cref="MerchantServiceProblemTypes"/>。</summary>
    [JsonPropertyName("problem_type")]
    public string? ProblemType { get; set; }

    /// <summary>申请退款金额（<c>apply_refund_amount</c>，选填 integer，单位分）。</summary>
    [JsonPropertyName("apply_refund_amount")]
    public long? ApplyRefundAmount { get; set; }

    /// <summary>用户标签列表（<c>user_tag_list</c>，选填 array[string]）。</summary>
    [JsonPropertyName("user_tag_list")]
    public string[]? UserTagList { get; set; }

    /// <summary>投诉单关联的支付分服务单（<c>service_order_info</c>，选填 array[object]）。</summary>
    [JsonPropertyName("service_order_info")]
    public MerchantServiceComplaintServiceOrderInfo[]? ServiceOrderInfo { get; set; }

    /// <summary>补充信息（<c>additional_info</c>，选填）。</summary>
    [JsonPropertyName("additional_info")]
    public MerchantServiceComplaintAdditionalInfo? AdditionalInfo { get; set; }

    /// <summary>是否正在申请平台协助（<c>in_platform_service</c>，选填 boolean）。</summary>
    [JsonPropertyName("in_platform_service")]
    public bool? InPlatformService { get; set; }

    /// <summary>是否需即时服务处理（<c>need_immediate_service</c>，选填 boolean）。</summary>
    [JsonPropertyName("need_immediate_service")]
    public bool? NeedImmediateService { get; set; }

    /// <summary>是否是智能体投诉（<c>is_agent_mode</c>，选填 boolean）。</summary>
    [JsonPropertyName("is_agent_mode")]
    public bool? IsAgentMode { get; set; }
}

/// <summary>投诉单关联订单（官方 <c>complaint_order_info[]</c>）：列表 / 详情 / 协商历史三面共用。</summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// <b><c>amount</c> 官方是 integer（单位分）</b>⇒ 以 <c>long?</c> 承载（线内惯例，防越出 int32）。
/// 一笔投诉可关联<b>多笔</b>订单，故是列表。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintOrderInfo
{
    /// <summary>微信支付订单号（<c>transaction_id</c>，必填 string(64)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，必填 string(64)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>订单金额（<c>amount</c>，必填 integer，单位<b>分</b>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>
/// 投诉资料凭证（官方 <c>complaint_media_list</c> 的元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（列表 / 详情面为 array[object]）
/// 与 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533439"/>（协商历史面为<b>单个 object</b>），
/// 2026-10-14 核验：<b>两面的内部键与必填性完全一致</b> ⇒ 共用本一型，容器形态差异留在属性上
/// （列表侧 <c>MerchantServiceComplaintMedia[]?</c>、历史侧 <c>MerchantServiceComplaintMedia?</c>）。
/// </para>
/// <para>
/// <b><c>media_url</c> 不是可直接下载的 URL</b>：官方原文给出的是
/// <c>https://api.mch.weixin.qq.com/v3/merchant-service/images/xxxxx</c> 形态，其中末段才是媒体标识 ID。
/// 取图须剥出末段再走「图片请求」通道（带签名），直接 HTTP GET 该串会因缺签名失败。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintMedia
{
    /// <summary>媒体文件业务类型（<c>media_type</c>，必填 string），取值见 <see cref="MerchantServiceMediaTypes"/>。</summary>
    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    /// <summary>媒体文件请求 URL 列表（<c>media_url</c>，必填 array[string(512)]）：末段是媒体标识 ID。</summary>
    [JsonPropertyName("media_url")]
    public string[]? MediaUrl { get; set; }
}

/// <summary>投诉单关联的支付分服务单（官方 <c>service_order_info[]</c>）。</summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// <b>三键全为选填</b>（含 <c>state</c>），且 <c>state</c> 是<b>投诉发生时的快照</b>、不实时更新。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintServiceOrderInfo
{
    /// <summary>微信支付服务单号（<c>order_id</c>，选填 string(128)）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>商户服务单号（<c>out_order_no</c>，选填 string(128)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务单状态（<c>state</c>，选填 string），取值见 <see cref="MerchantServiceServiceOrderStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }
}

/// <summary>
/// 投诉补充信息（官方 <c>additional_info</c>）：按 <c>type</c> 判别，当前官方只有共享充电宝一类。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// <b>本层只有 <c>type</c> 与 <c>share_power_info</c> 两键</b> —— 「是否同一台机器归还」挂在
/// <see cref="MerchantServiceComplaintSharePowerInfo"/> 里而非本层（两面同行，勿在补充信息层找它）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintAdditionalInfo
{
    /// <summary>补充信息类型（<c>type</c>，选填 string），取值见 <see cref="MerchantServiceAdditionalInfoTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>充电宝类补充信息（<c>share_power_info</c>，选填）。</summary>
    [JsonPropertyName("share_power_info")]
    public MerchantServiceComplaintSharePowerInfo? SharePowerInfo { get; set; }
}

/// <summary>充电宝类补充信息（官方 <c>additional_info.share_power_info</c>）。</summary>
/// <remarks>官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。</remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintSharePowerInfo
{
    /// <summary>归还时间（<c>return_time</c>，选填 string）。</summary>
    [JsonPropertyName("return_time")]
    public string? ReturnTime { get; set; }

    /// <summary>归还地址（<c>return_address_info</c>，选填）。</summary>
    [JsonPropertyName("return_address_info")]
    public MerchantServiceComplaintReturnAddressInfo? ReturnAddressInfo { get; set; }

    /// <summary>是否在同一台机器归还（<c>is_returned_to_same_machine</c>，选填 boolean）。</summary>
    [JsonPropertyName("is_returned_to_same_machine")]
    public bool? IsReturnedToSameMachine { get; set; }
}

/// <summary>
/// 归还地址（官方 <c>share_power_info.return_address_info</c>）。
/// </summary>
/// <remarks>
/// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533431"/>（2026-10-14 核验）。
/// <b><c>longitude</c> / <c>latitude</c> 官方是 string(32) 而非数值</b>（与门店域同形）⇒
/// 建成 <c>double?</c> 会在遇到非纯数字历史数据时反序列化失败。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceComplaintReturnAddressInfo
{
    /// <summary>归还地址（<c>return_address</c>，选填 string(256)）。</summary>
    [JsonPropertyName("return_address")]
    public string? ReturnAddress { get; set; }

    /// <summary>经度（<c>longitude</c>，选填 <b>string</b>(32)）。</summary>
    [JsonPropertyName("longitude")]
    public string? Longitude { get; set; }

    /// <summary>纬度（<c>latitude</c>，选填 <b>string</b>(32)）。</summary>
    [JsonPropertyName("latitude")]
    public string? Latitude { get; set; }
}

/// <summary>
/// 投诉协商历史应答（官方 <c>GET /v3/merchant-service/complaints-v2/{complaint_id}/negotiation-historys</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533439"/>（2026-10-14 逐字段核验；
/// 商户面与服务商面<b>九十七行逐字符一致</b> ⇒ 本域是「两面完全共用」的典型）。
/// </para>
/// <para>
/// <b>分页壳与列表页同名但上界不同</b>：本页 <c>limit</c> 范围 [1,300]，列表页 [1,50]；
/// 且本页 <c>total_count</c> 同样只在 <c>offset=0</c> 返回。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceNegotiationHistoryResponse : WechatPayResponse
{
    /// <summary>投诉协商历史（<c>data</c>，选填 array[object]）。</summary>
    [JsonPropertyName("data")]
    public MerchantServiceNegotiationHistoryItem[]? Data { get; set; }

    /// <summary>分页大小（<c>limit</c>，官方必填回显 integer，范围 [1,300]）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>分页开始位置（<c>offset</c>，官方必填回显 integer）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>协商历史总条数（<c>total_count</c>，选填 integer）：<b>仅 <c>offset=0</c> 时返回</b>。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

/// <summary>
/// 单条协商历史（官方 <c>data[]</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012533439"/>（2026-10-14 核验）。
/// </para>
/// <para>
/// <b>⚠️ 两套图片凭证、状态不同</b>：<c>image_list</c> 是历史遗留字段（官方原文：「此字段<b>不包含用户提交的
/// 图片凭证</b>，建议统一使用 <c>complaint_media_list</c> 接收和请求资料凭证，<b>未来该字段将废弃</b>」），
/// 且返回的是<b>可直接展示的图床 URL</b>；<c>complaint_media_list</c> 返回的是需签名请求的 API URL。
/// 本仓两键都建模，但新代码只应依赖后者。
/// </para>
/// <para>
/// <b><c>complaint_media_list</c> 在本页是单个 object</b>（列表 / 详情页是 array）⇒ 属性形态不同、
/// 元素类型共用 <see cref="MerchantServiceComplaintMedia"/>。
/// </para>
/// <para>
/// <b>两个「用户申请平台协助原因」字段官方已标废弃</b>（<c>user_appy_platform_service_reason</c> 及其
/// <c>_description</c>，注意官方<b>少一个字母 l</b>：<c>appy</c> 而非 <c>apply</c>）⇒ 字段名照录，
/// 「顺手纠正」拼写会造成名单不匹配。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MerchantService")]
public class MerchantServiceNegotiationHistoryItem
{
    /// <summary>操作流水号（<c>log_id</c>，必填 string(64)）：与即时服务回复返回的 <c>log_id</c> 对应，可追踪去重。</summary>
    [JsonPropertyName("log_id")]
    public string? LogId { get; set; }

    /// <summary>操作人（<c>operator</c>，必填 string(64)）：如「投诉人」「商户」「微信支付客服」。</summary>
    [JsonPropertyName("operator")]
    public string? Operator { get; set; }

    /// <summary>操作时间（<c>operate_time</c>，必填 string(64)，rfc3339）。</summary>
    [JsonPropertyName("operate_time")]
    public string? OperateTime { get; set; }

    /// <summary>操作类型（<c>operate_type</c>，必填 string），取值见 <see cref="MerchantServiceNegotiationOperateTypes"/>。</summary>
    [JsonPropertyName("operate_type")]
    public string? OperateType { get; set; }

    /// <summary>操作内容（<c>operate_details</c>，选填 string(500)）：拒绝退款时此处展示拒绝原因。</summary>
    [JsonPropertyName("operate_details")]
    public string? OperateDetails { get; set; }

    /// <summary>
    /// 图片凭证（旧字段，<c>image_list</c>，选填 array[string]）：官方注明<b>将废弃</b>且不含用户提交的凭证。
    /// </summary>
    [JsonPropertyName("image_list")]
    public string[]? ImageList { get; set; }

    /// <summary>
    /// 操作资料列表（<c>complaint_media_list</c>，选填<b>单对象</b>）：本域取凭证的推荐入口。
    /// </summary>
    [JsonPropertyName("complaint_media_list")]
    public MerchantServiceComplaintMedia? ComplaintMediaList { get; set; }

    /// <summary>用户申请平台协助原因（<c>user_appy_platform_service_reason</c>，选填 string，<b>官方已废弃</b>）。</summary>
    [JsonPropertyName("user_appy_platform_service_reason")]
    public string? UserApplyPlatformServiceReason { get; set; }

    /// <summary>用户申请平台协助原因描述（<c>user_appy_platform_service_reason_description</c>，选填 string，<b>官方已废弃</b>）。</summary>
    [JsonPropertyName("user_appy_platform_service_reason_description")]
    public string? UserApplyPlatformServiceReasonDescription { get; set; }

    /// <summary>
    /// 普通消息内容（<c>normal_message</c>，选填）：当 <c>operate_type</c> 为 <c>MERCHANT_RESPONSE</c>
    /// 且商户走即时服务回复支时才有值，结构与发送侧 <see cref="MerchantServiceNormalMessage"/> 一致。
    /// </summary>
    [JsonPropertyName("normal_message")]
    public MerchantServiceNormalMessage? NormalMessage { get; set; }

    /// <summary>用户点击回复消息（<c>click_message</c>，选填）：当 <c>operate_type</c> 为 <c>USER_CLICK_RESPONSE</c> 时才有值。</summary>
    [JsonPropertyName("click_message")]
    public MerchantServiceClickMessage? ClickMessage { get; set; }
}

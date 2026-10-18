// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.AdvancedApi;

/// <summary>
/// 高级接口下单请求体（<c>/cgi-bin/advanced_api/create_order</c>，官方 100257）。
/// </summary>
/// <remarks>
/// <b>写操作</b>：本请求会在服务商平台生成真实订单，SDK 不做自动重试；
/// 官方并发错误码 <c>701142</c>（购买信息已被其他请求更新，请重试）语义为「可安全重试」，
/// 由宿主自行决定是否重试。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class CreateAdvancedApiOrderRequest
{
    /// <summary>
    /// 获取或设置购买的高级接口类型（<c>advanced_api_type</c>）：1 会话内容数据接口，
    /// 官方目前仅支持 1 且默认值为 1。
    /// </summary>
    [JsonPropertyName("advanced_api_type")]
    public int? AdvancedApiType { get; set; }

    /// <summary>
    /// 获取或设置客户企业 id（<c>custom_corpid</c>，必填）。
    /// </summary>
    [JsonPropertyName("custom_corpid")]
    public string? CustomCorpId { get; set; }

    /// <summary>
    /// 获取或设置下单人（<c>buyer_userid</c>，必填）：服务商企业内成员的<b>明文</b> userid，
    /// 官方要求必须为服务商企业内具有「购买高级接口」或「高级接口管理」权限的成员
    /// （最终也支持由其他人支付）。
    /// </summary>
    [JsonPropertyName("buyer_userid")]
    public string? BuyerUserId { get; set; }

    /// <summary>
    /// 获取或设置订单类型（<c>order_type</c>）：0 新购、1 增购、2 续期、3 升级；
    /// 企业未购买过时默认新购。
    /// </summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>
    /// 获取或设置会话内容数据接口订单信息（<c>chat_archive_api</c>）。
    /// <para>官方约束：当 <c>advanced_api_type</c> 为 1 时<b>必填</b>。</para>
    /// </summary>
    [JsonPropertyName("chat_archive_api")]
    public AdvancedApiChatArchiveOrderInfo? ChatArchiveApi { get; set; }
}

/// <summary>
/// 下单时的「会话内容数据接口」订单信息（<c>chat_archive_api</c>，官方 100257 的 ChatArchiveApi 结构）。
/// </summary>
/// <remarks>
/// 官方版本状态矩阵（勿「顺手放宽」）：<c>edition</c> 仅未到期版本可增购、仅版本到期前 60 天起可续期、
/// 仅未到期版本 + 内外部会话版本可升级；<b>订单类型为升级时 <c>edition</c> 填的是目标升级版本</b>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class AdvancedApiChatArchiveOrderInfo
{
    /// <summary>
    /// 获取或设置版本（<c>edition</c>，官方标记必填）：2 内外部会话、3 内外部会话及语音通话。
    /// </summary>
    [JsonPropertyName("edition")]
    public int? Edition { get; set; }

    /// <summary>
    /// 获取或设置待升级的旧版本（<c>old_edition</c>）：订单类型为升级时必填，官方目前仅支持 2，默认值为 2。
    /// </summary>
    [JsonPropertyName("old_edition")]
    public int? OldEdition { get; set; }

    /// <summary>
    /// 获取或设置购买人数（<c>purchase_count</c>）。
    /// <para>
    /// 官方约束：新购 / 增购 / 升级<b>必须</b>指定人数（升级人数不可超出原版本已购人数）；
    /// 续期可指定，范围为 [1, 1000000]，未指定则默认为当前版本已购人数。
    /// </para>
    /// </summary>
    [JsonPropertyName("purchase_count")]
    public int? PurchaseCount { get; set; }

    /// <summary>
    /// 获取或设置生效时间戳（<c>take_effect_time</c>）。
    /// <para>
    /// 官方约束：仅新购、续期过期版本支持指定；若填则必须<b>≥ 当天 0 点且在 90 天内</b>（含第 90 天），
    /// 非 0 点的时间戳实际按指定日期 0 点生效；已有默认生效日期的场景（增购 / 未过期续期 / 升级）本字段无效。
    /// </para>
    /// </summary>
    [JsonPropertyName("take_effect_time")]
    public long? TakeEffectTime { get; set; }

    /// <summary>
    /// 获取或设置「需操作的旧版本」的生效/结束时间（<c>old_edition_info</c>）。
    /// <para>官方语义：增购、升级订单且目标版本同时存在已生效与待生效版本时可指定；未指定则默认选择未过期且生效时间最早的版本。</para>
    /// </summary>
    [JsonPropertyName("old_edition_info")]
    public AdvancedApiEditionPeriodInfo? OldEditionInfo { get; set; }

    /// <summary>
    /// 获取或设置「目标版本」的生效/结束时间（<c>target_edition_info</c>）。
    /// <para>官方语义：未指定则默认升级到生效时间最晚的版本。</para>
    /// </summary>
    [JsonPropertyName("target_edition_info")]
    public AdvancedApiEditionPeriodInfo? TargetEditionInfo { get; set; }
}

/// <summary>
/// 版本生效区间（官方 100257 的 EditonInfo 结构；官方原文键名拼写为 <c>EditonInfo</c>，字段名照抄 <c>begin_time</c> / <c>end_time</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class AdvancedApiEditionPeriodInfo
{
    /// <summary>
    /// 获取或设置所选版本的生效时间（<c>begin_time</c>）。
    /// <para>官方约束：与 <see cref="EndTime"/> 必须同时指定且有效。</para>
    /// </summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>
    /// 获取或设置所选版本的结束时间（<c>end_time</c>）。
    /// <para>官方约束：与 <see cref="BeginTime"/> 必须同时指定且有效。</para>
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}

/// <summary>
/// 高级接口下单响应体（官方 100257）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class CreateAdvancedApiOrderResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置订单号（<c>order_id</c>）。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 高级接口取消订单请求体（<c>/cgi-bin/advanced_api/cancel_order</c>，官方 100258）。
/// </summary>
/// <remarks><b>写操作且仅限未支付、未失效订单</b>；官方无业务响应负载。</remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class CancelAdvancedApiOrderRequest
{
    /// <summary>
    /// 获取或设置订单 id（<c>order_id</c>，必填）。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 提交余额支付订单请求体（<c>/cgi-bin/advanced_api/submit_pay</c>，官方 100259）。
/// </summary>
/// <remarks>
/// <b>不可逆写操作</b>：官方明确「提交成功后，该订单无法再变更支付方式」，支付成功后自动扣款
/// ⇒ SDK 不做自动重试，重复提交风险由宿主承担。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "AdvancedApi")]
public class SubmitAdvancedApiOrderPayRequest
{
    /// <summary>
    /// 获取或设置支付人（<c>payer_userid</c>，必填）：服务商企业内成员的<b>明文</b> userid，
    /// 用于充值账户的流水记录，官方要求须具有「购买高级接口」或「高级接口管理」权限。
    /// </summary>
    [JsonPropertyName("payer_userid")]
    public string? PayerUserId { get; set; }

    /// <summary>
    /// 获取或设置要使用充值账户余额支付的订单 id（<c>order_id</c>，必填）。
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

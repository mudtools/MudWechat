// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>
/// 投诉单主体（<c>complaintOrder</c>，官方「查询投诉单详情」参数表）。
/// </summary>
/// <remarks>
/// <para>
/// <b>敏感信息口径（MP-X7 同族）</b>：<see cref="OpenId"/> 与 <see cref="PhoneNumber"/> 属用户标识与联系方式，
/// 不得写入日志、遥测或异常消息。
/// </para>
/// <para>
/// <b>用户标识字段名照官方参数表 <c>openid</c></b>：官方本页<b>代码示例</b>写作 <c>openId</c>（大写 I），
/// 参数表与普通文档口径为 <c>openid</c> ⇒ 以参数表为准（本仓同域先例见 Vip 域 <c>outer_userid</c> 的处理）。
/// </para>
/// <para>
/// <b>状态与到期时间</b>：<see cref="Status"/> 决定可执行的商家动作（回应 / 补充凭证 / 提交退款 / 申诉），
/// 官方在回应投诉的错误码表里明确「<c>309</c> 状态不对（可能已转到下一个状态）」⇒
/// 执行业务动作前应重新查询本详情，<see cref="ExpireTime"/> 为当前状态的剩余处理时限（<c>0</c> 表示不存在）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaComplaintOrder
{
    /// <summary>投诉单 id（<c>complaintOrderId</c>，后续四个商家动作端点的定位键）。</summary>
    [JsonPropertyName("complaintOrderId")]
    public string? ComplaintOrderId { get; set; }

    /// <summary>用户的 openid（<c>openid</c>，敏感标识，禁止入日志）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>投诉发起时间（<c>createTime</c>，秒级时间戳）。</summary>
    [JsonPropertyName("createTime")]
    public long? CreateTime { get; set; }

    /// <summary>联系方式（<c>phoneNumber</c>，官方类型 number；敏感信息，禁止入日志）。</summary>
    [JsonPropertyName("phoneNumber")]
    public long? PhoneNumber { get; set; }

    /// <summary>
    /// 投诉问题分类（<c>type</c>；官方枚举 611~679 与 610001~610005 两组，覆盖发货 / 客服 /
    /// 承诺未履行 / 商品 / 收费异常 / 押金余额 / 骑行卡 / 欺诈 / 扣费 / 短剧内容等问题族）。
    /// </summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>
    /// 投诉单状态（<c>status</c>；官方枚举分三段：1xx 平台客服处理中 / 已完结、2xx 商家与用户协商段、
    /// 3xx 平台判责与退货段）。其中待商家动作的取值为 <c>106</c> 待商家补充凭证、<c>108</c> 待双方补充凭证、
    /// <c>201</c> 待处理、<c>202</c> 商家超时未回应待用户确认、<c>203</c>/<c>204</c> 已回应待用户确认、
    /// <c>206</c> 平台已判定为商责待上传处理凭证、<c>308</c>/<c>309</c> 待用户退货中；
    /// 其余取值官方文案为「平台客服处理中」或「投诉已完结 / 已关闭」。
    /// </summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>投诉信息（<c>customerMaterial</c>），见 <see cref="WxaComplaintCustomerMaterial"/>。</summary>
    [JsonPropertyName("customerMaterial")]
    public WxaComplaintCustomerMaterial? CustomerMaterial { get; set; }

    /// <summary>微信支付订单号（<c>orderId</c>）。</summary>
    [JsonPropertyName("orderId")]
    public string? OrderId { get; set; }

    /// <summary>商家订单号（<c>outTradeNo</c>，即微信支付 APIv3 的 <c>out_trade_no</c>）。</summary>
    [JsonPropertyName("outTradeNo")]
    public string? OutTradeNo { get; set; }

    /// <summary>商品名称（<c>productName</c>）。</summary>
    [JsonPropertyName("productName")]
    public string? ProductName { get; set; }

    /// <summary>支付时间（<c>payTime</c>，秒级时间戳）。</summary>
    [JsonPropertyName("payTime")]
    public long? PayTime { get; set; }

    /// <summary>交易金额（<c>totalCost</c>，官方类型 string，原样承载不做单位换算）。</summary>
    [JsonPropertyName("totalCost")]
    public string? TotalCost { get; set; }

    /// <summary>投诉单当前状态到期时间（<c>expireTime</c>，官方原文「<c>0</c> 为不存在」）。</summary>
    [JsonPropertyName("expireTime")]
    public long? ExpireTime { get; set; }

    /// <summary>头像 URL（<c>headImgUrl</c>）。</summary>
    [JsonPropertyName("headImgUrl")]
    public string? HeadImgUrl { get; set; }

    /// <summary>微信昵称（<c>nickName</c>）。</summary>
    [JsonPropertyName("nickName")]
    public string? NickName { get; set; }

    /// <summary>
    /// 申诉状态（<c>appealState</c>；官方枚举：<c>0</c> 未进入申诉阶段 / <c>401</c> 待商家申诉 /
    /// <c>402</c> 超过申诉时间 / <c>403</c> 申诉中 / <c>117</c> 申诉成功 / <c>118</c> 申诉失败）。
    /// <c>401</c> 即为发起 <c>busiAppeal</c> 的窗口。
    /// </summary>
    [JsonPropertyName("appealState")]
    public long? AppealState { get; set; }
}

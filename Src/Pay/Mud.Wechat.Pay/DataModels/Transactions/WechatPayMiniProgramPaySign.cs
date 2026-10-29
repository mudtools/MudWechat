// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 小程序调起支付参数（<c>wx.requestPayment</c> 的入参）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791898"/>（小程序调起支付，
/// 2026-10-09 逐字段核验）。支持商户：<b>普通商户</b>。
/// </para>
/// <para>
/// <b>字段名是官方的驼峰写法，不得「规范化」</b>：<c>timeStamp</c> / <c>nonceStr</c> /
/// <c>package</c> / <c>signType</c> / <c>paySign</c> —— 小程序端按这些键名取值，
/// 改成 <c>time_stamp</c> 之类会直接调起失败。故本 DTO 每个属性都钉死 <c>JsonPropertyName</c>，
/// 宿主可直接序列化后下发给小程序端（已登记进 AOT 上下文 <c>TransactionsJsonContext</c>）。
/// </para>
/// <para>
/// <b>本类型不是 HTTP 应答</b>：它由 SDK <b>本地签名</b>得出（用商户私钥对
/// <c>appId\ntimeStamp\nnonceStr\npackage\n</c> 做 RSA-SHA256），不由网络返回 ——
/// 但仍落 DataModels 以便宿主 AOT 安全地序列化下发。
/// </para>
/// <para>
/// <b>时效</b>：<c>prepay_id</c> 自下单起 <b>2 小时</b>内有效，过期须重新下单。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class WechatPayMiniProgramPaySign
{
    /// <summary>时间戳（<c>timeStamp</c>，string(32)）：标准北京时间（东八区）的<b>秒级</b> Unix 时间戳（10 位数字）。</summary>
    [JsonPropertyName("timeStamp")]
    public string? TimeStamp { get; set; }

    /// <summary>随机字符串（<c>nonceStr</c>，string(32)）：不长于 32 位。</summary>
    [JsonPropertyName("nonceStr")]
    public string? NonceStr { get; set; }

    /// <summary>预支付交易会话标识（<c>package</c>，string(128)）：官方固定格式 <c>prepay_id={prepay_id}</c>。</summary>
    [JsonPropertyName("package")]
    public string? Package { get; set; }

    /// <summary>签名类型（<c>signType</c>，string(32)）：官方<b>仅支持</b> <c>RSA</c>。</summary>
    [JsonPropertyName("signType")]
    public string? SignType { get; set; }

    /// <summary>签名值（<c>paySign</c>，string(512)）：Base64 的 RSA-SHA256 签名。</summary>
    [JsonPropertyName("paySign")]
    public string? PaySign { get; set; }
}

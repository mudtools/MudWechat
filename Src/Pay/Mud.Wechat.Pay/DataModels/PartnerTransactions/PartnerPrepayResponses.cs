// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerTransactions;

/// <summary>
/// 服务商「仅返回 <c>prepay_id</c>」的下单应答（JSAPI / APP / 扫码支付三族共用；HTTP 200）。
/// </summary>
/// <remarks>
/// <para><b>共用依据</b>：官方三页应答表<b>逐项一致</b>（均只有 <c>prepay_id</c>，string(64)、2 小时有效）——
/// JSAPI <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714911"/>、
/// APP <see href="https://pay.weixin.qq.com/doc/v3/partner/4012714669"/>、
/// 扫码支付 <see href="https://pay.weixin.qq.com/doc/v3/partner/4012492476"/>（2026-10-14 逐页核验）。
/// 按本线纪律「<b>表相同则共用，表不同则分建</b>」不建三份必然漂移的同形类型，
/// 故类型名<b>不带产品族前缀</b>。</para>
/// <para><b>与直连域 <c>PrepayIdResponse</c> 不共用</b>：两者字段确实相同，但分属两条产品线、
/// 各自命名空间与序列化上下文，跨线共用会让 Pay 直连域的上下文登记被服务商面牵动。</para>
/// <para><b>继承 <see cref="WechatPayResponse"/></b>：下单失败是 4xx/5xx + <c>{"code","message"}</c>，
/// 继承后错误体原样落到 <c>Code</c>/<c>Message</c>；成功时 <c>code</c> 缺省。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerPrepayIdResponse : WechatPayResponse
{
    /// <summary>预支付交易会话标识（<c>prepay_id</c>，必填 string(64)，有效期 2 小时）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }
}

/// <summary>
/// 服务商 Native 下单应答（仅 <c>code_url</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714902"/>（2026-10-14 核验）。</para>
/// <para><b><c>code_url</c> 用途</b>：调用方须自行把它生成二维码展示给用户扫码，
/// <b>不得</b>对链接做任何拼接或改写；<b>失效后须重新调用下单接口</b>取新链接。</para>
/// <para><b>应答形态与 JSAPI 不同</b>（本族给链接、不给 <c>prepay_id</c>）⇒ 独立类型，不复用
/// <see cref="PartnerPrepayIdResponse"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerNativePrepayResponse : WechatPayResponse
{
    /// <summary>二维码链接（<c>code_url</c>，必填 string(64)）：用于生成支付二维码供用户扫码。</summary>
    [JsonPropertyName("code_url")]
    public string? CodeUrl { get; set; }
}

/// <summary>
/// 服务商 H5 下单应答（仅 <c>h5_url</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012714759"/>（2026-10-14 核验）。</para>
/// <para><b><c>h5_url</c> 是「拉起微信收银台的中间页 URL」</b>：须按官方 H5 调起支付指引原样跳转
/// （含 <c>Referer</c> 要求），<b>严禁</b>自行拼装或改写；链接有时效，失效须重新下单。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerTransactions")]
public class PartnerH5PrepayResponse : WechatPayResponse
{
    /// <summary>H5 支付跳转链接（<c>h5_url</c>，必填 string(256)）：须原样跳转，不得改写或拼装。</summary>
    [JsonPropertyName("h5_url")]
    public string? H5Url { get; set; }
}

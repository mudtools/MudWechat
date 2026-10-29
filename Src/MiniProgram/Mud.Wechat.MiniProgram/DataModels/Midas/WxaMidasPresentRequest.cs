// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Midas;

/// <summary>
/// 赠送游戏币请求体（<c>POST /cgi-bin/midas/present</c>）。
/// </summary>
/// <remarks>
/// <para><b>幂等键是 <c>bill_no</c></b>：相同订单号不会重复扣款（官方原文如此表述，赠送语义同理）；
/// 90012 = 订单已存在。</para>
/// <para><c>present_counts</c> 不能为 0。</para>
/// <para><b>签名不代算</b>：<c>sig</c> 由调用方按官方「签名计算算法」用米大师密钥做 HMAC-SHA256
/// （全部参数 + uri + 密钥）后传入；SDK 不持有米大师密钥，只做透传。签名原文的参数顺序与
/// 参数名拼写必须与官方算法完全一致，否则官方返回 90011。</para>
/// <para><b>敏感面</b>：<c>openid</c> / <c>sig</c> / <c>bill_no</c> / <c>user_ip</c> 一律禁入日志、
/// 遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Midas")]
public class WxaMidasPresentRequest
{
    /// <summary>用户唯一标识（必填）。敏感字段，禁入日志。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>小程序 appId（必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>米大师分配的 offer_id（必填）。</summary>
    [JsonPropertyName("offer_id")]
    public string? OfferId { get; set; }

    /// <summary>UNIX 时间戳，单位秒（必填）。</summary>
    [JsonPropertyName("ts")]
    public long? Timestamp { get; set; }

    /// <summary>游戏服务器大区 id（必填）；不分大区填 <c>"1"</c>；支持角色时角色 ID 接在分区 ID 后以 <c>"_"</c> 连接。</summary>
    [JsonPropertyName("zone_id")]
    public string? ZoneId { get; set; }

    /// <summary>平台（必填），官方当前只给 <c>android</c>，见 <see cref="WxaMidasPlatforms"/>。</summary>
    [JsonPropertyName("pf")]
    public string? Platform { get; set; }

    /// <summary>用户外网 IP（可选）。敏感字段，禁入日志。</summary>
    [JsonPropertyName("user_ip")]
    public string? UserIp { get; set; }

    /// <summary>订单号（必填）：全局唯一、长度 ≤63、仅 <c>[0-9A-Za-z_-]</c>；相同订单号不重复扣款。敏感字段，禁入日志。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>赠送游戏币的个数（必填，不能为 0）。</summary>
    [JsonPropertyName("present_counts")]
    public long? PresentCounts { get; set; }

    /// <summary>HMAC-SHA256 签名（必填，官方算法见 <c>virtual-payment#签名计算算法</c>）。</summary>
    [JsonPropertyName("sig")]
    public string? Sig { get; set; }
}

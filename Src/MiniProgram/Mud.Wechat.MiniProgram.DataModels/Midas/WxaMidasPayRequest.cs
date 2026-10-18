// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Midas;

/// <summary>
/// 扣除游戏币请求体（<c>POST /cgi-bin/midas/pay</c>）。
/// </summary>
/// <remarks>
/// <para><b>幂等键是 <c>bill_no</c></b>：官方明示接口超时或返回系统失败时可能已实际扣币，
/// 宿主须用<b>相同</b> <c>bill_no</c> 重试直至非系统失败，不会重复扣款；也可调取消支付接口撤销本次扣款。</para>
/// <para><b>金额单位是游戏币个数</b>（非分），<c>amt</c> 不能为 0；<c>used_gen_amt</c> 回包为本次扣掉的赠送币。</para>
/// <para><b>签名不代算</b>：<c>sig</c> 由调用方按官方「签名计算算法」用米大师密钥做 HMAC-SHA256
/// （全部参数 + uri + 密钥）后传入；SDK 不持有米大师密钥，只做透传。签名原文的参数顺序与
/// 参数名拼写必须与官方算法完全一致，否则官方返回 90011。</para>
/// <para><b>敏感面</b>：<c>openid</c> / <c>sig</c> / <c>bill_no</c> / <c>user_ip</c> 一律禁入日志、
/// 遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Midas")]
public class WxaMidasPayRequest
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

    /// <summary>扣除游戏币数量（必填，不能为 0）。</summary>
    [JsonPropertyName("amt")]
    public long? Amount { get; set; }

    /// <summary>订单号（必填）：全局唯一、长度 ≤63、仅 <c>[0-9A-Za-z_-]</c>；相同订单号不重复扣款。敏感字段，禁入日志。</summary>
    [JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }

    /// <summary>道具名称（可选）。</summary>
    [JsonPropertyName("pay_item")]
    public string? PayItem { get; set; }

    /// <summary>备注（可选），会写入账户流水。</summary>
    [JsonPropertyName("app_remark")]
    public string? AppRemark { get; set; }

    /// <summary>HMAC-SHA256 签名（必填，官方算法见 <c>virtual-payment#签名计算算法</c>）。</summary>
    [JsonPropertyName("sig")]
    public string? Sig { get; set; }
}

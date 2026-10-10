// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 获取支付签名请求体（<c>/cgi-bin/miniapppay/get_sign</c>，普通支付域）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPaySignRequest
{
    /// <summary>
    /// 获取或设置应用 ID（官方必填，二级商户申请的公众号或移动应用 appid）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置预支付交易会话标识（官方必填，1~128 个字符）：
    /// 小程序下单接口返回的 prepay_id 参数值。
    /// <para>官方业务限制：仅支持下单两小时内的 prepay_id。</para>
    /// </summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>
    /// 获取或设置签名方式（官方可选，1~32 个字符）：默认为 RSA，仅支持 RSA。
    /// </summary>
    [JsonPropertyName("sign_type")]
    public string? SignType { get; set; }

    /// <summary>
    /// 获取或设置随机字符串（官方必填，1~32 个字符）：
    /// 不长于 32 位，内容仅支持数字、大小写字母。
    /// </summary>
    [JsonPropertyName("nonce")]
    public string? Nonce { get; set; }

    /// <summary>
    /// 获取或设置时间戳（官方必填）：当前的秒级时间戳。
    /// </summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 同步消息请求体（<c>/cgi-bin/kf/sync_msg</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Token"/> 取自「微信客服事件通知」回调下发的同步令牌，与 <c>access_token</c>
/// 是两个不同概念的令牌，勿混用；官方标注该令牌「10 分钟内有效；<b>可不填，如果不填接口有严格的频率限制</b>」，
/// 故正确用法是<b>收到回调事件后尽快带上该 token 拉取</b>，而非留空走裸轮询。
/// </para>
/// <para>
/// 须配合 <see cref="Cursor"/> 顺序翻页，翻页终点只以响应的 <c>has_more</c> 判定
/// （官方明令不得以 <c>msg_list</c> 是否为空判定，可能出现 has_more=1 而 msg_list 为空）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class SyncKfMessageRequest
{
    /// <summary>
    /// 获取或设置回调事件下发的同步令牌（官方<b>选填</b>，不多于 128 字节；不填时官方有严格的频率限制）。
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// 获取或设置翻页标记（官方选填，不多于 64 字节；第一次拉取可以不填，不填则从 3 天内最早的消息开始返回，
    /// 之后填入上一次响应的 next_cursor）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置期望请求的数据量（官方选填，<b>默认值和最大值都为 1000</b>；官方提示实际返回条数可能少于该值，
    /// 须结合 has_more 判断是否继续）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    /// 获取或设置语音消息的返回格式（官方选填，0-Amr、1-Silk，默认 0）。
    /// </summary>
    [JsonPropertyName("voice_format")]
    public int? VoiceFormat { get; set; }

    /// <summary>
    /// 获取或设置客服账号 ID（官方<b>必填</b>，指定拉取某个客服账号的消息）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string OpenKfId { get; set; } = string.Empty;
}

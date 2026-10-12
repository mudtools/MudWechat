// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.ShortLink;

/// <summary>
/// 长信息转短链（<c>POST /cgi-bin/shorten/gen</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>long_data</c>（必填，<b>不超过 4KB</b>）/
/// <c>expire_seconds</c>（最大值为 <b>2592000</b> 即 30 天，默认 2592000）。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>expire_seconds</c> 字段表标「必填」但又给出「默认值 2592000」
/// ⇒ SDK 以可空承载（不传即由官方取默认值）；SDK <b>不做本地长度/时长拦截</b>
/// （越界由官方 <c>9410010</c>/<c>9410011</c> 表达）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShortLink")]
public class MpShortenGenRequest
{
    /// <summary>获取或设置需要转换的长信息（官方 <c>long_data</c>，必填，不超过 4KB）。</summary>
    [JsonPropertyName("long_data")]
    public string LongData { get; set; } = string.Empty;

    /// <summary>获取或设置过期秒数（官方 <c>expire_seconds</c>，最大 2592000 即 30 天；缺省由官方取默认值 2592000）。</summary>
    [JsonPropertyName("expire_seconds")]
    public int? ExpireSeconds { get; set; }
}

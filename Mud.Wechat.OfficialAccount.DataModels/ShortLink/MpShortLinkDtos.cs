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

/// <summary>
/// 长信息转短链（<c>POST /cgi-bin/shorten/gen</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>short_key</c>（<b>15 字节，base62 编码</b>（0-9/a-z/A-Z））。
/// </para>
/// <para>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>44002</c>（empty post data）/ <c>47001</c>（data format error）/
/// <c>47003</c>（参数错误，没有传入 <c>long_data</c>）/ <c>9410010</c>（long_data 长度超限）/
/// <c>9410011</c>（expire_seconds 超限）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShortLink")]
public class MpShortenGenResponse : MpResponse
{
    /// <summary>获取或设置短 key（官方 <c>short_key</c>，15 字节 base62）。</summary>
    [JsonPropertyName("short_key")]
    public string? ShortKey { get; set; }
}

/// <summary>
/// 短链转长信息（<c>POST /cgi-bin/shorten/fetch</c>）请求体。
/// </summary>
/// <remarks>官方契约：<c>short_key</c> 必填。</remarks>
[HttpJsonSerializable(SerializerClassName = "ShortLink")]
public class MpShortenFetchRequest
{
    /// <summary>获取或设置短 key（官方 <c>short_key</c>，必填）。</summary>
    [JsonPropertyName("short_key")]
    public string ShortKey { get; set; } = string.Empty;
}

/// <summary>
/// 短链转长信息（<c>POST /cgi-bin/shorten/fetch</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>long_data</c>（长信息）/ <c>create_time</c>（创建的时间戳）/
/// <c>expire_seconds</c>（<b>剩余的</b>过期秒数）。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>44002</c> / <c>47001</c> / <c>47003</c> /
/// <c>9410012</c>（short_key 不存在 / 已过期 / 不属于本账号）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShortLink")]
public class MpShortenFetchResponse : MpResponse
{
    /// <summary>获取或设置长信息（官方 <c>long_data</c>）。</summary>
    [JsonPropertyName("long_data")]
    public string? LongData { get; set; }

    /// <summary>获取或设置创建的时间戳（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置剩余的过期秒数（官方 <c>expire_seconds</c>；官方原文为「剩余的过期秒数」）。</summary>
    [JsonPropertyName("expire_seconds")]
    public int? ExpireSeconds { get; set; }
}

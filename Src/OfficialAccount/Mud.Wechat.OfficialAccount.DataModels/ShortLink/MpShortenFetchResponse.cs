// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.ShortLink;

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

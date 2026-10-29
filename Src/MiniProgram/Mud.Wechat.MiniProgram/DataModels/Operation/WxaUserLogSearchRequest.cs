// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>查询实时日志请求体（<c>POST /wxaapi/userlog/userlog_search</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>operation/api_realtimelogsearch.html</c>。</para>
/// <para><c>date</c> / <c>begintime</c> / <c>endtime</c> / <c>start</c> / <c>limit</c> 必填；
/// <c>traceId</c>、<c>uin</c> 选填（按需过滤）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaUserLogSearchRequest
{
    /// <summary>日期（<c>date</c>，必填；格式 <c>yyyyMMdd</c>）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>起始时间（<c>begintime</c>，必填；格式 <c>HH:mm:ss</c>）。</summary>
    [JsonPropertyName("begintime")]
    public string? BeginTime { get; set; }

    /// <summary>结束时间（<c>endtime</c>，必填；格式 <c>HH:mm:ss</c>）。</summary>
    [JsonPropertyName("endtime")]
    public string? EndTime { get; set; }

    /// <summary>起始偏移（<c>start</c>，必填）。</summary>
    [JsonPropertyName("start")]
    public long? Start { get; set; }

    /// <summary>查询条数（<c>limit</c>，必填；上限以官方页面为准）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>链路 ID（<c>traceId</c>，选填）。</summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>用户标识（<c>uin</c>，选填）。</summary>
    [JsonPropertyName("uin")]
    public string? Uin { get; set; }
}

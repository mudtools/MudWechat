// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 获取收件箱邮件列表请求体（<c>/cgi-bin/exmail/app/get_mail_list</c>）。
/// <para>分页获取应用收件箱下指定时间范围内的邮件 id 列表（cursor + has_more 翻页拉取）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class GetMailListRequest
{
    /// <summary>获取或设置开始时间，Unix 时间戳（官方必填）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置结束时间，Unix 时间戳（官方必填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置上一次调用时返回的 next_cursor（官方选填；第一次拉取可以不填）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>获取或设置期望请求的数据量（官方选填；默认值为 100，最大值为 1000）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}

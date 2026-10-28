// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议录制文件对象（获取会议录制列表响应 <c>record_file_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingRecordFile
{
    /// <summary>获取或设置录制文件 ID。</summary>
    [JsonPropertyName("record_file_id")]
    public string? RecordFileId { get; set; }

    /// <summary>获取或设置开始录制时间（UNIX 时间戳，单位毫秒）。</summary>
    [JsonPropertyName("record_start_time")]
    public long? RecordStartTime { get; set; }

    /// <summary>获取或设置结束录制时间（UNIX 时间戳，单位毫秒）。</summary>
    [JsonPropertyName("record_end_time")]
    public long? RecordEndTime { get; set; }

    /// <summary>获取或设置文件大小（单位字节）。</summary>
    [JsonPropertyName("record_size")]
    public int? RecordSize { get; set; }

    /// <summary>获取或设置共享状态（是否开启共享）：0 - 未开启；1 - 开启（开启共享时返回访问权限、访问密码、共享链接有效期、是否允许下载）。</summary>
    [JsonPropertyName("sharing_state")]
    public int? SharingState { get; set; }

    /// <summary>获取或设置共享链接（开启共享时返回）。</summary>
    [JsonPropertyName("sharing_url")]
    public string? SharingUrl { get; set; }

    /// <summary>获取或设置是否仅企业成员可查看（开启共享时返回）。</summary>
    [JsonPropertyName("required_same_corp")]
    public bool? RequiredSameCorp { get; set; }

    /// <summary>获取或设置是否仅参会成员可查看（开启共享时返回）。</summary>
    [JsonPropertyName("required_attendee")]
    public bool? RequiredAttendee { get; set; }

    /// <summary>获取或设置访问密码（开启共享时返回）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>获取或设置共享链接有效期（单位毫秒；当未开启共享时，返回 0 表示永久有效；开启共享时返回）。</summary>
    [JsonPropertyName("sharing_expire")]
    public long? SharingExpire { get; set; }

    /// <summary>获取或设置是否允许下载（开启共享时返回）。</summary>
    [JsonPropertyName("allow_download")]
    public bool? AllowDownload { get; set; }
}

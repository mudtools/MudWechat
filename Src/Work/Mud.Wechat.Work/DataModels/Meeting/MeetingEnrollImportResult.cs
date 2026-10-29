// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 导入会议报名信息响应条目对象（导入会议报名信息响应 <c>enroll_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingEnrollImportResult
{
    /// <summary>获取或设置报名 ID。</summary>
    [JsonPropertyName("enroll_id")]
    public string? EnrollId { get; set; }

    /// <summary>获取或设置报名成员 userid。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置国家/地区代码（例如：中国传 86，不是 +86）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>获取或设置手机号。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>获取或设置报名的昵称。</summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }

    /// <summary>获取或设置报名码（用于电话入会的凭证）。</summary>
    [JsonPropertyName("enroll_code")]
    public string? EnrollCode { get; set; }
}

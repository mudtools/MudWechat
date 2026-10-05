// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议嘉宾对象（创建预约会议请求 <c>guests</c> 元素、获取/更新会议嘉宾列表与获取会议详情响应 <c>guests</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingGuest
{
    /// <summary>获取或设置国家/地区代码（例如：中国传 86，不是 +86，也不是 0086）。</summary>
    [JsonPropertyName("area")]
    public string? Area { get; set; }

    /// <summary>获取或设置手机号。</summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    /// <summary>获取或设置会议嘉宾姓名（1~16 位字符长度）。</summary>
    [JsonPropertyName("guest_name")]
    public string? GuestName { get; set; }
}

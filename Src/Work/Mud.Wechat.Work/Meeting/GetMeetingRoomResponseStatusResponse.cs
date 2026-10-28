// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取 Rooms 会议室应答状态响应体（<c>/cgi-bin/meeting/rooms/get_response_status</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingRoomResponseStatusResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置应答状态：0 - 无应答（60 秒无回应）；1 - 未呼叫；2 - 入会中；3 - 被拒绝；4 - 呼叫中；
    /// 5 - 取消呼叫（仅 Rooms 会议室有该状态）；6 - 已离会。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置最近一次应答时间。</summary>
    [JsonPropertyName("response_time")]
    public string? ResponseTime { get; set; }
}

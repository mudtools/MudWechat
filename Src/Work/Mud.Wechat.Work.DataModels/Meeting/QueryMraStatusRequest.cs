// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取 MRA 状态信息请求体（<c>/cgi-bin/meeting/mra/query_status</c>；对 API 创建的会议获取指定 MRA 设备的当前状态信息，
/// 包括名称、静音状态、视频状态、举手状态、默认布局等。会议室连接器支持企业通过 SIP 或 H.323 会议室加入会议，
/// 实现传统会议硬件与企业微信会议的互通——通过会议号加入）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class QueryMraStatusRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置被查询成员临时身份 ID（官方必填）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }
}

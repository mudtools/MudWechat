// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 会议室 PMI 信息对象（获取 Rooms 会议室详情响应 <c>pmi_info</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsPmiInfo
{
    /// <summary>获取或设置 Rooms 会议室专属 ID。</summary>
    [JsonPropertyName("pmi_code")]
    public string? PmiCode { get; set; }

    /// <summary>获取或设置入会密码（base64 编码）。</summary>
    [JsonPropertyName("pmi_pwd")]
    public string? PmiPwd { get; set; }
}

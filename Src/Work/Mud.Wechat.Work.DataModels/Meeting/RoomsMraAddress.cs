// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// MRA（会议室连接器）信令地址对象（呼叫/取消呼叫/获取应答状态请求 <c>mra_address</c> 嵌套对象；
/// 与 <c>meeting_room_id</c> 二选一使用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsMraAddress
{
    /// <summary>获取或设置信令协议（官方必填）：1 - SIP；2 - H.323。</summary>
    [JsonPropertyName("protocol")]
    public int? Protocol { get; set; }

    /// <summary>
    /// 获取或设置信令地址（官方必填）。
    /// <para>如果是 H.323 类型，输入 IP 地址或 E.164 号码；如果是 SIP 类型，输入 IP 地址或 URI。</para>
    /// </summary>
    [JsonPropertyName("dial_string")]
    public string? DialString { get; set; }
}

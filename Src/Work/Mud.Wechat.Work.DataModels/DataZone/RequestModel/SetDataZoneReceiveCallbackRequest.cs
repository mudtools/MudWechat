// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 设置专区接收回调事件请求体（<c>/cgi-bin/chatdata/set_receive_callback</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class SetDataZoneReceiveCallbackRequest
{
    /// <summary>
    /// 获取或设置应用关联的程序 id（官方必填）。
    /// <para>同一个应用只能设置一个程序接收；若先设置了程序 A 接收，再调用本接口设置程序 B 时，会更改为程序 B 接收。</para>
    /// </summary>
    [JsonPropertyName("program_id")]
    public string? ProgramId { get; set; }
}

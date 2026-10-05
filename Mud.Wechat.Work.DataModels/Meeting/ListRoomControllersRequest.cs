// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取控制器列表请求体（<c>/cgi-bin/meeting/rooms/list_controllers</c>；获取企业下的控制器列表）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class ListRoomControllersRequest
{
    /// <summary>获取或设置需要获取的控制器名称（支持模糊匹配查找）。</summary>
    [JsonPropertyName("controller_name")]
    public string? ControllerName { get; set; }

    /// <summary>获取或设置分页查询游标（将上一个请求返回的 <c>next_cursor</c> 字段传入；第一次查询时可不传值）。</summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置分页大小（从 1 开始，最大 50，默认 20）。
    /// <para>官方限制：<c>limit</c> 参数必须与首次调用获得 <c>cursor</c> 时传入的 limit 一致。</para>
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

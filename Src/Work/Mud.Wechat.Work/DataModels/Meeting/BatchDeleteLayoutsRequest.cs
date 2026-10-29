// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 批量删除布局请求体（<c>/cgi-bin/meeting/advanced_layout/batch_delete</c>；根据布局 ID 批量删除布局，可以删除基础布局和高级布局）。
/// </summary>
/// <remarks>
/// <para>官方限制：正在被应用的布局无法删除，请先设置成其他布局或恢复成默认原始布局后再行删除；
/// 接口不做布局是否存在的校验，删除不存在的布局不会有提示；最多支持 20 个布局 ID；高级布局目前仅支持 H.323/SIP 会议室终端。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class BatchDeleteLayoutsRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置布局 ID 列表（官方必填，最多支持 20 个；当前正在应用的布局不能被删除）。</summary>
    [JsonPropertyName("layout_id_list")]
    public List<string>? LayoutIdList { get; set; }
}

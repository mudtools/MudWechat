// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 设置高级布局请求体（<c>/cgi-bin/meeting/advanced_layout/apply</c>；
/// 将会议中的高级自定义布局应用到指定成员或者整个会议，也可以恢复指定成员或整个会议的默认布局）。
/// </summary>
/// <remarks>
/// <para>官方限制：高级布局应用到指定成员目前仅支持 H.323/SIP 会议室终端；
/// 应用布局的优先级从高到低为：个性布局 &gt; 自定义布局 &gt; 默认布局（MRA 不支持同框模式，如果会议设置为同框模式，MRA 应用默认布局）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class ApplyAdvancedLayoutRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置选择应用的布局 ID（官方必填；若传空 ""，表示恢复成当前会议的默认布局）。
    /// </summary>
    [JsonPropertyName("layout_id")]
    public string? LayoutId { get; set; }

    /// <summary>
    /// 获取或设置用户列表对象数组（详见 <see cref="LayoutApplyUser"/>）。
    /// <para>官方限制：如果该字段为空，为会议设置高级自定义布局；如果该字段携带用户，则只为指定用户设置个性布局；单次最多支持 20 个用户。</para>
    /// </summary>
    [JsonPropertyName("user_list")]
    public List<LayoutApplyUser>? UserList { get; set; }
}

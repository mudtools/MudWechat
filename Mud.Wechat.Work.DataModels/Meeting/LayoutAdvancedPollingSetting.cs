// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 高级布局轮询参数设置对象（添加/修改会议高级布局与获取布局列表/用户布局响应 <c>polling_setting</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class LayoutAdvancedPollingSetting
{
    /// <summary>获取或设置轮询间隔时间类型（官方必填）：1 - 秒；2 - 分钟。</summary>
    [JsonPropertyName("polling_interval_unit")]
    public int? PollingIntervalUnit { get; set; }

    /// <summary>获取或设置轮询间隔时长（官方必填，允许取值范围 1~999999）。</summary>
    [JsonPropertyName("polling_interval")]
    public int? PollingInterval { get; set; }

    /// <summary>获取或设置是否忽略没开启视频成员（官方必填）。</summary>
    [JsonPropertyName("ignore_user_novideo")]
    public bool? IgnoreUserNovideo { get; set; }

    /// <summary>获取或设置是否忽略未入会成员（官方必填）。</summary>
    [JsonPropertyName("ignore_user_absence")]
    public bool? IgnoreUserAbsence { get; set; }
}

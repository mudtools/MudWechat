// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议高级布局单页对象（添加/修改会议高级布局请求与获取布局列表/用户布局响应 <c>page_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class LayoutAdvancedPage
{
    /// <summary>获取或设置布局模板 ID（官方必填）。</summary>
    [JsonPropertyName("layout_template_id")]
    public string? LayoutTemplateId { get; set; }

    /// <summary>获取或设置开启或关闭轮询（默认关闭）：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("enable_polling")]
    public bool? EnablePolling { get; set; }

    /// <summary>获取或设置轮询参数设置对象（详见 <see cref="LayoutAdvancedPollingSetting"/>）。</summary>
    [JsonPropertyName("polling_setting")]
    public LayoutAdvancedPollingSetting? PollingSetting { get; set; }

    /// <summary>获取或设置用户座次对象列表（详见 <see cref="LayoutAdvancedSeat"/>）。</summary>
    [JsonPropertyName("user_seat_list")]
    public List<LayoutAdvancedSeat>? UserSeatList { get; set; }
}

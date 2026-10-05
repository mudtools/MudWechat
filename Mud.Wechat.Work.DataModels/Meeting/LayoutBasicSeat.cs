// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议基础布局座次对象（添加/修改会议基础布局请求与添加响应 <c>user_seat_list</c> 嵌套对象；
/// 与高级布局座次 <see cref="LayoutAdvancedSeat"/> 结构不同构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class LayoutBasicSeat
{
    /// <summary>获取或设置宫格 ID（官方必填；若多次传入同一宫格 ID 的对象，仅第一次出现的对象生效）。</summary>
    [JsonPropertyName("grid_id")]
    public string? GridId { get; set; }

    /// <summary>
    /// 获取或设置宫格类型（官方必填）：1 - 视频画面；2 - 共享画面；3 - 拓展应用（目前一页仅可添加一个应用）。
    /// <para>官方限制：添加的应用需满足以下条件——与会议绑定、开启网页服务、同企业下的仅企业内可见应用或外部企业可见应用。</para>
    /// </summary>
    [JsonPropertyName("grid_type")]
    public int? GridType { get; set; }

    /// <summary>获取或设置当场会议的企业成员的 userid（userid / tmp_openid / tool_sdkid 三者填其一）。</summary>
    [JsonPropertyName("userid")]
    public string? Userid { get; set; }

    /// <summary>获取或设置当场会议的用户临时 ID（userid / tmp_openid / tool_sdkid 三者填其一）。</summary>
    [JsonPropertyName("tmp_openid")]
    public string? TmpOpenid { get; set; }

    /// <summary>获取或设置昵称（当宫格类型 <see cref="GridType"/> 为 1 时必填，作为视频画面展示）。</summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }

    /// <summary>获取或设置拓展应用 ID（userid / tmp_openid / tool_sdkid 三者填其一）。</summary>
    [JsonPropertyName("tool_sdkid")]
    public string? ToolSdkid { get; set; }
}

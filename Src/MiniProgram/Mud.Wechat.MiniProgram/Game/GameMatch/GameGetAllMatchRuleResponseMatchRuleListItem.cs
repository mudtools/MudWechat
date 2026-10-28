// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>对局匹配规则项。官方返回表把 need_room_service_info 那行的名字误写成第二个 team_member_count ⇒ 按返回示例的真实字段名建模。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.getAllMatchRule.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetAllMatchRuleResponseMatchRuleListItem
{
    /// <summary>matchid，通过 createMatchRule 后台接口创建。</summary>
    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    /// <summary>创建 matchid 的小程序 appid。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>matchid 的打开状态，取值见 setMatchIdOpenState（0 关闭 / 1 打开）。</summary>
    [JsonPropertyName("open_state")]
    public int? OpenState { get; set; }

    /// <summary>匹配结果中的队伍数量。</summary>
    [JsonPropertyName("team_count")]
    public int? TeamCount { get; set; }

    /// <summary>匹配结果每个队伍对应的成员数量。</summary>
    [JsonPropertyName("team_member_count")]
    public int? TeamMemberCount { get; set; }

    /// <summary>是否在下发匹配结果时同步创建帧同步房间：0 不需要 / 1 需要（官方表格此行名字误写为 team_member_count，返回示例名为 need_room_service_info）。</summary>
    [JsonPropertyName("need_room_service_info")]
    public int? NeedRoomServiceInfo { get; set; }

    /// <summary>创建帧同步房间时的房间信息。</summary>
    [JsonPropertyName("game_room_info")]
    public GameMatchRoomInfo? GameRoomInfo { get; set; }
}

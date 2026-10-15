// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>创建对局匹配规则请求。每个小游戏持有的 matchid 上限 20（超限官方回 500014）。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.createMatchRule.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameCreateMatchRuleRequest
{
    /// <summary>匹配结果中的队伍数量（官方必填）。</summary>
    [JsonPropertyName("team_count")]
    public int TeamCount { get; set; }

    /// <summary>匹配结果每个队伍对应的成员数量（官方必填）。</summary>
    [JsonPropertyName("team_member_count")]
    public int TeamMemberCount { get; set; }

    /// <summary>是否在下发匹配结果时同步创建帧同步房间：0 不需要 / 1 需要；填 1 则返回 accessInfo，要求 team_count × team_member_count 不超过 10 人。</summary>
    [JsonPropertyName("need_room_service_info")]
    public int? NeedRoomServiceInfo { get; set; }

    /// <summary>创建帧同步房间时的房间信息，仅在 need_room_service_info 为 1 时有效。</summary>
    [JsonPropertyName("game_room_info")]
    public GameMatchRoomInfo? GameRoomInfo { get; set; }
}

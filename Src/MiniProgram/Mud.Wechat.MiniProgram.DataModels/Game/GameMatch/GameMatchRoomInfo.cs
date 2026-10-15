// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>匹配规则关联的帧同步房间信息。创建请求与 getAllMatchRule 返回项的 game_room_info 子表逐字段相同 ⇒ 共用一支类型。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.createMatchRule.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameMatchRoomInfo
{
    /// <summary>后台下发游戏逻辑帧的周期，单位 ms，最小不得小于 33ms（创建时官方必填）。</summary>
    [JsonPropertyName("game_tick")]
    public int? GameTick { get; set; }

    /// <summary>不填或 0 代表收到一次「开始游戏」即开始，否则须达到该百分比（创建时官方必填）。</summary>
    [JsonPropertyName("start_percent")]
    public int? StartPercent { get; set; }

    /// <summary>UDP 可靠性策略，N 固定冗余 N 帧，官方建议 3（创建时官方必填）。</summary>
    [JsonPropertyName("udp_reliability_strategy")]
    public int? UdpReliabilityStrategy { get; set; }

    /// <summary>房间是否需要用户的头像昵称。</summary>
    [JsonPropertyName("need_user_info")]
    public bool? NeedUserInfo { get; set; }

    /// <summary>游戏对局时长，单位 s，上限 1 小时；不填或 0 默认 20 分钟。</summary>
    [JsonPropertyName("game_last_time")]
    public int? GameLastTime { get; set; }

    /// <summary>是否需要在创建房间时同时生成一个随机的游戏种子。</summary>
    [JsonPropertyName("need_game_seed")]
    public bool? NeedGameSeed { get; set; }
}

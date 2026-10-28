// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>创建帧同步游戏房间请求。官方必填四支为 game_tick / open_id_list / udp_reliability_strategy / start_percent。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.createGameRoom.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameCreateRoomRequest
{
    /// <summary>后台下发游戏逻辑帧的周期，单位 ms，最小不得小于 33ms（官方必填）。</summary>
    [JsonPropertyName("game_tick")]
    public int GameTick { get; set; }

    /// <summary>房间成员的 OpenId 列表（官方必填）。</summary>
    [JsonPropertyName("open_id_list")]
    public List<string>? OpenIdList { get; set; }

    /// <summary>UDP 可靠性策略，N 代表固定冗余 N 帧，官方建议 3（必填）。</summary>
    [JsonPropertyName("udp_reliability_strategy")]
    public int UdpReliabilityStrategy { get; set; }

    /// <summary>不填或 0 代表收到一次「开始游戏」指令即开始；否则须达到该百分比（官方必填）。</summary>
    [JsonPropertyName("start_percent")]
    public int StartPercent { get; set; }

    /// <summary>房间是否需要用户头像昵称：true 时微信后台校验授权，未授权则加入房间请求失败；false 时加入成功但不下发头像昵称。</summary>
    [JsonPropertyName("need_user_info")]
    public bool? NeedUserInfo { get; set; }

    /// <summary>游戏对局时长，单位 s，不得超过后台上限 1 小时；不填或 0 默认 20 分钟。</summary>
    [JsonPropertyName("game_last_time")]
    public int? GameLastTime { get; set; }

    /// <summary>第三方自定义的房间信息，不得超过 32 个字节（超长官方回 4013）。</summary>
    [JsonPropertyName("room_ext_info")]
    public string? RoomExtInfo { get; set; }

    /// <summary>是否需要在创建房间时同时生成一个随机的游戏种子。</summary>
    [JsonPropertyName("need_game_seed")]
    public bool? NeedGameSeed { get; set; }
}

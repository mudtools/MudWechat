// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>获取指定房间信息·data 对象（本域最密集的返回体）。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameRoomInfo.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetRoomInfoResponseData
{
    /// <summary>房间 ID。</summary>
    [JsonPropertyName("room_id_str")]
    public string? RoomIdStr { get; set; }

    /// <summary>1 组队中 / 2 对局已开始 / 3 对局已结束 / 4 房间已销毁。</summary>
    [JsonPropertyName("room_state")]
    public int? RoomState { get; set; }

    /// <summary>房间最大可容纳人数。</summary>
    [JsonPropertyName("max_member_num")]
    public int? MaxMemberNum { get; set; }

    /// <summary>创建时间。</summary>
    [JsonPropertyName("create_timestamp")]
    public long? CreateTimestamp { get; set; }

    /// <summary>最近更新时间。</summary>
    [JsonPropertyName("update_timestamp")]
    public long? UpdateTimestamp { get; set; }

    /// <summary>游戏下发帧的时间间隔，单位 ms。</summary>
    [JsonPropertyName("game_tick")]
    public int? GameTick { get; set; }

    /// <summary>真正开始帧同步需要多少百分比的玩家发送了开始指令，填 50 表征 50%。</summary>
    [JsonPropertyName("start_percent")]
    public int? StartPercent { get; set; }

    /// <summary>游戏对局时长，单位 s。</summary>
    [JsonPropertyName("game_last_time")]
    public int? GameLastTime { get; set; }

    /// <summary>第三方自定义的游戏版本号。</summary>
    [JsonPropertyName("game_version")]
    public int? GameVersion { get; set; }

    /// <summary>该房间对应的游戏的 access_info。</summary>
    [JsonPropertyName("game_access_info")]
    public string? GameAccessInfo { get; set; }

    /// <summary>UDP 可靠性策略，0 全冗余 / N 固定冗余 N 帧。</summary>
    [JsonPropertyName("udp_reliability_strategy")]
    public int? UdpReliabilityStrategy { get; set; }

    /// <summary>给第三方用的 buffer，最长 32 个字节。</summary>
    [JsonPropertyName("room_ext_info")]
    public string? RoomExtInfo { get; set; }

    /// <summary>游戏随机种子。</summary>
    [JsonPropertyName("seed")]
    public string? Seed { get; set; }

    /// <summary>成员列表。</summary>
    [JsonPropertyName("member_list")]
    public List<GameGetRoomInfoResponseDataMemberListItem>? MemberList { get; set; }
}

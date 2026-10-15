// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>房间成员项。headimg / nickname 仅在用户授权时下发。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameRoomInfo.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetRoomInfoResponseDataMemberListItem
{
    /// <summary>玩家准备状态。</summary>
    [JsonPropertyName("is_ready")]
    public bool? IsReady { get; set; }

    /// <summary>0 普通成员 / 1 房主。</summary>
    [JsonPropertyName("role")]
    public int? Role { get; set; }

    /// <summary>座位号，从 0 开始。</summary>
    [JsonPropertyName("pos_num")]
    public int? PosNum { get; set; }

    /// <summary>头像 URL（用户授权才会返回）。</summary>
    [JsonPropertyName("headimg")]
    public string? Headimg { get; set; }

    /// <summary>用户昵称（用户授权才会返回）。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>用户在房间内的唯一标识。</summary>
    [JsonPropertyName("client_id")]
    public int? ClientId { get; set; }

    /// <summary>是否已做好游戏开始准备。</summary>
    [JsonPropertyName("enable_to_start")]
    public bool? EnableToStart { get; set; }

    /// <summary>给第三方用的 buffer，最长 32 个字节。</summary>
    [JsonPropertyName("member_ext_info")]
    public string? MemberExtInfo { get; set; }
}

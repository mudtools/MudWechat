// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>对局玩家位次项。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameIdentityInfo.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetIdentityInfoResponseDataInfoListItem
{
    /// <summary>玩家在房间里的唯一性标识 id，不同于玩家座位号，一旦分配不能修改。</summary>
    [JsonPropertyName("client_id")]
    public int? ClientId { get; set; }

    /// <summary>玩家 OpenId。</summary>
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }
}

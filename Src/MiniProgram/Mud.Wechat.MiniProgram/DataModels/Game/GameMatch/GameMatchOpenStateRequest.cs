// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>对局匹配规则状态请求。setMatchIdOpenState 与 updateMatchRule 两页官方参数表逐字段相同（只有 match_id + open_state）⇒ 共用一支请求类型。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/gamematch/gamematch.setMatchIdOpenState.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameMatchOpenStateRequest
{
    /// <summary>需要变更的 matchid，通过 createMatchRule 后台接口创建。官方类型列标 number 但请求示例值是字符串 ⇒ 按实际线格式取 string。</summary>
    [JsonPropertyName("match_id")]
    public string? MatchId { get; set; }

    /// <summary>matchid 的打开状态：0 关闭 / 1 打开（非法值官方回 500012）。</summary>
    [JsonPropertyName("open_state")]
    public int OpenState { get; set; }
}

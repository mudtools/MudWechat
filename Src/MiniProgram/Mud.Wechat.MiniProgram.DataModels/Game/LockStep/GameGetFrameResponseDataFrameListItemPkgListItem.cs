// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>指令包项。<c>action_list</c> 官方类型列仅标 array、未给元素类型 ⇒ 以 <c>List&lt;JsonElement&gt;</c> 承载，不做类型推断。官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/lock-step/lock-step.getGameFrame.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameGetFrameResponseDataFrameListItemPkgListItem
{
    /// <summary>该包发送者的 OpenId。</summary>
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }

    /// <summary>第三方自定义游戏指令数组（官方类型列只写 array，元素结构由游戏自身决定）。</summary>
    [JsonPropertyName("action_list")]
    public List<JsonElement>? ActionList { get; set; }

    /// <summary>true 代表是空帧（官方字段名就是一个字母 b）。</summary>
    [JsonPropertyName("b")]
    public bool? B { get; set; }

    /// <summary>微信内部使用，官方原文「无需理解，为了更快速返回数据没有去掉该字段内容」。</summary>
    [JsonPropertyName("dp_id")]
    public long? DpId { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>文本内容安全识别响应。官方返回表未列 detail[].level，但返回示例中出现 ⇒ 按参数表建模、不凭空补字段。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.msgSecCheck.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameMsgSecCheckResponse : WxaResponse
{
    /// <summary>唯一请求标识，标记单次请求。</summary>
    [JsonPropertyName("trace_id")]
    public string? TraceId { get; set; }

    /// <summary>综合结果。</summary>
    [JsonPropertyName("result")]
    public GameMsgSecCheckResult? Result { get; set; }

    /// <summary>详细检测结果。</summary>
    [JsonPropertyName("detail")]
    public List<GameMsgSecCheckDetailItem>? Detail { get; set; }
}

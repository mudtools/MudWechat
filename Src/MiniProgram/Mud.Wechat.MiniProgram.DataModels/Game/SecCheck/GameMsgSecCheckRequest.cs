// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>文本内容安全识别请求（gameMsgSecCheck）。version 官方固定 2，scene 为 1~5 枚举（错误 40129 / 750030）。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.msgSecCheck.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameMsgSecCheckRequest
{
    /// <summary>用户的 openid（官方必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>接口版本号，官方固定值 2（填错回 750030）。</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>场景枚举：1 资料 / 2 评论 / 3 论坛 / 4 社交日志 / 5 聊天（官方必填）。</summary>
    [JsonPropertyName("scene")]
    public int Scene { get; set; }

    /// <summary>需检测的文本内容，上限 2500 字，须 UTF-8 编码（官方必填）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>用户昵称，须 UTF-8 编码。</summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.MiniProgram.DataModels.Game;

/// <summary>图片内容安全识别请求（gamesecurity.mediaCheck，路由是 media_check_sync）。media_type 官方固定填 2 图片。 官方页：<see href="https://developers.weixin.qq.com/minigame/dev/api-backend/open-api/wxa-sec-check/gamesecurity.mediaCheck.html"/></summary>
[HttpJsonSerializable(SerializerClassName = "Game")]
public class GameMediaCheckRequest
{
    /// <summary>用户的 openid（官方必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>接口版本号，官方固定值 2。</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>场景枚举：1 资料 / 2 评论 / 3 论坛 / 4 社交日志 / 5 聊天（官方必填）。</summary>
    [JsonPropertyName("scene")]
    public int Scene { get; set; }

    /// <summary>检测的图片 url，支持 jpg/jpeg/png/bmp/tif/tiff/webp/gif（须能被检测服务器下载）。</summary>
    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; set; }

    /// <summary>媒体类型：官方固定填 2 图片（填错回 750034）。</summary>
    [JsonPropertyName("media_type")]
    public int MediaType { get; set; }
}

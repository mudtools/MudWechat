// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.MiniProgram.DataModels.Operation;

/// <summary>论坛（feedback）列表条目（<c>list[]</c>）。</summary>
/// <remarks>字段以官方页面为准；SDK 不做本地校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Operation")]
public class WxaFeedbackItem
{
    /// <summary>反馈记录 ID（<c>record_id</c>）。</summary>
    [JsonPropertyName("record_id")]
    public long? RecordId { get; set; }

    /// <summary>反馈用户 openid（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>反馈时间（<c>create_time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>反馈内容（<c>content</c>）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>反馈类型（<c>type</c>）：<c>1</c> 无法打开 / <c>2</c> 一直闪退 / <c>3</c> 一直卡死 / <c>4</c> 其他问题（数值以官方页面为准）。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>反馈图片媒体 ID 列表（<c>media_ids</c>）。</summary>
    [JsonPropertyName("media_ids")]
    public List<string>? MediaIds { get; set; }
}

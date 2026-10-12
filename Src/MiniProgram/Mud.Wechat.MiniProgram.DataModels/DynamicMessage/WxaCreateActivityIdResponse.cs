// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DynamicMessage;

/// <summary>创建动态消息 <c>activity_id</c> 应答（<c>activity_id</c> + 有效期）。</summary>
/// <remarks>
/// <para>官方文档：<c>mp-message-management/updatable-message/api_createactivityid.html</c>。</para>
/// <para><b>一次性（官方原文）</b>：<see cref="ActivityId"/> 只能被使用一次（分享后即失效）；逾期亦失效。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DynamicMessage")]
public class WxaCreateActivityIdResponse : WxaResponse
{
    /// <summary>动态消息活动唯一标识（<c>activity_id</c>）。</summary>
    [JsonPropertyName("activity_id")]
    public string? ActivityId { get; set; }

    /// <summary>过期时间（<c>expiration_time</c>，Unix 秒级时间戳）。</summary>
    [JsonPropertyName("expiration_time")]
    public long? ExpirationTime { get; set; }

    /// <summary>是否已过期（<c>is_expired</c>）。</summary>
    [JsonPropertyName("is_expired")]
    public bool? IsExpired { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.PlatformKf;

/// <summary>平台处理记录（官方 <c>platform_operate</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketPlatformOperate
{
    /// <summary>获取或设置平台审核结果（官方 <c>audit_result</c>，1-5，含义同 <see cref="ChannelsPlatformKfReviewResults"/>）。</summary>
    [JsonPropertyName("audit_result")]
    public int? AuditResult { get; set; }

    /// <summary>获取或设置对商家的说明（官方 <c>reason</c>，审核不通过原因）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置用户是否认可（官方 <c>is_agree</c>）。</summary>
    [JsonPropertyName("is_agree")]
    public int? IsAgree { get; set; }

    /// <summary>获取或设置回复用户话术（官方 <c>reply_description</c>）。</summary>
    [JsonPropertyName("reply_description")]
    public string? ReplyDescription { get; set; }

    /// <summary>获取或设置平台答复时间（官方 <c>platform_reply_time</c>，Unix 时间戳，秒）。</summary>
    [JsonPropertyName("platform_reply_time")]
    public long? PlatformReplyTime { get; set; }
}

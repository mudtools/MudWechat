// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.PlatformKf;

/// <summary>工单回复选项配置（官方 <c>option_list</c> 数组元素）。</summary>
/// <remarks><c>operatemchticket</c> 的 <c>reply_option</c> / <c>description</c> / <c>confer_time</c>
/// 必填性由本配置的 <c>option_id</c> / <c>is_must_description</c> / <c>is_must_confer_time</c> 决定。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfTicketReplyOption
{
    /// <summary>获取或设置工单回复选项（官方 <c>option_id</c>，处理工单时作为 <c>reply_option</c> 回传）。</summary>
    [JsonPropertyName("option_id")]
    public int? OptionId { get; set; }

    /// <summary>获取或设置工单回复名称（官方 <c>option_name</c>）。</summary>
    [JsonPropertyName("option_name")]
    public string? OptionName { get; set; }

    /// <summary>获取或设置补充说明是否必填（官方 <c>is_must_description</c>）。</summary>
    [JsonPropertyName("is_must_description")]
    public bool? IsMustDescription { get; set; }

    /// <summary>获取或设置图片 / 视频凭证是否必填（官方 <c>is_must_urls</c>）。</summary>
    [JsonPropertyName("is_must_urls")]
    public bool? IsMustUrls { get; set; }

    /// <summary>获取或设置协商时间是否必填（官方 <c>is_must_confer_time</c>）。</summary>
    [JsonPropertyName("is_must_confer_time")]
    public bool? IsMustConferTime { get; set; }
}

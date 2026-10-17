// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取推客的注册状态以及和机构的绑定状态（<c>promoter/get_promoter_register_and_bind_status</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetRegisterAndBindStatusResponse : ChannelsResponse
{
    /// <summary>获取或设置和机构的绑定状态（官方 <c>bind_status</c>，0：未绑定 1：已绑定）。</summary>
    [JsonPropertyName("bind_status")]
    public int? BindStatus { get; set; }

    /// <summary>获取或设置当前推客的注册状态（官方 <c>register_status</c>，见 <see cref="ChannelsPromoterRegisterStatuses"/>）。</summary>
    [JsonPropertyName("register_status")]
    public int? RegisterStatus { get; set; }

    /// <summary>获取或设置注册流程 <c>openBusinessView</c> 需要的 businessType（官方 <c>register_business_type</c>，<c>register_status</c> 为 0 或 1 时使用）。</summary>
    [JsonPropertyName("register_business_type")]
    public string? RegisterBusinessType { get; set; }

    /// <summary>获取或设置注册时需要的 queryString 参数（官方 <c>register_query_string</c>）。</summary>
    [JsonPropertyName("register_query_string")]
    public string? RegisterQueryString { get; set; }

    /// <summary>获取或设置绑定流程 <c>openBusinessView</c> 需要的 businessType（官方 <c>bind_business_type</c>，<c>bind_status</c> 为 0 时使用）。</summary>
    [JsonPropertyName("bind_business_type")]
    public string? BindBusinessType { get; set; }

    /// <summary>获取或设置绑定时需要的 queryString 参数（官方 <c>bind_query_string</c>）。</summary>
    [JsonPropertyName("bind_query_string")]
    public string? BindQueryString { get; set; }

    /// <summary>获取或设置已绑定推客的 appid（官方 <c>sharer_appid</c>，推客和机构已绑定时返回）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }
}

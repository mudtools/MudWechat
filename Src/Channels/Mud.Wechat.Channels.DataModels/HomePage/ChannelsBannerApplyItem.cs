// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>精选展示位申请明细（官方 <c>apply.banner[]</c> 数组元素，<c>getbanner</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsBannerApplyItem
{
    /// <summary>获取或设置展示位（官方 <c>banner</c>，结构与生效展示位一致）。</summary>
    [JsonPropertyName("banner")]
    public ChannelsBannerItem? Banner { get; set; }

    /// <summary>获取或设置审核状态（官方 <c>audit_state</c>，见 <see cref="ChannelsHomepageApplyStates"/>）。</summary>
    [JsonPropertyName("audit_state")]
    public int? AuditState { get; set; }

    /// <summary>获取或设置审核结果描述（官方 <c>audit_desc</c>；audit_state 为驳回时有值）。</summary>
    [JsonPropertyName("audit_desc")]
    public string? AuditDesc { get; set; }
}

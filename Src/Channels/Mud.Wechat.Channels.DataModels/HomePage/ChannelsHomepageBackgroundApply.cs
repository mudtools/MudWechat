// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>主页背景图申请信息（官方 <c>apply</c> 对象，<c>getbackground</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsHomepageBackgroundApply
{
    /// <summary>获取或设置申请编号（官方 <c>apply_id</c>）。</summary>
    [JsonPropertyName("apply_id")]
    public long? ApplyId { get; set; }

    /// <summary>获取或设置申请状态（官方 <c>state</c>，见 <see cref="ChannelsHomepageApplyStates"/>）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>获取或设置审核结果描述（官方 <c>audit_desc</c>；state 为审核驳回时有值）。</summary>
    [JsonPropertyName("audit_desc")]
    public string? AuditDesc { get; set; }

    /// <summary>获取或设置图片 url（官方 <c>img_url</c>）。</summary>
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
}

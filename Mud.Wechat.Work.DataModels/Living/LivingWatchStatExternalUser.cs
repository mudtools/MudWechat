// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 观看直播的外部成员明细（获取直播观看明细响应 <c>stat_info.external_users</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class LivingWatchStatExternalUser
{
    /// <summary>获取或设置外部成员的 external_userid。</summary>
    [JsonPropertyName("external_userid")]
    public string? ExternalUserid { get; set; }

    /// <summary>获取或设置外部成员类型：1 - 微信用户；2 - 企业微信用户。</summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>获取或设置外部成员的名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置观看时长（单位秒）。</summary>
    [JsonPropertyName("watch_time")]
    public int? WatchTime { get; set; }

    /// <summary>获取或设置是否评论：0 - 否；1 - 是。</summary>
    [JsonPropertyName("is_comment")]
    public int? IsComment { get; set; }

    /// <summary>获取或设置是否连麦发言：0 - 否；1 - 是。</summary>
    [JsonPropertyName("is_mic")]
    public int? IsMic { get; set; }

    /// <summary>
    /// 获取或设置邀请人的 userid。
    /// <para>邀请人为企业内部成员时返回（观众首次进入直播时所用的直播卡片/二维码对应的分享人；仅「推广产品」直播支持）。</para>
    /// </summary>
    [JsonPropertyName("invitor_userid")]
    public string? InvitorUserid { get; set; }

    /// <summary>
    /// 获取或设置邀请人的 external_userid。
    /// <para>邀请人为非企业内部成员时返回（含义同邀请人为企业内部成员时；仅「推广产品」直播支持）。</para>
    /// </summary>
    [JsonPropertyName("invitor_external_userid")]
    public string? InvitorExternalUserid { get; set; }
}

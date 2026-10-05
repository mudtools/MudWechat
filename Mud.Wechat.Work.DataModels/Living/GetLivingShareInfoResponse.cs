// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 获取跳转小程序商城的直播观众信息响应体（<c>/cgi-bin/living/get_living_share_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class GetLivingShareInfoResponse : WechatWorkResponse
{
    /// <summary>获取或设置直播 id。</summary>
    [JsonPropertyName("livingid")]
    public string? Livingid { get; set; }

    /// <summary>获取或设置观众的 userid（观众为企业内部成员时返回）。</summary>
    [JsonPropertyName("viewer_userid")]
    public string? ViewerUserid { get; set; }

    /// <summary>获取或设置观众的 external_userid（观众为非企业内部成员时返回）。</summary>
    [JsonPropertyName("viewer_external_userid")]
    public string? ViewerExternalUserid { get; set; }

    /// <summary>
    /// 获取或设置邀请人的 userid。
    /// <para>邀请人为企业内部成员时返回（观众首次进入直播时，其使用的直播卡片/二维码所对应的分享人）。</para>
    /// </summary>
    [JsonPropertyName("invitor_userid")]
    public string? InvitorUserid { get; set; }

    /// <summary>
    /// 获取或设置邀请人的 external_userid。
    /// <para>邀请人为非企业内部成员时返回（含义同邀请人为企业内部成员时）。</para>
    /// </summary>
    [JsonPropertyName("invitor_external_userid")]
    public string? InvitorExternalUserid { get; set; }
}

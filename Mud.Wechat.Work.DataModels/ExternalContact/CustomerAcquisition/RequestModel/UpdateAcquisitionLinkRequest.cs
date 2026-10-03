// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

/// <summary>
/// 编辑获客链接请求体（<c>/cgi-bin/externalcontact/customer_acquisition/update_link</c>）。
/// <para><see cref="LinkId"/> 为官方必填且需为当前应用创建；
/// <see cref="Range"/> 为覆盖式更新（覆盖总人数不超过 500），<see cref="PriorityOption"/> 为覆盖式更新。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class UpdateAcquisitionLinkRequest
{
    /// <summary>
    /// 获取或设置获客链接 id（官方必填；需为当前应用创建的链接）。
    /// </summary>
    [JsonPropertyName("link_id")]
    public string? LinkId { get; set; }

    /// <summary>
    /// 获取或设置更新后的获客链接名称（最长 30 个字符）。
    /// </summary>
    [JsonPropertyName("link_name")]
    public string? LinkName { get; set; }

    /// <summary>
    /// 获取或设置获客链接的使用范围（覆盖式更新，覆盖总人数不超过 500）。
    /// </summary>
    [JsonPropertyName("range")]
    public AcquisitionLinkRange? Range { get; set; }

    /// <summary>
    /// 获取或设置添加客户时是否无需验证（默认为 true）。
    /// </summary>
    [JsonPropertyName("skip_verify")]
    public bool? SkipVerify { get; set; }

    /// <summary>
    /// 获取或设置获客链接的优先分配选项（覆盖式更新，仅部分经营类目企业支持）。
    /// </summary>
    [JsonPropertyName("priority_option")]
    public AcquisitionPriorityOption? PriorityOption { get; set; }

    /// <summary>
    /// 获取或设置是否标记客户添加来源（仅对「营销获客」应用生效）。
    /// </summary>
    [JsonPropertyName("mark_source")]
    public bool? MarkSource { get; set; }
}

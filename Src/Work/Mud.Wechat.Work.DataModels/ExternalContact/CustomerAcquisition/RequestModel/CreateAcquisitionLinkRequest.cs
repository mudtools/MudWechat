// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

/// <summary>
/// 创建获客链接请求体（<c>/cgi-bin/externalcontact/customer_acquisition/create_link</c>）。
/// <para>
/// <see cref="LinkName"/> 为官方必填；<see cref="Range"/> 的成员与部门不可同时为空，
/// 覆盖总人数不超过 500，且须在应用可见范围或客户可建联成员范围内；
/// <see cref="PriorityOption"/> 仅部分经营类目企业支持；
/// 可通过 <c>weixin://biz/ww/profile/{urlencode(LINK_URL?customer_channel=STATE)}</c> 拼接获客链接 scheme。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "CustomerAcquisition")]
public class CreateAcquisitionLinkRequest
{
    /// <summary>
    /// 获取或设置获客链接名称（官方必填，最长 30 个字符）。
    /// </summary>
    [JsonPropertyName("link_name")]
    public string? LinkName { get; set; }

    /// <summary>
    /// 获取或设置获客链接的使用范围（成员与部门不可同时为空，覆盖总人数不超过 500）。
    /// </summary>
    [JsonPropertyName("range")]
    public AcquisitionLinkRange? Range { get; set; }

    /// <summary>
    /// 获取或设置添加客户时是否无需验证（默认为 true）。
    /// </summary>
    [JsonPropertyName("skip_verify")]
    public bool? SkipVerify { get; set; }

    /// <summary>
    /// 获取或设置获客链接的优先分配选项（仅部分经营类目企业支持）。
    /// </summary>
    [JsonPropertyName("priority_option")]
    public AcquisitionPriorityOption? PriorityOption { get; set; }

    /// <summary>
    /// 获取或设置是否标记客户添加来源（默认为 true；仅对「营销获客」应用生效）。
    /// </summary>
    [JsonPropertyName("mark_source")]
    public bool? MarkSource { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>证照组（官方 <c>license_group_list</c> 数组元素——「申请类目」请求体与「审核单详情」响应 <c>audit_info</c> 字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicenseGroup
{
    /// <summary>获取或设置证照组 id（官方 <c>license_group_id</c>，必填；经「获取所有类目」获取）。</summary>
    [JsonPropertyName("license_group_id")]
    public long LicenseGroupId { get; set; }

    /// <summary>获取或设置证照信息（官方 <c>license</c>，必填；组内多个证照只需选一个提交）。</summary>
    [JsonPropertyName("license")]
    public ChannelsCategoryLicense License { get; set; } = new();
}

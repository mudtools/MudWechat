// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>证照列表项（官方 <c>license_list</c> 数组元素，<c>getallcategory</c> 证照组内）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicenseInfo
{
    /// <summary>获取或设置证照 id（官方 <c>id</c>）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置证照名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置是否必填（官方 <c>is_necessary</c>）。</summary>
    [JsonPropertyName("is_necessary")]
    public int? IsNecessary { get; set; }

    /// <summary>获取或设置填写字段列表（官方 <c>license_field_list</c>）。</summary>
    [JsonPropertyName("license_field_list")]
    public List<ChannelsCategoryLicenseField>? LicenseFieldList { get; set; }
}

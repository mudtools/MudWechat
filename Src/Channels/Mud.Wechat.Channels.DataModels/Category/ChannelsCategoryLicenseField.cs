// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>证照填写字段（官方 <c>license_field_list</c> 数组元素，<c>getallcategory</c> 证照内）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicenseField
{
    /// <summary>
    /// 获取或设置填写字段类型（官方 <c>field_key</c>）——官方枚举：<c>audit_license_no</c>（许可证编号）/
    /// <c>audit_license_registration_no</c>（备案编号）/ <c>audit_license_validty_period</c>（有效期，官方历史拼写照录）。
    /// </summary>
    [JsonPropertyName("field_key")]
    public string? FieldKey { get; set; }

    /// <summary>获取或设置填写字段名称（官方 <c>field_name</c>）。</summary>
    [JsonPropertyName("field_name")]
    public string? FieldName { get; set; }

    /// <summary>获取或设置字段正则要求（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public string? Info { get; set; }

    /// <summary>获取或设置是否必填（官方 <c>is_necessary</c>）。</summary>
    [JsonPropertyName("is_necessary")]
    public int? IsNecessary { get; set; }
}

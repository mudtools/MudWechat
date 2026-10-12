// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>证照填写字段键值对（官方 <c>license_field_list</c> 数组元素 <c>{key, value}</c>，证照提交与审核材料形态一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicenseFieldPair
{
    /// <summary>获取或设置字段 key（官方 <c>key</c>，必填；取值为「获取所有类目」返回的 <c>field_key</c>）。</summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>获取或设置字段 value（官方 <c>value</c>，必填）。</summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

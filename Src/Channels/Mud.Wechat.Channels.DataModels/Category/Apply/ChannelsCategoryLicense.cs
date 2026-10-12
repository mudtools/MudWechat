// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>证照信息（官方 <c>license</c> 对象——「申请类目」请求体与「审核单详情」响应字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryLicense
{
    /// <summary>获取或设置证照 id（官方 <c>license_id</c>，必填；经「获取所有类目」获取）。</summary>
    [JsonPropertyName("license_id")]
    public long LicenseId { get; set; }

    /// <summary>获取或设置证照图片列表（官方 <c>file_id_list</c>，必填；必须用「上传资质图片」接口获取的 file_id）。</summary>
    [JsonPropertyName("file_id_list")]
    public List<string> FileIdList { get; set; } = new();

    /// <summary>获取或设置证照填写字段列表（官方 <c>license_field_list</c>，必填）。</summary>
    [JsonPropertyName("license_field_list")]
    public List<ChannelsCategoryLicenseFieldPair> LicenseFieldList { get; set; } = new();
}

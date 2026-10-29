// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>品牌授权信息（官方 <c>grant_details</c> 对象，提交形态与响应形态字段集一致 ⇒ 共用）。</summary>
/// <remarks>授权品牌建议使用分级授权书结构（<c>use_split_grant_info</c> 大于 0 即使用，使用时兼容填充旧结构）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandGrantDetails
{
    /// <summary>获取或设置（旧结构）品牌销售授权书 file_id 列表（官方 <c>grant_certifications</c>，授权品牌必填，最多 9 张）。</summary>
    [JsonPropertyName("grant_certifications")]
    public List<string>? GrantCertifications { get; set; }

    /// <summary>获取或设置授权级数（官方 <c>grant_level</c>，授权品牌必填，取值 1-3）。</summary>
    [JsonPropertyName("grant_level")]
    public int? GrantLevel { get; set; }

    /// <summary>获取或设置（旧结构）授权有效期开始时间（官方 <c>start_time</c>）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置（旧结构）授权有效期结束时间（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置（旧结构）是否长期有效（官方 <c>is_permanent</c>）。</summary>
    [JsonPropertyName("is_permanent")]
    public bool? IsPermanent { get; set; }

    /// <summary>获取或设置品牌权利人证件照 file_id 列表（官方 <c>brand_owner_id_photos</c>，选填，最多 2 张）。</summary>
    [JsonPropertyName("brand_owner_id_photos")]
    public List<string>? BrandOwnerIdPhotos { get; set; }

    /// <summary>获取或设置是否使用分级授权书结构（官方 <c>use_split_grant_info</c>，大于 0 即使用）。</summary>
    [JsonPropertyName("use_split_grant_info")]
    public long? UseSplitGrantInfo { get; set; }

    /// <summary>获取或设置一级授权结构（官方 <c>grant_info_lv1</c>；多于一级授权时必填）。</summary>
    [JsonPropertyName("grant_info_lv1")]
    public ChannelsBrandGrantInfo? GrantInfoLv1 { get; set; }

    /// <summary>获取或设置二级授权结构（官方 <c>grant_info_lv2</c>；多于二级授权时必填）。</summary>
    [JsonPropertyName("grant_info_lv2")]
    public ChannelsBrandGrantInfo? GrantInfoLv2 { get; set; }

    /// <summary>获取或设置三级授权结构（官方 <c>grant_info_lv3</c>；三级授权时必填）。</summary>
    [JsonPropertyName("grant_info_lv3")]
    public ChannelsBrandGrantInfo? GrantInfoLv3 { get; set; }

    /// <summary>获取或设置联系方式列表（官方 <c>contact_info_list</c>，选填）。</summary>
    [JsonPropertyName("contact_info_list")]
    public List<ChannelsBrandContactInfo>? ContactInfoList { get; set; }
}

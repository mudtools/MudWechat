// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>分级授权结构（官方 <c>grant_info_lv1</c>/<c>grant_info_lv2</c>/<c>grant_info_lv3</c>，三级字段集一致 ⇒ 共用一个 DTO）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandGrantInfo
{
    /// <summary>获取或设置（分级结构）品牌销售授权书 file_id 列表（官方 <c>grant_certifications</c>；对应级别授权时必填）。</summary>
    [JsonPropertyName("grant_certifications")]
    public List<string>? GrantCertifications { get; set; }

    /// <summary>获取或设置（分级结构）授权有效期开始时间（官方 <c>start_time</c>）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置（分级结构）授权有效期结束时间（官方 <c>end_time</c>）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置（分级结构）是否长期有效（官方 <c>is_permanent</c>）。</summary>
    [JsonPropertyName("is_permanent")]
    public bool? IsPermanent { get; set; }
}

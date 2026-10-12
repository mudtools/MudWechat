// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>商标注册信息（官方 <c>register_details</c> 对象，提交形态与响应形态字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsBrandRegisterDetails
{
    /// <summary>获取或设置商标注册人（官方 <c>registrant</c>，R 标时必填）。</summary>
    [JsonPropertyName("registrant")]
    public string? Registrant { get; set; }

    /// <summary>获取或设置商标注册号（官方 <c>register_no</c>，R 标时必填）。</summary>
    [JsonPropertyName("register_no")]
    public string? RegisterNo { get; set; }

    /// <summary>获取或设置注册有效期开始时间（官方 <c>start_time</c>，选填，长期有效可不填）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置注册有效期结束时间（官方 <c>end_time</c>，选填，长期有效可不填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否长期有效（官方 <c>is_permanent</c>，必填）。</summary>
    [JsonPropertyName("is_permanent")]
    public bool? IsPermanent { get; set; }

    /// <summary>获取或设置商标注册证 file_id 列表（官方 <c>register_certifications</c>，R 标时必填，最多 1 张）。</summary>
    [JsonPropertyName("register_certifications")]
    public List<string>? RegisterCertifications { get; set; }

    /// <summary>获取或设置变更/续展证明 file_id 列表（官方 <c>renew_certifications</c>，选填，最多 5 张）。</summary>
    [JsonPropertyName("renew_certifications")]
    public List<string>? RenewCertifications { get; set; }
}

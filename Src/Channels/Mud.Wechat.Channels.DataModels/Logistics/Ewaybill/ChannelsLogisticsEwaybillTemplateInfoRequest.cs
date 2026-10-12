// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>新增 / 更新面单模板请求信息（官方 <c>info</c>，create 与 update 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTemplateInfoRequest
{
    /// <summary>获取或设置模板 id（官方 <c>template_id</c>，update 必填）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置模板名（官方 <c>template_name</c>，必填，同一快递公司下不可重复）。</summary>
    [JsonPropertyName("template_name")]
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>获取或设置模板描述（官方 <c>template_desc</c>，默认填「一联单标准模板」）。</summary>
    [JsonPropertyName("template_desc")]
    public string? TemplateDesc { get; set; }

    /// <summary>获取或设置模板类型（官方 <c>template_type</c>，默认填 'single'）。</summary>
    [JsonPropertyName("template_type")]
    public string? TemplateType { get; set; }

    /// <summary>获取或设置是否为该快递公司默认模板（官方 <c>is_default</c>，必填）。</summary>
    [JsonPropertyName("is_default")]
    public bool IsDefault { get; set; }

    /// <summary>获取或设置模板信息选项（官方 <c>options</c>，必填，总数必须为 8 个，option_id 不得重复）。</summary>
    [JsonPropertyName("options")]
    public List<ChannelsLogisticsEwaybillTemplateOption> Options { get; set; } = new();
}

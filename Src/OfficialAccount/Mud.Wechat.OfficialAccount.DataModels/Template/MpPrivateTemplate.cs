// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>已选用模板条目（官方 <c>template_list</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpPrivateTemplate
{
    /// <summary>获取或设置模板 ID（官方 <c>template_id</c>）。</summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置模板标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置模板所属行业的一级行业（官方 <c>primary_industry</c>）。</summary>
    [JsonPropertyName("primary_industry")]
    public string? PrimaryIndustry { get; set; }

    /// <summary>获取或设置模板所属行业的二级行业（官方 <c>deputy_industry</c>——注意与行业查询页的 secondary_industry 命名不同）。</summary>
    [JsonPropertyName("deputy_industry")]
    public string? DeputyIndustry { get; set; }

    /// <summary>获取或设置模板内容（官方 <c>content</c>，形如 <c>{{result.DATA}}</c> 的参数占位形态）。</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置模板示例（官方 <c>example</c>）。</summary>
    [JsonPropertyName("example")]
    public string? Example { get; set; }
}

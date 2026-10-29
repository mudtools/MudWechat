// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

/// <summary>
/// 「将草稿添加到模板库」请求（<c>addtotemplate</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/thirdparty-management/template-management/addToTemplate.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramTemplate")]
public class OpenPlatformAddToTemplateRequest
{
    /// <summary>草稿 ID（官方必填 <c>draft_id</c>，number）。</summary>
    [JsonPropertyName("draft_id")]
    public long? DraftId { get; set; }

    /// <summary>模板类型：缺省 0 普通模板，可选 1 标准模板（官方选填 <c>template_type</c>，number）。</summary>
    [JsonPropertyName("template_type")]
    public long? TemplateType { get; set; }
}

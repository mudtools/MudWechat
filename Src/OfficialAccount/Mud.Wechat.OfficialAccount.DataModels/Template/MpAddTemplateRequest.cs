// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>选用模板（<c>template/api_add_template</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>template_id_short</c> + <c>keyword_name_list</c> 两字段；
/// 行业模板库编号为 TM**/OPENTMTM** 形态，<b>类目模板为纯数字 id</b>（示例 47123）——
/// 官方将两类形态合并在同一字段集中，无独立分支字段。
/// </para>
/// <para>
/// <b>官方文档矛盾（照录）</b>：<c>keyword_name_list</c> 标注必填，但仅行业模板库（TM** 形态）
/// 无需关键词——行业模板传关键词的行为页面未说明。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpAddTemplateRequest
{
    /// <summary>获取或设置模板库中模板的编号（官方 <c>template_id_short</c>，TM**/OPENTMTM**/纯数字形态）。</summary>
    [JsonPropertyName("template_id_short")]
    public string TemplateIdShort { get; set; } = string.Empty;

    /// <summary>获取或设置选用的类目模板关键词（官方 <c>keyword_name_list</c>，按顺序传入；空或不在模板库返回 40246）。</summary>
    [JsonPropertyName("keyword_name_list")]
    public List<string>? KeywordNameList { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Template;

/// <summary>获取已选用模板列表（<c>template/get_all_private_template</c>）响应。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体（请求示例 <c>{}</c>）；
/// 响应元素二级行业字段名为 <c>deputy_industry</c>（非 secondary_industry，照抄官方）。
/// 官方指南页另有「每个账号可同时使用 25 个模板」规则（不在 API 页）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Template")]
public class MpGetAllPrivateTemplateResponse : MpResponse
{
    /// <summary>获取或设置已选用模板列表（官方 <c>template_list</c>）。</summary>
    [JsonPropertyName("template_list")]
    public List<MpPrivateTemplate>? TemplateList { get; set; }
}

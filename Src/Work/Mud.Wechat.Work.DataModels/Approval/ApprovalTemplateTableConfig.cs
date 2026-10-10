// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 明细控件配置（config.table，获取审批模板详情响应侧）。
/// </summary>
/// <remarks>
/// <para>官方全字段示例中存在 stat_field 数组，但官方文档未提供其参数说明，本模型暂不建模（反序列化时忽略该字段）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateTableConfig
{
    /// <summary>
    /// 获取或设置明细内的子控件列表（内部结构同 controls）。
    /// </summary>
    [JsonPropertyName("children")]
    public List<ApprovalTemplateControl>? Children { get; set; }
}

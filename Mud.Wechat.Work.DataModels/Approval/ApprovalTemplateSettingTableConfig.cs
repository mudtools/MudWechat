// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 创建/更新模板的明细控件配置（config.table 请求侧）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateSettingTableConfig
{
    /// <summary>
    /// 获取或设置打印格式：0-合并成一行打印；1-拆分成多行打印。
    /// </summary>
    [JsonPropertyName("print_format")]
    public int? PrintFormat { get; set; }

    /// <summary>
    /// 获取或设置明细内的子控件列表，每个子控件的属性和控件相同。
    /// </summary>
    /// <remarks>
    /// <para>官方限制：不能为空数组，至少需要包含一个子控件；明细中不能设置假勤控件、明细控件。</para>
    /// </remarks>
    [JsonPropertyName("children")]
    public List<ApprovalTemplateSettingControl>? Children { get; set; }
}

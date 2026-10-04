// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 部门控件选项（departments 元素，control 为 Contact 且 value 参数为 departments）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalDepartmentItem
{
    /// <summary>
    /// 获取或设置所选部门id。
    /// </summary>
    /// <remarks>
    /// <para>官方示例按字符串传输（如 "2"），故本模型以字符串承载。</para>
    /// </remarks>
    [JsonPropertyName("openapi_id")]
    public string? OpenapiId { get; set; }

    /// <summary>
    /// 获取或设置所选部门名。
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

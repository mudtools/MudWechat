// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批记录模板信息（comm）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalCommData
{
    /// <summary>
    /// 获取或设置审批申请的单据数据（JSON 字符串）。
    /// </summary>
    /// <remarks>
    /// <para>近期官方对 APPLY_DATA 进行了优化，短时间内存在新老两种数据格式：老格式为对象（键为控件id，value 含 title/type/value）、新格式为数组（元素含 id/type/value/title）；可根据其是对象还是数组判断新老结构，官方建议统一使用新格式数据。本模型按官方传输形态以字符串承载，由调用方按需二次解析。</para>
    /// </remarks>
    [JsonPropertyName("apply_data")]
    public string? ApplyData { get; set; }
}

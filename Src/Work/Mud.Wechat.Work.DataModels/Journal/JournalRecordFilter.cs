// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 汇报记录筛选条件（官方 filters 元素，key / value 形态）。
/// <para>
/// 官方以 key 指明筛选维度、value 给出该维度的取值；<c>filters</c> 整体可选，其 key / value 各自不多于 256 字节。
/// 本 SDK 按字符串原样承载（与「审批」域的筛选条件 <see cref="Approval.ApprovalInfoFilter"/> 同形）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalRecordFilter
{
    /// <summary>获取或设置筛选维度（官方 key，不多于 256 字节；官方参数表列出的取值为 creator - 指定汇报记录提单人、department - 指定提单人所在部门、template_id - 指定模板）。</summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>获取或设置筛选值（官方 value，不多于 256 字节，随 <see cref="Key"/> 的维度语义取值：userid / 部门 id / 汇报表单 id）。</summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
}

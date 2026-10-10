// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 智能表格内容权限规则（官方 <c>rule_list</c> 元素；查询智能表格子表权限响应体嵌套对象）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：每个智能表格有且只有一个全员权限；智能表格内容权限由全员权限及至多 20 条成员额外权限组成。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSheetPrivRule
{
    /// <summary>获取或设置规则 id（官方 <c>rule_id</c>）。</summary>
    [JsonPropertyName("rule_id")]
    public uint? RuleId { get; set; }

    /// <summary>
    /// 获取或设置权限规则类型（官方 <c>type</c>）。
    /// 官方取值：<c>1</c> 全员权限、<c>2</c> 额外权限。
    /// </summary>
    [JsonPropertyName("type")]
    public uint? Type { get; set; }

    /// <summary>获取或设置权限名称（官方 <c>name</c>），仅当 <c>type</c> 为 <c>2</c> 时有效。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置针对不同子表设置的内容权限列表（官方 <c>priv_list</c>）。</summary>
    [JsonPropertyName("priv_list")]
    public List<WedocSheetPriv>? PrivList { get; set; }
}

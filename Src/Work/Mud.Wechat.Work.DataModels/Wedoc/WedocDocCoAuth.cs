// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档查看权限特定部门项（官方 <c>co_auth_list</c> 元素；
/// 获取文档权限信息响应与修改文档加入规则请求共用），列表中的部门可直接浏览文档。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：<c>type</c> 目前仅支持部门（值 2）；修改文档加入规则传入空列表会清空特定部门权限列表。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocCoAuth
{
    /// <summary>
    /// 获取或设置特定部门类型（官方 <c>type</c>）。
    /// 官方取值：<c>2</c> 部门（目前仅支持部门）。
    /// </summary>
    [JsonPropertyName("type")]
    public uint? Type { get; set; }

    /// <summary>获取或设置特定部门 id（官方 <c>departmentid</c>）。</summary>
    [JsonPropertyName("departmentid")]
    public ulong? Departmentid { get; set; }

    /// <summary>
    /// 获取或设置该部门的权限类型（官方 <c>auth</c>）。
    /// 官方取值：<c>1</c> 只读、<c>2</c> 读写。
    /// </summary>
    [JsonPropertyName("auth")]
    public uint? Auth { get; set; }
}

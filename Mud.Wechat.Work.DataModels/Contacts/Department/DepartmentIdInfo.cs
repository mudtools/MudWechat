// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Department;

/// <summary>
/// 部门 ID 信息（获取子部门 ID 列表 <c>/cgi-bin/department/simplelist</c> 响应中 <c>department_id[]</c> 的元素；
/// 2022-08 通讯录安全加固后的官方推荐替代接口）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Department")]
public class DepartmentIdInfo
{
    /// <summary>
    /// 获取或设置部门 ID（32 位整型）。
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 获取或设置父部门 ID（根部门为 1）。
    /// </summary>
    [JsonPropertyName("parentid")]
    public int ParentId { get; set; }

    /// <summary>
    /// 获取或设置在父部门中的次序值（官方口径：<b>值小的排序靠前</b>，与成员 <c>order</c>「值大靠前」语义相反；官方范围 [0, 2^32)，故以 64 位整数承载）。
    /// </summary>
    [JsonPropertyName("order")]
    public long? Order { get; set; }
}

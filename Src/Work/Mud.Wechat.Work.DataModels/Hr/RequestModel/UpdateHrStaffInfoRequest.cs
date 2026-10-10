// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Hr;

/// <summary>
/// 更新员工花名册信息请求体（<c>/cgi-bin/hr/update_staff_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Hr")]
public class UpdateHrStaffInfoRequest
{
    /// <summary>获取或设置需要更新花名册信息的员工 userid（官方必填；该员工须在调用应用的可见范围内，否则返回错误码）。</summary>
    [JsonPropertyName("userid")]
    public string Userid { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置需要更新、增加或清空单个字段的内容（官方 update_items；与 remove_items / insert_items 不能全部为空）。
    /// </summary>
    [JsonPropertyName("update_items")]
    public List<HrUpdateFieldItem>? UpdateItems { get; set; }

    /// <summary>
    /// 获取或设置可重复字段组中需要整组删除的字段组（官方 remove_items；与 update_items / insert_items 不能全部为空）。
    /// </summary>
    [JsonPropertyName("remove_items")]
    public List<HrRemoveFieldGroup>? RemoveItems { get; set; }

    /// <summary>
    /// 获取或设置可重复字段组中需要增加一组字段的字段组（官方 insert_items；与 update_items / remove_items 不能全部为空）。
    /// </summary>
    [JsonPropertyName("insert_items")]
    public List<HrInsertFieldGroup>? InsertItems { get; set; }
}

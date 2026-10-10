// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Security;

/// <summary>
/// 获取成员操作记录响应体（<c>/cgi-bin/security/member_oper_log/list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class ListMemberOperLogsResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置是否还有下一页。
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>
    /// 获取或设置下一页的分页游标（不同过滤条件的游标不能混用）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置成员操作记录列表。
    /// </summary>
    [JsonPropertyName("record_list")]
    public List<MemberOperLogItem>? RecordList { get; set; }
}

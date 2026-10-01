// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 获取成员授权列表请求体（<c>/cgi-bin/user/list_member_auth</c>，仅企业为成员授权模式下可调用）。
/// </summary>
public class ListMemberAuthRequest
{
    /// <summary>
    /// 获取或设置分页游标（上一次调用返回的 next_cursor；第一次拉取不填）。
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    /// 获取或设置每次拉取的数据量（默认值和最大值都为 1000）。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

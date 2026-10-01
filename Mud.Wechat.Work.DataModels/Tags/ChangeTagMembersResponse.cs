// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Tags;

/// <summary>
/// 增加 / 删除标签成员的三态响应体（<c>/cgi-bin/tag/addtagusers</c>、<c>/cgi-bin/tag/deltagusers</c> 共用）。
/// </summary>
/// <remarks>
/// 三种返回：<c>errcode = 0</c> 表示全部合法（也可能带部分非法列表）；
/// 部分非法时仍返回 0 并附 <see cref="InvalidList"/> / <see cref="InvalidParty"/>；
/// 全部非法时返回错误码（增加成员自建版文档标注 40070，第三方 / 代开发删除成员文档标注 40031，
/// 两文档树对「全部非法」错误码标注不一致，以实际返回为准）。
/// </remarks>
public class ChangeTagMembersResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置非法成员账号列表（官方契约：竖线 <c>|</c> 分隔的字符串，如 <c>usr1|usr2|usr</c>；不再以数组返回）。
    /// </summary>
    [JsonPropertyName("invalidlist")]
    public string? InvalidList { get; set; }

    /// <summary>
    /// 获取或设置非法部门 ID 列表（数组）。
    /// </summary>
    [JsonPropertyName("invalidparty")]
    public List<int>? InvalidParty { get; set; } = [];
}

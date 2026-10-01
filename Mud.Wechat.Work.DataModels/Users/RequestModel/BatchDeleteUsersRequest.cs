// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 批量删除成员请求体（<c>/cgi-bin/user/batchdelete</c>，仅通讯录同步助手或第三方通讯录应用可调用）。
/// </summary>
public class BatchDeleteUsersRequest
{
    /// <summary>
    /// 获取或设置待删除的成员 UserID 列表（最多支持 200 个；若存在无效 UserID，直接返回错误）。
    /// </summary>
    [JsonPropertyName("useridlist")]
    public List<string> UserIdList { get; set; } = [];
}

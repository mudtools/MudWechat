// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contracts.Users;

/// <summary>
/// 查询成员用户是否已授权响应体（<c>/cgi-bin/user/check_member_auth</c>，仅企业为成员授权模式下可调用）。
/// </summary>
public class CheckMemberAuthResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置该成员是否已授权（成员授权模式下返回；以可空布尔承载，字段缺失时不与 false 混淆）。
    /// </summary>
    [JsonPropertyName("is_member_auth")]
    public bool? IsMemberAuth { get; set; }
}

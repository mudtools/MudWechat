// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Users;

/// <summary>
/// 成员对外属性中的视频号信息（<c>external_profile.wechat_channels</c>）。
/// </summary>
public class UserWechatChannels
{
    /// <summary>
    /// 获取或设置视频号名称（须从企业绑定的视频号中选择）。
    /// </summary>
    [JsonPropertyName("nickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// 获取或设置视频号状态。
    /// </summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

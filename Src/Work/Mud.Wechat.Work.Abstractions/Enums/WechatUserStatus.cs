// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Enums;

/// <summary>
/// 成员激活状态（官方 90970：1 = 已激活，2 = 已禁用，4 = 未激活，5 = 退出企业）。
/// </summary>
/// <remarks>
/// 需管理员授权才回调；未授权时对应载荷字段为 <c>null</c> —— 处理器<b>不得假设必有值</b>。
/// </remarks>
public enum WechatUserStatus
{
    /// <summary>已激活（官方值 <c>1</c>）。</summary>
    Activated = 1,

    /// <summary>已禁用（官方值 <c>2</c>）。</summary>
    Disabled = 2,

    /// <summary>未激活（官方值 <c>4</c>）。</summary>
    NotActivated = 4,

    /// <summary>退出企业（官方值 <c>5</c>）。</summary>
    Quit = 5,
}

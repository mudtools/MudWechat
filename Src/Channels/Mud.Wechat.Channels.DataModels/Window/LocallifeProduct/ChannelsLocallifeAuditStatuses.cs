// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>团购优惠审核状态常量（官方 <c>audit_status</c> 枚举值，<c>getlocalproduct</c> audit_result）。</summary>
public static class ChannelsLocallifeAuditStatuses
{
    /// <summary>等待审核中。</summary>
    public const int Waiting = 1;

    /// <summary>审核失败。</summary>
    public const int Failed = 2;

    /// <summary>审核成功。</summary>
    public const int Success = 3;
}

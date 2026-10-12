// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Marketing;

/// <summary>优惠券状态常量（官方 <c>status</c> 枚举值，getcoupon 响应与 getcouponlist 筛选）。</summary>
public static class ChannelsCouponStatuses
{
    /// <summary>编辑中（创建后初始状态）。</summary>
    public const int Editing = 1;

    /// <summary>已生效。</summary>
    public const int Active = 2;

    /// <summary>已过期。</summary>
    public const int Expired = 3;

    /// <summary>已作废。</summary>
    public const int Invalid = 4;

    /// <summary>已删除。</summary>
    public const int Deleted = 5;

    /// <summary>过期或作废（官方 getcouponlist 筛选复合值）。</summary>
    public const int ExpiredOrInvalid = 200;
}

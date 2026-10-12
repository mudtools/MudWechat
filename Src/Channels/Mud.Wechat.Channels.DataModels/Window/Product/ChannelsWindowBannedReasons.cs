// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>橱窗商品禁售原因常量（官方 <c>reason</c> 枚举值，<c>getwindowproduct</c>）。</summary>
public static class ChannelsWindowBannedReasons
{
    /// <summary>三级类目在橱窗禁售，或商品在来源处被禁售。</summary>
    public const int CategoryOrSourceBanned = 0;

    /// <summary>商品属于可申请售卖的类目，但商家未完成申请。</summary>
    public const int CategoryApplyRequired = 1;

    /// <summary>商品所属分店未处于营业状态。</summary>
    public const int BranchClosed = 2;
}

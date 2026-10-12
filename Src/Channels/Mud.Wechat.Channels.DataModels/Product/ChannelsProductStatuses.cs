// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

// ---- 枚举常量 ---------------------------------------------------------------

/// <summary>商品线上状态常量（官方 <c>status</c> 枚举值）。</summary>
public static class ChannelsProductStatuses
{
    /// <summary>初始值。</summary>
    public const int Initial = 0;

    /// <summary>上架。</summary>
    public const int Listing = 5;

    /// <summary>回收站。</summary>
    public const int Recycled = 6;

    /// <summary>初始值（草稿）。</summary>
    public const int EditInit = 10;

    /// <summary>自主下架。</summary>
    public const int ManualDelisting = 11;

    /// <summary>资质审核失败。</summary>
    public const int QualificationAuditFailed = 13;

    /// <summary>审核失败。</summary>
    public const int AuditFailed = 14;

    /// <summary>违规下架。</summary>
    public const int ViolationDelisting = 15;

    /// <summary>审核中。</summary>
    public const int Auditing = 16;

    /// <summary>审核通过。</summary>
    public const int AuditPassed = 17;

    /// <summary>待提交审核。</summary>
    public const int PendingAudit = 19;

    /// <summary>平台已下架。</summary>
    public const int PlatformDelisting = 20;
}

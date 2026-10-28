// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.PlatformKf;

/// <summary>平台审核结果常量（官方 <c>platform_review_result</c> 枚举值）。</summary>
public static class ChannelsPlatformKfReviewResults
{
    /// <summary>审核通过，协商一致支持用户。</summary>
    public const int ApprovedSupportUser = 1;

    /// <summary>审核不通过，默认判支持用户。</summary>
    public const int RejectedDefaultSupportUser = 2;

    /// <summary>平台介入处理，判支持商家。</summary>
    public const int IntervenedSupportMerchant = 3;

    /// <summary>平台接入处理，判支持用户。</summary>
    public const int IntervenedSupportUser = 4;

    /// <summary>平台关闭工单。</summary>
    public const int PlatformClosed = 5;
}

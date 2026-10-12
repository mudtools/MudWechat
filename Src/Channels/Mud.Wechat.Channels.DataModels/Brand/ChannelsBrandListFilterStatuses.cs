// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>品牌资质申请列表状态筛选常量（官方 <c>status</c> 请求参数枚举值，<c>getbrandlistlogic</c>——与品牌资质状态枚举语义不同，勿混用）。</summary>
public static class ChannelsBrandListFilterStatuses
{
    /// <summary>获取全部（不传或传 0 返回全部）。</summary>
    public const int All = 0;

    /// <summary>审核中。</summary>
    public const int Auditing = 1;

    /// <summary>审核失败。</summary>
    public const int Rejected = 2;

    /// <summary>审核通过（含即将过期 / 已过期）。</summary>
    public const int Approved = 3;

    /// <summary>已撤回。</summary>
    public const int Withdrawn = 4;
}

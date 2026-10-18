// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>优惠券有效期类型（<c>valid_info.valid_type</c>，请求侧）。</summary>
/// <remarks>
/// <para><b>官方两页值域冲突（照录）</b>：create 页为 1 指定时间 / 2 生效天数；update 页为
/// 0 指定时间 / 1 生效天数。本类同时登记两套：<see cref="TimeRange"/> / <see cref="Days"/> 供 create，
/// <see cref="UpdatePageTimeRange"/> / <see cref="UpdatePageDays"/> 供 update。</para>
/// </remarks>
public static class WxaProductCouponValidTypes
{
    /// <summary>1 指定时间范围生效（create 页口径）。</summary>
    public const int TimeRange = 1;

    /// <summary>2 生效天数（create 页口径）。</summary>
    public const int Days = 2;

    /// <summary>0 指定时间范围生效（update 页口径）。</summary>
    public const int UpdatePageTimeRange = 0;

    /// <summary>1 生效天数（update 页口径）。</summary>
    public const int UpdatePageDays = 1;
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Observability;

/// <summary>
/// 企业微信产品线 Activity 名称常量。
/// </summary>
/// <remarks>
/// 带 <c>Work.</c> 产品线段保证全仓唯一（与公众号 <c>OfficialAccount.</c> 等前缀区分）。
/// </remarks>
public static class WorkActivityNames
{
    /// <summary>回调入站 Activity</summary>
    public const string CallbackRequest = "Work.Callback.Request";

    /// <summary>事件分发 Activity</summary>
    public const string EventHandle = "Work.Event.Handle";

    /// <summary>令牌获取/刷新 Activity</summary>
    public const string TokenAcquire = "Work.Token.Acquire";

    /// <summary>授权编排（换码/撤销）Activity</summary>
    public const string Authorization = "Work.Authorization";
}

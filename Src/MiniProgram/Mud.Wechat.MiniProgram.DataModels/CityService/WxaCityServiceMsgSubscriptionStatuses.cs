// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>长辈就医开通状态（<c>is_subscribed</c>）。</summary>
/// <remarks>
/// <para>官方字段表类型写 <c>boolean</c>，但合法值表给出 1/0 ⇒ 以合法值表为准建模为 <c>int</c>。</para>
/// </remarks>
public static class WxaCityServiceMsgSubscriptionStatuses
{
    /// <summary>1 已开通。</summary>
    public const int Subscribed = 1;

    /// <summary>0 未开通。</summary>
    public const int Unsubscribed = 0;
}

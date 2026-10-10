// -----------------------------------------------------------------------
//  作者：Mud Studio 版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Callback;

/// <summary>
/// 回调路由与注册表键约定（企业微信 / 公众号共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何落叶层</b>：通配键是「回调注册表分桶」与「路由兜底」的共同约定，被
/// <see cref="WechatCallbackTypeRegistry{T}"/> 的 <c>EnumerateWithWildcard</c> 直接消费；
/// 若留在某条产品线内，注册表基类就无法下沉（叶层不得反向引用产品线）。
/// </para>
/// <para>
/// 各产品线的配置面仍保留自己的常量名（如企微 <c>WechatCallbackOptions.WildcardAppKey</c>）——
/// 但取值<b>指向本常量</b>，单一来源、不允许各写一份字面量。
/// </para>
/// </remarks>
public static class WechatCallbackRouteKeys
{
    /// <summary>
    /// 通配应用键：承接「无 AppKey 形态」的回调路由（如企微通讯录同步助手），
    /// 并作为「全局处理器/拦截器」的注册桶。
    /// </summary>
    /// <remarks>
    /// <c>"*"</c> 不满足 <c>WechatAppKeyValidator</c> 的应用键形状（首字符须字母/数字），
    /// 配置校验对其显式豁免——也正因形状非法，通配键与任何真实应用键不可能碰撞。
    /// </remarks>
    public const string Wildcard = "*";
}

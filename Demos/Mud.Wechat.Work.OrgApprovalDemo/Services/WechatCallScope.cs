// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.OrgApprovalDemo.Configuration;

namespace Mud.Wechat.Work.OrgApprovalDemo.Services;

/// <summary>
/// 应用作用域的唯一入口（方案 §4.1 的「本工程不变量」）：<b>任何 SDK 调用都包在显式应用作用域里</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不能靠「默认应用」</b>：两个应用的令牌管理器各自独立（令牌键含 AppKey），但「当前用哪个应用」
/// 是 <c>AsyncLocal</c> 状态。不显式进入作用域时会落到默认应用 —— 而默认应用是按<b>注册顺序</b>
/// 推出来的（名单里第一个），于是同一段代码在「调整了 Program.cs 里的应用顺序」后就会拿审批 secret
/// 去写通讯录，报出来的却是权限错误，非常难定位。
/// </para>
/// <para>
/// <b>为什么用 <c>UseAppScope</c> 而不是 <c>UseApp</c> / <c>BeginScope</c></b>：后两者已 <c>[Obsolete]</c>，
/// 且 <c>UseApp</c> 不归还上下文 —— 泄漏后同一异步流上的后续调用会带着上一个应用的身份出去。
/// <c>UseAppScope</c> 返回 <see cref="IDisposable"/>，释放时自动归还。
/// </para>
/// </remarks>
/// <param name="switcher">多应用上下文切换器（SDK 注册为单例）。</param>
public sealed class WechatCallScope(IWechatAppContextSwitcher switcher)
{
    private readonly IWechatAppContextSwitcher _switcher = switcher;

    /// <summary>进入「通讯录同步助手」作用域（读写部门/成员、<c>user/list_id</c> 游标）。</summary>
    public IDisposable UseContacts() => _switcher.UseAppScope(AppScopes.Contacts);

    /// <summary>进入「审批应用」作用域（模板详情/创建/更新、审批单回捞）。</summary>
    public IDisposable UseApproval() => _switcher.UseAppScope(AppScopes.Approval);
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Agent.Events;
using Mud.Wechat.Work.Abstractions.Authentication;

namespace Mud.Wechat.Work.AI.Agents;

/// <summary>
/// 企微工具作用域工厂：把中立回合管线的 <see cref="ITurnScopeFactory"/> 接缝桥接到
/// <see cref="IWechatAppContextSwitcher"/> 的「应用 / 应用 + 企业」两级作用域。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么直接实现 Mud.AI 的端口</b>：<see cref="ITurnScopeFactory"/> 就是上游为「租户切换 / 预算 / 工具执行上下文」
/// 预留的<b>产品事实接缝</b>（<c>TurnScopeContext</c> 字段全中立）；飞书侧即直接实现该端口。另建
/// <c>IWechatAiScopeFactory</c> 会形成「第二个上下文槽位」（宿主该初始化哪个？），故本方案不建 WeChat 端口。
/// </para>
/// <para>
/// <b>切换规则</b>（企业维度来自事件层写入的 <see cref="IWechatAiCorpContextAccessor"/>）：
/// <list type="number">
/// <item>携带企业维度（第三方 / 代开发）⇒ <c>UseCorpScope(appKey, authCorpId)</c>（企业令牌 scope 唯一来源；永久授权码由
/// <c>CorpTokenManager</c> 按 <c>(appKey, authCorpId)</c> 自行解析，不进入本类型）；</item>
/// <item>仅应用维度（自建应用）⇒ <c>UseAppScope(appKey)</c>；</item>
/// <item>应用维度缺失 ⇒ <c>UseDefaultAppScope()</c>（缺省应用，与多应用注册表的默认语义一致）。</item>
/// </list>
/// </para>
/// <para>
/// <b>返回的作用域必须可嵌套还原</b>：两个入口都返回「释放时自动归还进入前上下文」的一次性作用域，
/// 嵌套回合（续跑轮）据此不会读到外层租户 —— 多租户下这是越权读取的防线，不得改成手工 <c>SetCorp</c>/<c>ClearCorp</c>。
/// </para>
/// </remarks>
public sealed class WechatWorkAiScopeFactory : ITurnScopeFactory
{
    private readonly IWechatAppContextSwitcher _switcher;
    private readonly IWechatAiCorpContextAccessor _corpContext;

    /// <summary>初始化作用域工厂。</summary>
    /// <param name="switcher">多应用上下文切换器（DI 单例）。</param>
    /// <param name="corpContext">事件层写入的工具身份上下文（DI 单例）。</param>
    /// <exception cref="ArgumentNullException">任一参数为 <see langword="null"/>。</exception>
    public WechatWorkAiScopeFactory(IWechatAppContextSwitcher switcher, IWechatAiCorpContextAccessor corpContext)
    {
        _switcher = switcher ?? throw new ArgumentNullException(nameof(switcher));
        _corpContext = corpContext ?? throw new ArgumentNullException(nameof(corpContext));
    }

    /// <inheritdoc />
    public IDisposable BeginToolScope(TurnScopeContext context)
    {
        var principal = _corpContext.Current;
        var appKey = string.IsNullOrWhiteSpace(context.TenantKey)
            ? principal?.AppKey
            : context.TenantKey;

        if (principal is not null && principal.HasCorpScope)
        {
            return string.IsNullOrWhiteSpace(appKey)
                ? _switcher.UseDefaultAppScope()
                : _switcher.UseCorpScope(appKey!, principal.AuthCorpId!);
        }

        return string.IsNullOrWhiteSpace(appKey)
            ? _switcher.UseDefaultAppScope()
            : _switcher.UseAppScope(appKey!);
    }
}

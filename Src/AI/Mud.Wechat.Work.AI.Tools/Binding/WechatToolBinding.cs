// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools;

/// <summary>
/// 工具执行绑定（剖面 <c>BindingTypeName</c> 槽的实参类型）：把「哪个应用、哪个企业」带进执行链，
/// 使执行器在不读进程级单例状态的前提下推导令牌归属。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不直接把 <c>appKey</c> / <c>authCorpId</c> 塞进每个工具的 Schema</b>：身份是<b>环境</b>而非<b>参数</b>——
/// 让模型看见身份参数等于把越权入口写进工具签名；绑定由宿主装配期给定，模型不可触碰。
/// </para>
/// <para>
/// <b>与作用域端口的分工</b>：回合级租户切换由 <c>ITurnScopeFactory</c>（企微实现 <c>WechatWorkAiScopeFactory</c>）
/// 在工具执行前完成；本类型只携带「事实」，不建第二个上下文槽位。
/// </para>
/// </remarks>
public sealed class WechatToolBinding
{
    /// <summary>初始化工具绑定。</summary>
    /// <param name="appKey">应用键（多应用注册表键；不得为空白）。</param>
    /// <param name="authCorpId">授权企业 ID（企业级令牌的 scope 键；可为空表示未限定企业）。</param>
    /// <param name="permanentCode">代开发永久授权码（服务商代开发场景的企业级凭据；其余场景为 <see langword="null"/>）。</param>
    /// <exception cref="ArgumentException"><paramref name="appKey"/> 为空白。</exception>
    public WechatToolBinding(string appKey, string? authCorpId = null, string? permanentCode = null)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentException("appKey 不能为空（绑定的应用归属是令牌路由的前提）。", nameof(appKey));
        }

        AppKey = appKey;
        AuthCorpId = authCorpId;
        PermanentCode = permanentCode;
    }

    /// <summary>应用键。</summary>
    public string AppKey { get; }

    /// <summary>授权企业 ID（企业级令牌的 scope 键）。</summary>
    public string? AuthCorpId { get; }

    /// <summary>代开发永久授权码（企业级凭据）。</summary>
    public string? PermanentCode { get; }
}

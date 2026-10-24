// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Agents;

/// <summary>
/// 回合事实（<c>TurnScopeContext.Facts</c>）在企微侧的<b>键字面量单一来源</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要有 Facts 通道</b>：<c>TurnScopeContext</c> 的中立字段（<c>TenantKey</c>/<c>UserId</c>）不足以表达
/// 企微的「应用 + 企业」两级作用域——企业维度此前只能靠旁路环境量（<see cref="IWechatAiCorpContextAccessor"/>）传递，
/// 而环境量是<b>隐式</b>输入：忘写一处就静默退化为「只切应用」，表现为第三方/代开发场景下工具取到错误令牌。
/// Facts 把这份事实放进回合管线的显式参数里，作用域工厂优先读它，环境量退为软缺席回退。
/// </para>
/// <para>
/// <b>键以 <c>wechat.</c> 前缀区分</b>：Facts 是同进程可能被多产品线共用的字典，前缀保证跨线共存时不互相覆盖
/// （中立管线不认识键语义，只负责搬运）。
/// </para>
/// </remarks>
internal static class WechatTurnFacts
{
    /// <summary>应用维度标识（多应用注册表键）。</summary>
    public const string AppKey = "wechat.app_key";

    /// <summary>授权方（企业）CorpId（仅第三方/代开发；缺席 = 自建应用，无企业级 scope）。</summary>
    public const string AuthCorpId = "wechat.auth_corp_id";

    /// <summary>发起人的开放 id（可空；工具身份轴的用户维度）。</summary>
    public const string OpenId = "wechat.open_id";

    /// <summary>从事实字典中读取一个非空白值（空白视为缺席）。</summary>
    /// <param name="facts">事实字典（可空）。</param>
    /// <param name="key">键（取值限本类常量）。</param>
    /// <returns>非空白值；缺席或空白时为 <see langword="null"/>。</returns>
    public static string? Read(IReadOnlyDictionary<string, string?>? facts, string key)
        => facts is not null && facts.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;
}

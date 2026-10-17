// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.AI.AgentTools;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 授权判定输入：一次工具调用的「谁 / 对哪个应用与企业 / 调什么 / 传了什么摘要」。
/// </summary>
/// <remarks>
/// <para>
/// <b>不含原始入参</b>：授权器与审计只需要<b>摘要</b>（<see cref="ArgumentsDigest"/>，键名排序 + 敏感键掩码），
/// 把原始值交给外部授权器等于把客户数据复制到审计/日志面；确需明文的自定义授权器应由宿主在回合作用域内自取。
/// </para>
/// <para>
/// <b>不含令牌</b>：身份是「应用 + 企业」这一事实，令牌由执行链在 <c>UseCorpScope</c> 后自行获取；
/// 任何把 access_token 传给授权器的设计都会把凭据扩散到业务判断面。
/// </para>
/// </remarks>
public sealed class WechatToolAuthorizationContext
{
    /// <summary>初始化授权上下文。</summary>
    /// <param name="toolName">工具名（模型可见契约名）。</param>
    /// <param name="appKey">应用键（多应用注册表键）。</param>
    /// <param name="authCorpId">授权企业 ID（可空 = 未限定企业）。</param>
    /// <param name="risk">工具风险档。</param>
    /// <param name="requiredScopes">工具声明的权限点（生态词表）。</param>
    /// <param name="argumentsDigest">入参审计摘要（已掩码）。</param>
    public WechatToolAuthorizationContext(
        string toolName,
        string appKey,
        string? authCorpId,
        WechatToolRisk risk,
        IReadOnlyList<string> requiredScopes,
        string argumentsDigest)
    {
        ToolName = toolName;
        AppKey = appKey;
        AuthCorpId = authCorpId;
        Risk = risk;
        RequiredScopes = requiredScopes;
        ArgumentsDigest = argumentsDigest;
    }

    /// <summary>工具名。</summary>
    public string ToolName { get; }

    /// <summary>应用键。</summary>
    public string AppKey { get; }

    /// <summary>授权企业 ID（可空）。</summary>
    public string? AuthCorpId { get; }

    /// <summary>风险档。</summary>
    public WechatToolRisk Risk { get; }

    /// <summary>工具声明的权限点。</summary>
    public IReadOnlyList<string> RequiredScopes { get; }

    /// <summary>入参审计摘要（键名排序、敏感键掩码、超长截断）。</summary>
    public string ArgumentsDigest { get; }
}

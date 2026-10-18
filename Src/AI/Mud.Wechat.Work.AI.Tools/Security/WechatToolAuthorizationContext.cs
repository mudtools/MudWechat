// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.AI.Abstractions.Tools;
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
/// <para>
/// <b>租户是回合事实，不是装配事实</b>：<see cref="AppKey"/> / <see cref="AuthCorpId"/> 取<b>回合上下文</b>
/// （事件层写入的 <c>IWechatAiCorpContextAccessor</c>），装配期绑定只作兜底。一个 Agent 进程服务多个应用/企业时，
/// 用装配期绑定做鉴权会让「授权判定在 A 企业、实际执行在 B 企业」——行级鉴权因此完全失真。
/// </para>
/// <para>
/// <b>发起人是一等事实</b>：<see cref="Principal"/> 携带 <c>OpenId</c> 等回合身份，授权器据此做「这个成员能不能查
/// 这个部门 / 给这个客户发消息」的判定。企微通讯录的可见范围本身就是行级的，缺这一维就只能做应用级放行。
/// </para>
/// </remarks>
public sealed class WechatToolAuthorizationContext
{
    /// <summary>初始化授权上下文。</summary>
    /// <param name="toolName">工具名（模型可见契约名）。</param>
    /// <param name="appKey">应用键（<b>回合租户</b>；装配期绑定只作兜底）。</param>
    /// <param name="authCorpId">授权企业 ID（回合租户；可空 = 未限定企业）。</param>
    /// <param name="risk">工具风险档。</param>
    /// <param name="requiredScopes">工具声明的权限点（生态词表）。</param>
    /// <param name="argumentsDigest">入参审计摘要（已掩码）。</param>
    /// <param name="principal">回合身份（可空 = 上下文未建立）；发起人维度是行级鉴权的唯一来源。</param>
    /// <param name="preview">审批现场的人类可读预览（可空；<b>不含原始载荷</b>，与摘要分工不同）。</param>
    public WechatToolAuthorizationContext(
        string toolName,
        string appKey,
        string? authCorpId,
        WechatToolRisk risk,
        IReadOnlyList<string> requiredScopes,
        string argumentsDigest,
        WechatToolPrincipal? principal = null,
        WechatToolApprovalPreview? preview = null)
    {
        ToolName = toolName;
        AppKey = appKey;
        AuthCorpId = authCorpId;
        Risk = risk;
        RequiredScopes = requiredScopes;
        ArgumentsDigest = argumentsDigest;
        Principal = principal;
        Preview = preview;
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

    /// <summary>回合身份（可空 = 回合上下文未建立；<b>不是</b>装配期事实）。</summary>
    public WechatToolPrincipal? Principal { get; }

    /// <summary>
    /// 审批现场的人类可读预览（可空）。
    /// </summary>
    /// <remarks>
    /// <b>只给审批人看，不进审计</b>：审计记录只存 <see cref="ArgumentsDigest"/>；
    /// 二者生命周期不同（预览随审批现场消失，审计长期留存），不得合并存储。
    /// </remarks>
    public WechatToolApprovalPreview? Preview { get; }

    /// <summary>发起人（成员 userid；来自回合身份，可空）。</summary>
    public string? SenderId => Principal?.OpenId;
}

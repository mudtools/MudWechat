// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Agent.Tools;
using Mud.AI.Agent.Tools.Errors;

namespace Mud.Wechat.AI.Agent.Results;

/// <summary>
/// 微信生态业务码（<c>errcode</c>）→ 中立错误读数（<see cref="ToolApiFailure"/>）的<b>唯一码表</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么码表落在这里而不是中立包</b>：中立层（<c>Mud.AI.Agent</c>）刻意不认识任何产品码表
/// （见 <c>ToolErrorClassifier</c> 的 remarks）——上游化时码表会随之被带走。本类型属<b>产品侧</b>
/// （<c>Mud.Wechat.*</c> 包），承担「读自家信封」的职责，符合「产品码 → 段位由产品侧完成」的分工。
/// </para>
/// <para>
/// <b>为什么是跨线单表而不是企微线独占</b>：<c>errcode</c> 是微信生态共享码空间（企微 / 公众号 / 小程序 /
/// 小店共用同一套全局错误码语义）。按线各写一份会让同一码在不同工具面得到不同处置，而模型的自我修正
/// 完全依赖「同一事实同一读数」。
/// </para>
/// <para>
/// <b>码表纪律（有实证才收录）</b>：只收官方全局错误码文档中语义明确、且工具面确有恢复动作的码。
/// 未收录的码一律返回 <see langword="null"/> ⇒ 中立层回落 <c>api/upstream_error</c>。猜测性码表比没有码表
/// 更危险：它会把模型引向错误的处置（例如把「参数错」告诉它「稍后重试」）。
/// </para>
/// <para>
/// <b>只影响「模型怎么读」，不影响「系统怎么做」</b>：本码表不驱动自动重试（重试由宿主策略决定），
/// 只产出分类读数与恢复候选文案；写工具永远不因分类而被重试。
/// </para>
/// </remarks>
public static class WechatApiErrorCatalog
{
    /// <summary>系统繁忙（官方建议稍后重试）。</summary>
    public const int SystemBusy = -1;

    /// <summary>access_token 过期（SDK 已按需自动刷新；仍返回说明刷新链路异常）。</summary>
    public const int AccessTokenExpired = 42001;

    /// <summary>不合法的 access_token。</summary>
    public const int InvalidAccessToken = 40014;

    /// <summary>接口调用超过限制（应用级频控）。</summary>
    public const int ApiFrequencyLimited = 45009;

    /// <summary>接口并发调用超过限制。</summary>
    public const int ApiConcurrencyLimited = 45033;

    /// <summary>无权限访问该资源（成员 / 部门 / 标签等）。</summary>
    public const int NoPrivilege = 60011;

    /// <summary>API 无权限（应用未获该接口的授权）。</summary>
    public const int ApiForbidden = 48002;

    /// <summary>不合法的 UserID。</summary>
    public const int InvalidUserId = 40003;

    /// <summary>UserID 不存在。</summary>
    public const int UserIdNotFound = 60111;

    /// <summary>user / party / tag 参数均不合法。</summary>
    public const int InvalidUserPartyTag = 81013;

    private static readonly Dictionary<int, ToolApiFailure> Map = new()
    {
        [SystemBusy] = new ToolApiFailure(
            ToolErrorCategory.Retryable,
            ToolErrorSubtype.Transient,
            ["稍后重试一次；连续失败时告知用户服务繁忙，不要反复重试"]),

        [AccessTokenExpired] = new ToolApiFailure(
            ToolErrorCategory.Retryable,
            ToolErrorSubtype.Transient,
            ["重试一次（令牌刷新链路通常可在一次重试内恢复）", "仍失败时告知用户应用授权配置异常，需管理员介入"]),

        [InvalidAccessToken] = new ToolApiFailure(
            ToolErrorCategory.Retryable,
            ToolErrorSubtype.Transient,
            ["重试一次（令牌刷新链路通常可在一次重试内恢复）", "仍失败时告知用户应用授权配置异常，需管理员介入"]),

        [ApiFrequencyLimited] = new ToolApiFailure(
            ToolErrorCategory.Retryable,
            ToolErrorSubtype.RateLimited,
            ["缩小查询范围（减少条数 / 收窄时间区间 / 指定部门）后重试", "不要以相同参数立即重试"]),

        [ApiConcurrencyLimited] = new ToolApiFailure(
            ToolErrorCategory.Retryable,
            ToolErrorSubtype.RateLimited,
            ["减少同一轮的并行调用后重试", "不要以相同参数立即重试"]),

        [NoPrivilege] = new ToolApiFailure(
            ToolErrorCategory.Authorization,
            ToolErrorSubtype.AuthorizationDenied,
            ["当前应用或成员没有该数据的查看权限，重试无效", "告知用户需由管理员在企微后台开通对应权限或可见范围"]),

        [ApiForbidden] = new ToolApiFailure(
            ToolErrorCategory.Authorization,
            ToolErrorSubtype.AuthorizationDenied,
            ["该应用未获此接口授权，重试无效", "告知用户联系管理员在企微后台开通该接口权限"]),

        [InvalidUserId] = new ToolApiFailure(
            ToolErrorCategory.Validation,
            ToolErrorSubtype.InvalidArgs,
            ["确认 userid 拼写；不确定时先用 contacts.user.resolve 按手机号/邮箱解析"]),

        [UserIdNotFound] = new ToolApiFailure(
            ToolErrorCategory.Validation,
            ToolErrorSubtype.InvalidArgs,
            ["该成员不在当前应用的可见范围内，或已离职", "改用其他成员 ID 重试，不要重复提交同一 ID"]),

        [InvalidUserPartyTag] = new ToolApiFailure(
            ToolErrorCategory.Validation,
            ToolErrorSubtype.InvalidArgs,
            ["核对 userid / 部门 ID / 标签 ID 的取值来源，改用只读列表接口获取合法 ID"]),
    };

    /// <summary>把业务码映射为中立的错误读数（未收录的码返回 <see langword="null"/>）。</summary>
    /// <param name="code">业务码（<c>errcode</c>）。</param>
    /// <returns>分类读数；未收录时为 <see langword="null"/>（中立层回落 <c>api/upstream_error</c>）。</returns>
    public static ToolApiFailure? Classify(int? code)
        => code is { } value && Map.TryGetValue(value, out var failure) ? failure : null;

    /// <summary>取该码的恢复候选文案（供错误正文追加一行「建议」，无则该码返回空串）。</summary>
    /// <param name="code">业务码。</param>
    /// <returns>「建议：…」行；未收录或无可选建议时为空串。</returns>
    public static string HintOf(int? code)
    {
        var failure = Classify(code);
        if (failure?.Suggestions is not { Count: > 0 } suggestions)
        {
            return string.Empty;
        }

        return "建议：" + string.Join("；", suggestions);
    }

    /// <summary>已收录的业务码（升序；守卫据此冻结码表，防随手增删）。</summary>
    public static IReadOnlyList<int> Codes
    {
        get
        {
            var codes = new List<int>(Map.Keys);
            codes.Sort();
            return codes;
        }
    }
}

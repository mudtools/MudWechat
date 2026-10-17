// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 授权判定结果（<b>三态之外只有两态</b>：放行或拒绝——挂起态属于 HITL 审批，不进本类型）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么没有「放行并记待确认」</b>：审批挂起是<b>状态机</b>（需要存储、重投递、批准通道），
/// 授权器每次被问都应是纯判定；把挂起塞进返回值会让「同一次调用问两次」得到两个答案。
/// HITL 由层 1 审批端口承接（S3），本类型保持无状态。
/// </para>
/// <para>
/// <b>拒绝理由不出模型</b>：<see cref="DenialReason"/> 只进宿主日志/审计，回填模型的是中性文案
/// （不泄露权限点名称、也不暴露「哪些工具存在」）。
/// </para>
/// </remarks>
public readonly struct WechatToolAuthorizationResult
{
    private WechatToolAuthorizationResult(bool isAllowed, string? denialReason)
    {
        IsAllowed = isAllowed;
        DenialReason = denialReason;
    }

    /// <summary>是否放行。</summary>
    public bool IsAllowed { get; }

    /// <summary>拒绝理由（仅宿主可见；放行时为 <see langword="null"/>）。</summary>
    public string? DenialReason { get; }

    /// <summary>放行。</summary>
    /// <returns>放行结果。</returns>
    public static WechatToolAuthorizationResult Allow() => new(true, null);

    /// <summary>拒绝。</summary>
    /// <param name="reason">宿主可见理由（如「未授予 contacts.write 权限点」）。</param>
    /// <returns>拒绝结果。</returns>
    /// <exception cref="ArgumentException"><paramref name="reason"/> 为空白。</exception>
    public static WechatToolAuthorizationResult Deny(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("拒绝理由不能为空（审计需要可读原因）。", nameof(reason));
        }

        return new WechatToolAuthorizationResult(false, reason);
    }
}

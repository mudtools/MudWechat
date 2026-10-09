// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions;

/// <summary>
/// 第三方平台令牌（<c>component_access_token</c>）的运行时状态。
/// </summary>
/// <remarks>
/// <b>不可变值对象</b>：刷新即<b>整体替换</b>（而非就地改字段），
/// 避免并发读写看到「新令牌 + 旧到期时间」这种不一致组合。
/// </remarks>
public sealed class ComponentAccessTokenState
{
    /// <summary>创建状态。</summary>
    /// <param name="accessToken">令牌内容。</param>
    /// <param name="expiresAt">失效时刻（<b>UTC</b>）。</param>
    public ComponentAccessTokenState(string? accessToken, DateTimeOffset expiresAt)
    {
        AccessToken = accessToken;
        ExpiresAt = expiresAt;
    }

    /// <summary>令牌内容（可能为 <c>null</c>／空白，表示未取得）。</summary>
    public string? AccessToken { get; }

    /// <summary>失效时刻（UTC）。</summary>
    public DateTimeOffset ExpiresAt { get; }
}

/// <summary>
/// 第三方平台令牌的<b>刷新策略</b>（纯函数，便于逐边界测试）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何把策略独立出来</b>：这套判定决定「何时打官方接口」「何时拿旧令牌继续用」，
/// 是本线最容易写错、且错了以后表现为<b>间歇性失败</b>的地方。抽成纯函数后，
/// 每个边界（未取得 / 已过期 / 进入提前窗口 / 窗口外）都能在<b>不触网</b>的前提下逐条锁定。
/// </para>
/// <para>
/// <b>两种判定的分工</b>：<see cref="IsUsable"/> 回答「这把令牌<b>现在</b>还能不能拿去调接口」，
/// <see cref="ShouldRefresh"/> 回答「是否<b>该去换</b>新的」。两者在提前窗口内<b>同时为真</b>
/// （旧令牌还能用，但应尽快换）—— 这正是官方建议「1 小时 50 分刷新」的语义。
/// </para>
/// </remarks>
public static class ComponentAccessTokenPolicy
{
    /// <summary>
    /// 判断令牌是否<b>当前可用</b>。
    /// </summary>
    /// <param name="state">当前状态（<c>null</c> 表示尚未取得）。</param>
    /// <param name="nowUtc">当前 UTC 时间。</param>
    /// <returns>可用为 <c>true</c>。</returns>
    public static bool IsUsable(ComponentAccessTokenState? state, DateTimeOffset nowUtc)
        => IsUsable(state?.AccessToken, state?.ExpiresAt ?? default, nowUtc);

    /// <summary>
    /// 判断令牌是否<b>当前可用</b>（<b>基础重载</b>：直接给令牌与到期时刻）。
    /// </summary>
    /// <param name="accessToken">令牌内容。</param>
    /// <param name="expiresAt">失效时刻（UTC）。</param>
    /// <param name="nowUtc">当前 UTC 时间。</param>
    /// <remarks>
    /// <b>为何做成基础重载</b>：授权方接口调用令牌（<c>authorizer_access_token</c>）与平台令牌
    /// 的<b>时效语义完全一致</b>（同样是 2 小时、同样需要提前刷新）⇒ 两条链共用同一份经过
    /// 逐边界测试的判定，而不是各写一份「看起来一样」的实现。
    /// </remarks>
    public static bool IsUsable(string? accessToken, DateTimeOffset expiresAt, DateTimeOffset nowUtc)
        => !string.IsNullOrWhiteSpace(accessToken) && nowUtc < expiresAt;

    /// <summary>
    /// 判断是否<b>应当刷新</b>令牌。
    /// </summary>
    /// <param name="state">当前状态（<c>null</c> 表示尚未取得）。</param>
    /// <param name="nowUtc">当前 UTC 时间。</param>
    /// <param name="refreshLead">
    /// 提前刷新窗口；<c>null</c> 取官方建议值（<see cref="OpenPlatformContract.RecommendedRefreshLeadSeconds"/> 秒）。
    /// <b>负值按 0 处理</b>（见 remarks）。
    /// </param>
    /// <returns>应刷新为 <c>true</c>。</returns>
    /// <remarks>
    /// <b>为何把负值按 0 处理</b>：负窗口的数学语义是「过期<b>之后</b>再刷新」，
    /// 那会让刷新永远慢于失效（每次过期后都有一段失败窗口）—— 属于必然错误的配置，
    /// 静默按「不提前」处理比让它按字面生效更安全（也避免在这里抛异常打断启动）。
    /// </remarks>
    public static bool ShouldRefresh(
        ComponentAccessTokenState? state,
        DateTimeOffset nowUtc,
        TimeSpan? refreshLead = null)
        => ShouldRefresh(state?.AccessToken, state?.ExpiresAt ?? default, nowUtc, refreshLead);

    /// <summary>
    /// 判断是否<b>应当刷新</b>令牌（<b>基础重载</b>：直接给令牌与到期时刻）。
    /// </summary>
    /// <param name="accessToken">令牌内容（<c>null</c>/空白视为「尚未取得」⇒ 应刷新）。</param>
    /// <param name="expiresAt">失效时刻（UTC）。</param>
    /// <param name="nowUtc">当前 UTC 时间。</param>
    /// <param name="refreshLead">
    /// 提前刷新窗口；<c>null</c> 取官方建议值（<see cref="OpenPlatformContract.RecommendedRefreshLeadSeconds"/> 秒）。
    /// <b>负值按 0 处理</b>。
    /// </param>
    public static bool ShouldRefresh(
        string? accessToken,
        DateTimeOffset expiresAt,
        DateTimeOffset nowUtc,
        TimeSpan? refreshLead = null)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return true;
        }

        var lead = refreshLead ?? TimeSpan.FromSeconds(OpenPlatformContract.RecommendedRefreshLeadSeconds);
        if (lead < TimeSpan.Zero)
        {
            lead = TimeSpan.Zero;
        }

        return nowUtc >= expiresAt - lead;
    }
}

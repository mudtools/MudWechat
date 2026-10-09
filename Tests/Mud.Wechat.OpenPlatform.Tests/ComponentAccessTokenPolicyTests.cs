// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// 第三方平台令牌刷新策略的<b>逐边界</b>锁定（纯函数，不触网）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组锁的是「什么时候该换令牌」</b>：这套判定写错的后果是<b>间歇性 40001</b>
/// （临界期内用在途的旧令牌），而不是稳定报错 —— 故每个边界都用显式用例钉死，
/// 尤其是「恰好进入窗口」与「恰好差一秒」两侧。
/// </para>
/// </remarks>
public class ComponentAccessTokenPolicyTests
{
    private static readonly DateTimeOffset Now = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero);

    /// <summary>尚未取得令牌 ⇒ 必须刷新，且不可用。</summary>
    [Fact]
    public void NullState_ShouldRefreshAndBeUnusable()
    {
        ComponentAccessTokenPolicy.ShouldRefresh(null, Now).Should().BeTrue();
        ComponentAccessTokenPolicy.IsUsable(null, Now).Should().BeFalse();
    }

    /// <summary>
    /// <b>有状态但令牌为空/空白</b> ⇒ 仍须刷新、仍不可用（不可把「空壳状态」当成有效缓存）。
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BlankToken_ShouldRefreshAndBeUnusable(string? token)
    {
        var state = new ComponentAccessTokenState(token, Now.AddHours(2));

        ComponentAccessTokenPolicy.ShouldRefresh(state, Now).Should().BeTrue();
        ComponentAccessTokenPolicy.IsUsable(state, Now).Should().BeFalse();
    }

    /// <summary>窗口外且未过期 ⇒ 不刷新，可用（避免无谓打官方接口）。</summary>
    [Fact]
    public void BeforeRefreshWindow_ShouldNotRefresh_ButBeUsable()
    {
        var state = new ComponentAccessTokenState("token-1", Now.AddMinutes(30));

        ComponentAccessTokenPolicy.ShouldRefresh(state, Now).Should().BeFalse();
        ComponentAccessTokenPolicy.IsUsable(state, Now).Should().BeTrue();
    }

    /// <summary>
    /// <b>边界两侧</b>：恰好进入提前窗口 ⇒ 刷新；差一秒未进入 ⇒ 不刷新。
    /// </summary>
    /// <remarks>
    /// 判定用的是 <c>now &gt;= ExpiresAt - lead</c>：进入窗口的<b>那一瞬</b>就该换，
    /// 早不换会吃掉余量、晚换会留出失效缺口。
    /// </remarks>
    [Fact]
    public void RefreshWindowBoundary_ShouldRefreshInclusively()
    {
        var lead = TimeSpan.FromMinutes(10);
        var expiresAt = Now.AddMinutes(20);   // 距离失效 20 分钟

        var oneSecondBeforeWindow = new ComponentAccessTokenState("token-1", expiresAt);
        ComponentAccessTokenPolicy
            .ShouldRefresh(oneSecondBeforeWindow, expiresAt - lead - TimeSpan.FromSeconds(1), lead)
            .Should().BeFalse("距窗口还差一秒 ⇒ 不刷新");

        var exactlyAtWindow = new ComponentAccessTokenState("token-1", expiresAt);
        ComponentAccessTokenPolicy
            .ShouldRefresh(exactlyAtWindow, expiresAt - lead, lead)
            .Should().BeTrue("恰好进入窗口即刷新（含端点）");

        // 进入窗口后旧令牌**仍可用**（这正是「提前刷新」而非「提前失效」的语义）。
        ComponentAccessTokenPolicy
            .IsUsable(exactlyAtWindow, expiresAt - lead)
            .Should().BeTrue("提前刷新窗口内旧令牌依然可用 —— 否则会白造一段失败窗口");
    }

    /// <summary>已过期 ⇒ 必须刷新且不可用；<b>到期瞬间</b>即视为不可用（严格小于）。</summary>
    [Fact]
    public void ExpiredOrAtExpiry_ShouldRefreshAndBeUnusable()
    {
        var state = new ComponentAccessTokenState("token-1", Now);

        ComponentAccessTokenPolicy.ShouldRefresh(state, Now).Should().BeTrue();
        ComponentAccessTokenPolicy.IsUsable(state, Now).Should().BeFalse(
            "命中失效时刻即不可用（判定为严格小于，不给自己留边界侥幸）");

        ComponentAccessTokenPolicy.IsUsable(state, Now.AddSeconds(1)).Should().BeFalse();
    }

    /// <summary>
    /// <b>不传窗口时取官方建议值</b>（600 秒 / 10 分钟）。
    /// </summary>
    [Fact]
    public void DefaultLead_ShouldComeFromOfficialContract()
    {
        var expiresAt = Now.AddMinutes(20);

        // 距失效 9 分钟（< 10 分钟窗口）⇒ 默认策略应判「该刷新」。
        var inside = new ComponentAccessTokenState("token-1", expiresAt);
        ComponentAccessTokenPolicy.ShouldRefresh(inside, expiresAt.AddMinutes(-9)).Should().BeTrue();

        // 距失效 11 分钟（> 10 分钟窗口）⇒ 不该刷新。
        var outside = new ComponentAccessTokenState("token-1", expiresAt);
        ComponentAccessTokenPolicy.ShouldRefresh(outside, expiresAt.AddMinutes(-11)).Should().BeFalse();

        OpenPlatformContract.RecommendedRefreshLeadSeconds.Should().Be(600, "默认窗口即官方建议的提前 10 分钟");
    }

    /// <summary>
    /// <b>负窗口按 0 处理</b>：不得变成「过期之后才刷新」（那必然留出失败窗口）。
    /// </summary>
    [Fact]
    public void NegativeLead_ShouldBeTreatedAsZero()
    {
        var state = new ComponentAccessTokenState("token-1", Now.AddSeconds(1));

        ComponentAccessTokenPolicy
            .ShouldRefresh(state, Now, TimeSpan.FromHours(-1))
            .Should().BeFalse("按 0 处理 ⇒ 仅在到期时刻才刷新（若按字面生效，这里会变成 true 之外的怪异行为）");

        ComponentAccessTokenPolicy
            .ShouldRefresh(state, Now.AddSeconds(1), TimeSpan.FromHours(-1))
            .Should().BeTrue("到期即刷新");
    }
}

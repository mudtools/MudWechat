// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.AllInOne.Demo;

/// <summary>
/// 线激活台账：记录「哪条线装上了、挂在哪个路由、没装的原因」。
/// </summary>
/// <remarks>
/// <para>
/// <b>它同时是 /healthz 与 /routes 的数据源</b>，也是启动横幅的内容。多线共存最大的风险不是装不上，
/// 而是「以为装上了其实没装」—— 少配一个环境变量时 SDK 不会报错（整块注册被宿主跳过），
/// 而读者照着 README 推送会得到 404，很容易误读成 SDK 的路由缺陷。
/// </para>
/// <para>
/// <b>路由为什么由本台账持有而不是从端点表读</b>：各线的回调中间件是经典
/// <c>RequestDelegate</c> 形态（不进 DI、不注册端点），<c>EndpointDataSource</c> 里<b>看不到它们</b>。
/// 故 /routes 把「中间件承载的回调路由」与「端点表里的真实端点」分两段列出 —— 这个区分本身就是要演示的事实。
/// </para>
/// </remarks>
internal sealed class AllInOneLines
{
    private readonly List<LineEntry> _entries = new();

    /// <summary>记一条已激活的线。</summary>
    /// <param name="line">线名（与 <c>Demos/README.md</c> 的索引同名）。</param>
    /// <param name="route">回调路由形态（含前缀，不含查询串）。</param>
    /// <param name="form">报文形态（XML / JSON / 手工接收器）。</param>
    /// <param name="note">补充说明（如「与公众号同令牌域」）。</param>
    public void Activated(string line, string route, string form, string? note = null)
        => _entries.Add(new LineEntry(line, "active", route, form, note));

    /// <summary>记一条未激活的线（原因 = 缺失的变量名清单，绝不含值）。</summary>
    /// <param name="line">线名。</param>
    /// <param name="route">本该承载它的路由形态（供排障时对照官方后台填写的 URL）。</param>
    /// <param name="bundle">未齐备的凭据束。</param>
    public void Skipped(string line, string route, CredentialBundle bundle)
        => _entries.Add(new LineEntry(line, "inactive", route, bundle.Name + " 凭据不齐", "缺少 " + bundle.MissingText()));

    /// <summary>记一条可选基建（Redis / OpenTelemetry）的取舍。</summary>
    /// <param name="name">基建名。</param>
    /// <param name="active">是否启用。</param>
    /// <param name="reason">取舍原因（开关值或配置摘要，不含凭据）。</param>
    public void Infrastructure(string name, bool active, string reason)
        => _entries.Add(new LineEntry(name, active ? "active" : "inactive", "(无路由)", "基建", reason));

    /// <summary>快照（供 /healthz、/routes 与启动横幅使用）。</summary>
    public IReadOnlyList<object> Snapshot()
        => _entries
            .Select(static e => (object)new
            {
                line = e.Line,
                state = e.State,
                route = e.Route,
                form = e.Form,
                note = e.Note,
            })
            .ToList();

    /// <summary>已激活的回调线条数（不含基建条目，用于横幅给出一眼能读懂的规模）。</summary>
    public int ActiveCallbackLines
        => _entries.Count(static e => e.State == "active" && e.Form != "基建");

    /// <summary>启动横幅文本：每线一行「状态 + 路由 + 原因」，不含任何凭据。</summary>
    public IReadOnlyList<string> Banner()
        => _entries
            .Select(static e => $"  [{e.State}] {e.Line,-14} {e.Route,-28} {e.Form}"
                                + (string.IsNullOrEmpty(e.Note) ? string.Empty : "  ← " + e.Note))
            .ToList();

    private sealed record LineEntry(string Line, string State, string Route, string Form, string? Note);
}

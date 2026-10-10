// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 事件键 → 载荷契约注册表（v2 方案 ADR-3'）。
/// </summary>
/// <remarks>
/// <para>
/// 注册表是映射表的<b>唯一持有者</b>（载荷类型本身不持映射表），故手写链与生成链可逐类切换。
/// </para>
/// <para>
/// <b>生命周期</b>：登记仅发生在<b>组合根期</b>（<c>AddWechatCallback</c> 与其建造者链），
/// 单线程、先于容器构建；读取路径无锁，约定「登记须先于首次读取」。
/// </para>
/// </remarks>
public interface IWechatPayloadContractRegistry
{
    /// <summary>
    /// 登记事件键契约。
    /// </summary>
    /// <param name="contract">载荷契约（含上游映射表与本仓库开放面声明）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="contract"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentException">同一事件键重复登记（fail-fast：组合根期暴露接线错误）。</exception>
    void Register(WechatPayloadContract contract);

    /// <summary>按事件键解析契约。</summary>
    /// <param name="eventTypeKey">事件类型键。</param>
    /// <param name="contract">命中的契约；未命中为 <see langword="null"/>。</param>
    /// <returns>是否命中。</returns>
    bool TryResolve(string eventTypeKey, out WechatPayloadContract? contract);

    /// <summary>已登记的事件键集合（声明序，供守卫与诊断）。</summary>
    IReadOnlyList<string> RegisteredKeys { get; }
}

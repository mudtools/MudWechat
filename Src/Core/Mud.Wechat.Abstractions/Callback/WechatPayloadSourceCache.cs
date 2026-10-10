// -----------------------------------------------------------------------
//  作者：Mud Studio 版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Mud.Wechat.Abstractions.Callback;

/// <summary>
/// 单次解析源缓存：接收器（信封视图）与载荷读取器（节点视图）<b>共享同一份 XML 树</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>消除重复解析</b>：旧解析器每次 <c>ParseXxx</c> 调用都会 <c>XDocument.Parse</c> 一遍
/// （与接收器的解析叠加 ⇒ O(N) 次全量解析）。本缓存使每个事件的<b>解析</b>与<b>节点投影</b>各发生<b>恰好一次</b>。
/// </para>
/// <para>
/// <b>一致性收益</b>：信封字段与载荷字段来自同一棵树，结构上排除「两次解析看到不同内容」的 TOCTOU 风险。
/// </para>
/// <para>
/// <b>弱键 ⇒ 无泄漏</b>：键为事件信封对象本身（<c>ConditionalWeakTable</c>），
/// 事件被回收时条目自动消失，无需手工驱逐，也无需配置清理策略。
/// </para>
/// <para>
/// <b>键为 <see cref="object"/>（产品线中立）</b>：缓存语义与信封的具体类型无关，只需
/// 「同一信封对象 → 同一解析树」。各产品线信封（企微 <c>WechatCallbackEvent</c>、公众号 <c>MpCallbackEnvelope</c>）
/// 直接作为键传入，故本类型可下沉叶层而不引用任何产品线类型。
/// </para>
/// </remarks>
internal sealed class WechatPayloadSourceCache
{
    /// <summary>进程级共享实例：保证接收器与读取器即便经由不同注入路径也命中同一缓存。</summary>
    internal static readonly WechatPayloadSourceCache Shared = new WechatPayloadSourceCache();

    private readonly ConditionalWeakTable<object, XElementPayloadSource.SourceHolder> _sources =
        new ConditionalWeakTable<object, XElementPayloadSource.SourceHolder>();

    /// <summary>取（或首次解析）事件的 XML 源。</summary>
    /// <param name="envelope">回调事件信封对象（弱键，作为「同一请求同一棵树」的身份）。</param>
    /// <param name="decryptedXml">信封携带的解密明文（首次调用时用于解析）。</param>
    /// <returns>源容器；明文非 XML 时容器的根节点为 <c>null</c>。</returns>
    /// <remarks>
    /// 解析与 XML 归属整体委托给 <see cref="XElementPayloadSource"/> —— 本文件只负责「弱键缓存」这一件事，
    /// 不触碰任何 XML 类型，使「XML 触点唯一」的守卫只需放行一个文件。
    /// </remarks>
    internal XElementPayloadSource.SourceHolder GetOrCreate(object envelope, string? decryptedXml)
    {
        if (envelope == null)
            throw new ArgumentNullException(nameof(envelope));

        return _sources.GetValue(
            envelope, _ => new XElementPayloadSource.SourceHolder(XElementPayloadSource.TryParseRoot(decryptedXml)));
    }
}

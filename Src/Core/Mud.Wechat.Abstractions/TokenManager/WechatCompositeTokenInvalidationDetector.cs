// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.TokenManager;

/// <summary>
/// 微信系「令牌失效判定器」组合器：把各产品线的判定器（企微 / 公众号 / …）聚合为**单一**
/// 判定器实例，供组件 <c>TokenRecoveryOptions.TokenInvalidationDetector</c> 单槽属性消费。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何必须有组合器（架构约束）</b>：组件 <c>TokenRecoveryOptions.TokenInvalidationDetector</c>
/// 是<b>单槽引用属性</b>。若每条产品线各自 <c>PostConfigure</c> 写入，则<b>后注册者覆盖前者</b>——
/// 同一宿主同时引用两个 SDK 时，必有一方的 errcode 令牌恢复<b>静默失效</b>。组合器把「谁登记」收敛为
/// 「谁提供子判定器」，登记动作唯一（见
/// <see cref="WechatTokenRecoveryRegistration.AddWechatTokenRecovery"/>）。
/// </para>
/// <para>
/// <b>判定语义</b>：<see cref="ShouldInspect"/> 任一子判定器放行即放行（短路）；
/// <see cref="IsTokenInvalidAsync"/> 任一子判定器判真即判真。语义安全——各子判定器均按自身
/// 产品线域名 / 失效码集合判定，跨产品线的「放行」只会让另一族判定器多读一次响应体，
/// 不会产生误判（响应体非合法 JSON 或缺省 errcode 一律为「未失效」）。
/// </para>
/// <para>
/// <b>零反射、AOT 安全</b>：仅列表遍历，无反射、无动态代码。
/// </para>
/// </remarks>
public sealed class WechatCompositeTokenInvalidationDetector : ITokenInvalidationDetector
{
    private readonly IReadOnlyList<ITokenInvalidationDetector> _detectors;

    /// <summary>创建组合判定器。</summary>
    /// <param name="detectors">子判定器集合（顺序即判定顺序）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="detectors"/> 为 null。</exception>
    public WechatCompositeTokenInvalidationDetector(IEnumerable<ITokenInvalidationDetector> detectors)
    {
        if (detectors == null)
        {
            throw new ArgumentNullException(nameof(detectors));
        }

        _detectors = detectors as IReadOnlyList<ITokenInvalidationDetector> ?? detectors.ToArray();
    }

    /// <summary>子判定器数量（诊断 / 测试用）。</summary>
    public int Count => _detectors.Count;

    /// <inheritdoc />
    /// <remarks>任一子判定器放行即放行（同步短路，无响应体读取开销）。</remarks>
    public bool ShouldInspect(HttpRequestMessage request)
    {
        for (var i = 0; i < _detectors.Count; i++)
        {
            if (_detectors[i].ShouldInspect(request))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    /// <remarks>
    /// 逐个询问子判定器，首个判真即返回 <c>true</c>（短路）。子判定器自身的异常降级由组件恢复链路
    /// 统一兜底（除取消外一律按「未失效」处理并记 Warning），本层不额外捕获。
    /// </remarks>
    public async ValueTask<bool> IsTokenInvalidAsync(
        HttpResponseMessage response,
        ReadOnlyMemory<byte>? body,
        CancellationToken cancellationToken)
    {
        for (var i = 0; i < _detectors.Count; i++)
        {
            if (await _detectors[i].IsTokenInvalidAsync(response, body, cancellationToken).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }
}

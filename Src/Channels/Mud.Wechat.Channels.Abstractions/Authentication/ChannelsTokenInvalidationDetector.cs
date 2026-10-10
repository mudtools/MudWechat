// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Channels.Abstractions.Enums;

namespace Mud.Wechat.Channels.Abstractions.Authentication;

/// <summary>
/// 微信小店 / 视频号 errcode 令牌失效判定器（组件 <c>ITokenInvalidationDetector</c> 两阶段实现）。
/// </summary>
/// <remarks>
/// <para>
/// 业务响应 HTTP 200 + <c>errcode ∈ {40001, 40014, 42001}</c> 判真，触发恢复链路：
/// 失效缓存 → 强制刷新 → 重建请求重注入 → 重试。
/// </para>
/// <para>
/// <b>为何 <c>40001</c> 必须在内</b>：官方语义为「AppSecret 错误<b>或 access_token 无效 / 非最新</b>」。
/// 业务端点的 40001 表达「本地持有的 token 已被其它进程刷新为非最新」——正是恢复链路要解决的场景。
/// 签发接口（不带 <c>[Token]</c>）的 40001 不进恢复链路，由 <see cref="Exceptions.WechatChannelsException"/> 抛出。
/// </para>
/// <para>
/// <b>登记方式</b>：以「子判定器」身份注册为 DI 集合项，由公用层
/// <see cref="WechatTokenRecoveryRegistration.AddWechatTokenRecovery"/> 组装为组合判定器写入
/// <c>TokenRecoveryOptions.TokenInvalidationDetector</c>（该选项为单槽属性，多产品线各自 PostConfigure
/// 会互相覆盖 ⇒ 子判定器登记是唯一正确形态）。
/// </para>
/// </remarks>
internal sealed class ChannelsTokenInvalidationDetector : ITokenInvalidationDetector
{
    private static readonly long[] InvalidErrcodes =
    {
        ChannelsErrorCodes.InvalidCredential,      // 40001（access_token 无效 / 非最新）
        ChannelsErrorCodes.InvalidAccessToken,     // 40014
        ChannelsErrorCodes.ExpiredAccessToken,     // 42001
    };

    /// <summary>
    /// 同步预过滤：仅小店域名（或显式登记的自定义 BaseUrl 主机）请求参与判定，无关请求零捕获开销。
    /// </summary>
    /// <param name="request">请求报文。</param>
    /// <returns>需要参与判定返回 <c>true</c>。</returns>
    /// <remarks>
    /// 本方法为<b>同步</b>预过滤，只有请求对象、无法按应用读取 <c>AllowCustomBaseUrl</c>，
    /// 故放行条件为「官方域白名单 ∪ 注册期显式登记的自定义主机」（见
    /// <see cref="WechatCustomBaseUrlRegistry"/>）。放行只影响「是否参与判定」，判定本身仍要求响应体为
    /// 合法 JSON 且 errcode 命中集合，不会造成误判。
    /// </remarks>
    public bool ShouldInspect(HttpRequestMessage request)
    {
        var host = request.RequestUri?.Host;
        if (string.IsNullOrEmpty(host))
        {
            return false;
        }

        foreach (var domain in WechatApiHosts.AllowedBaseUrlDomains)
        {
            if (string.Equals(host, domain, StringComparison.OrdinalIgnoreCase)
                || host!.EndsWith("." + domain, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return WechatCustomBaseUrlRegistry.IsRegistered(host);
    }

    /// <summary>
    /// 判断响应是否表示令牌失效：解析 JSON 响应体的 errcode 并比对失效码集合
    /// （body 为空——超限 / chunked / 未声明 Content-Length——时判定退化为「未失效」）。
    /// </summary>
    /// <param name="response">原始响应。</param>
    /// <param name="body">已捕获的响应体字节（可能为 null）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>令牌失效返回 <c>true</c>。</returns>
    public ValueTask<bool> IsTokenInvalidAsync(
        HttpResponseMessage response,
        ReadOnlyMemory<byte>? body,
        CancellationToken cancellationToken)
    {
        if (body is not { Length: > 0 } captured)
        {
            return new ValueTask<bool>(false);
        }

        try
        {
            using var doc = JsonDocument.Parse(captured);
            var errcode = doc.RootElement.TryGetProperty("errcode", out var e) && e.TryGetInt64(out var code)
                ? code
                : -1;
            return new ValueTask<bool>(Array.IndexOf(InvalidErrcodes, errcode) >= 0);
        }
        catch (JsonException)
        {
            // 非小店 JSON 响应体（如网关错误页）不参与判定（判定器链路异常由组件侧统一降级）。
            return new ValueTask<bool>(false);
        }
    }
}
// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.TokenManagers;

/// <summary>
/// 企业微信 errcode 令牌失效判定器（Mud.HttpUtils v2.0.9 <see cref="ITokenInvalidationDetector"/> 两阶段实现）。
/// </summary>
/// <remarks>
/// <para>
/// 业务响应 HTTP 200 + <c>errcode ∈ {40014, 42001, 42007, 42009, 42011}</c> 判真，
/// 触发恢复链路：失效缓存 → 强制刷新 → 重建请求重注入 → 重试。
/// </para>
/// <para>
/// 注入方式：经 <c>TokenRecoveryOptions.TokenInvalidationDetector</c> 属性编程式注入
/// （PostConfigure 保证 IOptionsMonitor 每次快照都携带判定器），<b>不是</b> DI 集合注入。
/// </para>
/// </remarks>
public sealed class WechatTokenInvalidationDetector : ITokenInvalidationDetector
{
    private static readonly long[] InvalidErrcodes =
    {
        WechatErrorCodes.InvalidAccessToken,          // 40014
        WechatErrorCodes.ExpiredAccessToken,          // 42001
        WechatErrorCodes.RelatedAccessTokenInvalid,   // 42007
        WechatErrorCodes.InvalidSuiteAccessToken,     // 42009
        WechatErrorCodes.InvalidProviderAccessToken,  // 42011
    };

    /// <summary>
    /// 同步预过滤：仅企业微信域名请求参与判定，无关请求零捕获开销。
    /// </summary>
    public bool ShouldInspect(HttpRequestMessage request)
        => request.RequestUri?.Host.EndsWith("weixin.qq.com", StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>
    /// 判断响应是否表示令牌失效：解析 JSON 响应体的 errcode 并比对失效码集合
    /// （body 为空——超限 / chunked / 未声明 Content-Length——时回退状态码判定，即返回 false）。
    /// </summary>
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
            // 非企业微信 JSON 响应体（如网关错误页）不参与判定（判定器链路异常由组件侧统一降级）。
            return new ValueTask<bool>(false);
        }
    }
}

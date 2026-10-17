// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Mud.Wechat.Demo.Shared;

/// <summary>
/// Demo 统一健康检查端点：<c>GET /healthz</c> 用于在无凭据环境下验证「装配正确 + 配置已读到」，
/// 响应<b>只含配置摘要</b>，不含任何密钥、令牌或解密明文。
/// </summary>
public static class DemoHealthEndpoint
{
    /// <summary>映射 <c>GET /healthz</c>。</summary>
    /// <param name="endpoints">端点路由建造器（<c>WebApplication</c> 本身即可）。</param>
    /// <param name="demoName">Demo 标识，用于在混合宿主里区分来源。</param>
    /// <param name="configuration">
    /// 配置摘要对象。调用方负责只放入非敏感字段（应用键、应用类型、路由前缀、是否已配置回调凭据等）。
    /// </param>
    public static IEndpointRouteBuilder MapDemoHealth(
        this IEndpointRouteBuilder endpoints,
        string demoName,
        object configuration)
    {
        endpoints.MapGet("/healthz", () => Results.Json(new
        {
            status = "ok",
            demo = demoName,
            utcNow = DateTimeOffset.UtcNow,
            writeGate = DemoWriteGate.Enabled ? "enabled" : "disabled",
            config = configuration,
        }));

        return endpoints;
    }
}

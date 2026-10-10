// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mud.Wechat.OfficialAccount.Web;

namespace Mud.Wechat.OfficialAccount.Extensions;

/// <summary>
/// JS-SDK 签名服务注册入口（P9）。
/// </summary>
/// <remarks>
/// <b>依赖前置</b>：票据管理器 <c>IMpJsApiTicketManager</c> 由多应用基座（<c>AddMpApp</c>）注册 ⇒
/// 本扩展须在其之后调用（与回调/令牌链路的装配顺序约束一致）。
/// </remarks>
public static class MpJsApiSignatureExtensions
{
    /// <summary>注册 JS-SDK 签名服务（<c>IMpJsApiSignatureService</c>）。</summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddMpJsApiSignature(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddTransient<IMpJsApiSignatureService, MpJsApiSignatureService>();
        return services;
    }
}

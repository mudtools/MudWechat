// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Exceptions;

/// <summary>
/// 令牌归属域与应用类型不匹配异常：把「第三方/代开发契约入口注入到自建应用」这类错配从静默转为 fail-fast。
/// </summary>
/// <remarks>
/// <para>
/// <b>派生自 <see cref="InvalidOperationException"/></b>（与 <c>WechatCallbackException</c> 同惯例）：
/// 宿主既有的 <c>catch (InvalidOperationException)</c> 语义不变，同时可精确捕获本类型做配置诊断。
/// </para>
/// <para>
/// 触发点唯一：<c>WechatAppContext.GetTokenManager(string)</c> 的归属域校验
/// （由 <see cref="WechatTokenManagerKeys"/> 声明的键驱动）。这是<b>编程错误</b>而非运行时故障，
/// 不重试、不降级。
/// </para>
/// </remarks>
public sealed class WechatTokenOwnerMismatchException : InvalidOperationException
{
    /// <summary>接口声明的令牌查找键（携带归属域后缀，可在源码中直接 grep）。</summary>
    public string TokenManagerKey { get; }

    /// <summary>当前应用标识。</summary>
    public string AppKey { get; }

    /// <summary>当前应用类型（与接口声明的归属域不匹配的一方）。</summary>
    public WechatAppType ActualAppType { get; }

    /// <summary>创建归属域不匹配异常。</summary>
    /// <param name="message">错误消息（含键、期望应用类型、当前 appKey/AppType 与修复动作）。</param>
    /// <param name="tokenManagerKey">接口声明的令牌查找键。</param>
    /// <param name="appKey">当前应用标识。</param>
    /// <param name="actualAppType">当前应用类型。</param>
    public WechatTokenOwnerMismatchException(
        string message, string tokenManagerKey, string appKey, WechatAppType actualAppType)
        : base(message)
    {
        TokenManagerKey = tokenManagerKey;
        AppKey = appKey;
        ActualAppType = actualAppType;
    }
}

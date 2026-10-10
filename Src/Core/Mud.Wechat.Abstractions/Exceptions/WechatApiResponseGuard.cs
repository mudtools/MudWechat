// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Exceptions;

/// <summary>
/// 微信系 API 响应判错出口：把「null / 业务错误码非 0」两种失败形态收敛到一处，
/// 由各产品线以自身异常类型抛出（保持 <c>catch</c> 面的类型身份不变）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何不把 <c>ThrowIfFailed</c> 直接放在 <see cref="WechatApiException"/> 上</b>：静态成员无法是
/// <c>abstract</c>，若基类实现它就只能抛出基类类型，各产品线的具体异常类型（
/// <c>WechatWorkException</c> / <c>MpException</c>）将失去类型身份，破坏既有 <c>catch</c> 语义。
/// 故本守卫接受「异常工厂」委托，由各产品线以 <b>静态缓存</b> 的委托传入（零每调用分配）。
/// </para>
/// <para>
/// <b>错误码缺省语义</b>：响应的 <see cref="IWechatApiResponse.ErrorCode"/> 缺省为 0 ⇒ 视为成功。
/// 这天然覆盖「成功响应体不含错误码」的官方形态（公众号 <c>token</c> / <c>stable_token</c>）。
/// </para>
/// </remarks>
public static class WechatApiResponseGuard
{
    /// <summary>
    /// 响应为 <c>null</c> 或 <see cref="IWechatApiResponse.IsSuccess"/> 为 <c>false</c> 时抛出
    /// 由 <paramref name="exceptionFactory"/> 构造的产品线异常。
    /// </summary>
    /// <typeparam name="TResponse">响应类型（须实现判错契约）。</typeparam>
    /// <param name="response">响应实例（可为 null）。</param>
    /// <param name="exceptionFactory">产品线异常工厂：<c>(错误码, 消息, 请求地址) =&gt; 异常</c>。</param>
    /// <param name="requestUri">请求地址（可选，诊断用；由异常构造期脱敏）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> 为 null。</exception>
    public static void ThrowIfFailed<TResponse>(
        TResponse? response,
        Func<int, string, string?, WechatApiException> exceptionFactory,
        string? requestUri = null)
        where TResponse : class, IWechatApiResponse
    {
        if (exceptionFactory == null)
        {
            throw new ArgumentNullException(nameof(exceptionFactory));
        }

        if (response == null)
        {
            throw exceptionFactory(-1, "微信 API 返回结果为 null。", requestUri);
        }

        if (!response.IsSuccess)
        {
            throw exceptionFactory(
                response.ErrorCode,
                $"微信 API 调用失败：errcode={response.ErrorCode}, errmsg={response.ErrorMessage}。",
                requestUri);
        }
    }
}

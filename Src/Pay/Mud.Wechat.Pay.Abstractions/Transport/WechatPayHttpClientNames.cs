// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Abstractions.Transport;

/// <summary>支付线命名 HttpClient 的名称与类型名（供 <c>[HttpClientApi]</c> 引用）。</summary>
/// <remarks>
/// <para>
/// <b>为什么支付线要自占一个类型名，而不是复用 <c>IEnhancedHttpClient</c></b>：
/// <c>[HttpClientApi(HttpClient = ...)]</c> 注入的客户端是<b>按类型</b>从 DI 取的（无键控注册），
/// 而 <c>IEnhancedHttpClient</c> 的<b>默认</b>实例已被企微线的
/// <c>IWechatWorkProviderAuthenticationUrl</c> 占用。若支付线也走该默认实例：
/// ① 命名客户端的 <c>BaseUrl</c> 会互相冲突（支付是 <c>api.mch.weixin.qq.com</c>、企微是 <c>qyapi.weixin.qq.com</c>）；
/// ② 支付签名 Handler 会给企微请求也套上 <c>Authorization</c> 头。
/// 故支付线独立占位，互不干扰。
/// </para>
/// <para>
/// <b>类型名必须写全限定名</b>：生成器 <c>ValidateHttpClientType</c> 对自定义类型按 metadata name
/// 精确查找（仅 <c>IEnhancedHttpClient</c>/<c>IBaseHttpClient</c> 有短名特判），
/// 用 <c>nameof(...)</c> 只会产出短名 ⇒ HTTPCLIENT014。故此处提供常量而非让调用方拼写。
/// </para>
/// </remarks>
public static class WechatPayHttpClientNames
{
    /// <summary>命名 HttpClient 的名称（与 <c>AddMudHttpClient</c> 注册名一致）。</summary>
    public const string ClientName = "wechat-pay";

    /// <summary>
    /// 支付请求主域名（官方主域名，<b>已在 SSRF 白名单内</b>，见方案 §5.2 / 守卫 PAY-B8）。
    /// </summary>
    /// <remarks>
    /// 备域名 <c>https://api2.mch.weixin.qq.com</c> 同在白名单内；个别端点（如账单下载）
    /// 会返回备域名地址，由端点层按官方返回值处理，不在此处切换 BaseAddress。
    /// </remarks>
    public const string BaseAddress = "https://api.mch.weixin.qq.com";

    /// <summary>
    /// 支付客户端类型的<b>全限定</b> metadata name，供 <c>[HttpClientApi(HttpClient = ...)]</c> 使用。
    /// </summary>
    public const string TypeName = "Mud.Wechat.Pay.Abstractions.Transport.IWechatPayHttpClient";
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions.Transport;

/// <summary>
/// 开放平台线命名 HttpClient 的名称与类型名。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么本线要自占一个类型名，而不是复用 <c>IEnhancedHttpClient</c></b>：与支付线同款的理由 ——
/// <c>IEnhancedHttpClient</c> 的<b>默认</b>实例已被企微线占用，共用会让
/// ① 命名客户端的 <c>BaseUrl</c> 互相冲突（开放平台是 <c>api.weixin.qq.com</c>、
/// 企微是 <c>qyapi.weixin.qq.com</c>、支付是 <c>api.mch.weixin.qq.com</c>）；
/// ② 彼此注册的拦截器互相串扰（例如支付线的签名 Handler 会给开放平台请求套上
/// <c>Authorization</c> 头，而开放平台的 component 调用<b>没有</b> APIv3 签名语义）。
/// </para>
/// <para>
/// <b>与支付线的关键差异</b>：支付线在命名客户端上挂 <c>WechatPayAuthorizationHandler</c>（APIv3 报文签名）；
/// 本线<b>不挂任何签名 Handler</b> —— 开放平台的 component 凭证是
/// <c>component_appid</c> / <c>component_appsecret</c> 这类<b>请求参数</b>，
/// 由官方自行校验，不存在 APIv3 那套 <c>Authorization</c> 报文签名。
/// </para>
/// <para>
/// <b>类型名必须写全限定名</b>：生成器对自定义类型按 metadata name 精确查找
/// （仅 <c>IEnhancedHttpClient</c>/<c>IBaseHttpClient</c> 有短名特判），
/// 用 <c>nameof(...)</c> 只会产出短名 ⇒ HTTPCLIENT014。故此处提供常量而非让调用方拼写。
/// </para>
/// </remarks>
public static class OpenPlatformHttpClientNames
{
    /// <summary>命名 HttpClient 的名称（与 <c>AddMudHttpClient</c> 注册名一致）。</summary>
    public const string ClientName = "wechat-openplatform";

    /// <summary>
    /// 开放平台 API 主域名（官方主域名，已在 SSRF 白名单内 —— 域名后缀命中
    /// <c>weixin.qq.com</c>，故本线对进程级白名单<b>零改动</b>）。
    /// </summary>
    public const string BaseAddress = OpenPlatformContract.ApiBaseUrl;

    /// <summary>本线 HTTP 客户端契约的<b>全限定</b> metadata name（供 <c>[HttpClientApi(HttpClient = ...)]</c> 使用）。</summary>
    public const string TypeName = "Mud.Wechat.OpenPlatform.Abstractions.Transport.IWechatOpenPlatformHttpClient";
}

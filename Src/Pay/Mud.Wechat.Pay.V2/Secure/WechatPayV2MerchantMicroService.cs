// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2MerchantMicroService"/> 的实现：逐商户证书通道 + 手工签名收发。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写</b>：两页官方「是否需要证书」为「是，详见证书使用」⇒ 证书按商户绑定。
/// </para>
/// <para>
/// <b>域名是 <c>api.mch</c></b>：两页「接口说明」栏给 <c>api.mch.weixin.qq.com/applyment/micro/*</c> ⇒
/// 取 <see cref="WechatPayV2HttpClientNames.BaseAddress"/>。
/// </para>
/// <para>
/// <b>算法不在本类分支</b>：官方「仅支持 HMAC-SHA256」由 DTO 的 <c>sign_type</c> 默认值承载，
/// <see cref="WechatPayV2Signer"/> 见字段表即选 HMAC；本类不提供「降级到 MD5」的口子。
/// </para>
/// <para>
/// <b>不出密文</b>（PAY-B7）：<c>account_number</c>／<c>mobile_phone</c>／<c>email</c> 属个人金融信息，
/// 无论明文密文都不进日志与异常消息；<see cref="WechatPayV2SecureInvoker"/> 的异常文本只含商户键、
/// 路由与官方字段名，本类不额外拼接任何请求内容。
/// </para>
/// </remarks>
internal sealed class WechatPayV2MerchantMicroService : IWechatPayV2MerchantMicroService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建小微商户资料服务（由 DI 解析）。</summary>
    public WechatPayV2MerchantMicroService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2MicroModifyResponse> ModifyArchivesAsync(
        V2MicroArchivesModifyRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2MicroArchivesModifyRequest, V2MicroModifyResponse>(
            "/applyment/micro/modifyarchives",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2MicroModifyResponse> ModifyContactInfoAsync(
        V2MicroContactInfoModifyRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2MicroContactInfoModifyRequest, V2MicroModifyResponse>(
            "/applyment/micro/modifycontactinfo",
            WechatPayV2HttpClientNames.BaseAddress, request, cancellationToken);
}

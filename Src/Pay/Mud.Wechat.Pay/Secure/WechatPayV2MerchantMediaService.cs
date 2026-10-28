// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2MerchantMediaService"/> 的实现：证书通道上的 <c>multipart/form-data</c> 出口。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写</b>：官方「是否需要证书」为「是」，且请求载体是表单而非 XML ——
/// 声明式管道既拿不到逐商户证书，也没有把文件分节接进生成式客户端的口子。
/// 收发仍走 <see cref="WechatPayV2SecureInvoker.PostMultipartAsync{TRequest,TResponse}"/>，
/// 签名字段表与 XML 出口同源 ⇒ 只是把「XML 体」换成「表单分节」，验签口径不变。
/// </para>
/// <para>
/// <b>文件分节不参与签名</b>：官方字段表里 <c>media</c> 是文件标识而非签名项，
/// 收发器只对 DTO 字段 + <c>sign</c> 建表 ⇒ 图片字节天然落在签名串之外，
/// 完整性由 <c>media_hash</c>（<b>小写</b> MD5）承载。
/// </para>
/// <para>
/// <b>域名是 <c>apihk.mch</c></b>：本页「接口说明」栏只印 <c>apihk</c> ⇒
/// 取 <see cref="WechatPayV2HttpClientNames.GlobalBaseAddress"/>。
/// </para>
/// </remarks>
internal sealed class WechatPayV2MerchantMediaService : IWechatPayV2MerchantMediaService
{
    /// <summary>官方文件分节字段名（<c>form-data</c> 中的 name 属性，照抄不得改）。</summary>
    internal const string MediaFieldName = "media";

    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建商户媒体服务（由 DI 解析）。</summary>
    public WechatPayV2MerchantMediaService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2UploadMediaResponse> UploadMediaAsync(
        V2UploadMediaRequest request,
        Stream media,
        string fileName,
        CancellationToken cancellationToken = default)
        => _invoker.PostMultipartAsync<V2UploadMediaRequest, V2UploadMediaResponse>(
            "/secapi/mch/uploadmedia",
            WechatPayV2HttpClientNames.GlobalBaseAddress, request,
            MediaFieldName, media, fileName, cancellationToken);
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// <see cref="IWechatPayV2InstitutionSubService"/> 的实现：逐商户证书通道 + 手工签名收发，域名钉在 <c>apihk.mch</c>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么手写</b>：三页官方「是否需要证书」均为「是」⇒ 证书按商户绑定，声明式管道拿不到。
/// </para>
/// <para>
/// <b>域名是本线的第三个常量</b>：三页「接口说明」栏<b>只印</b> <c>apihk.mch.weixin.qq.com</c>，
/// 全文不出现 <c>api.mch</c> ⇒ 取 <see cref="WechatPayV2HttpClientNames.GlobalBaseAddress"/>。
/// 这是「要证书」与「挂在证书域名上」两个独立事实的第三处落点：本族既不在 <c>api.mch</c> 也不在 <c>api2.mch</c>。
/// </para>
/// <para>
/// <b>算法只有 MD5</b>：官方原文「默认加密方式为MD5，且仅支持MD5加密」⇒ 本族 DTO 不带 <c>sign_type</c>，
/// <see cref="WechatPayV2Signer"/> 据字段表自动落 MD5；不得因「别的族用 HMAC」而在此补齐 <c>sign_type</c>。
/// </para>
/// <para>
/// <b>个人证件与联系方式不出门</b>（PAY-B7）：法人／负责人证件号、注册证件编号、联系人手机邮箱、
/// 结算银行账户按个人金融信息处理，不进日志、不进异常消息；收发器的异常文本只含商户键、路由与官方字段名。
/// </para>
/// </remarks>
internal sealed class WechatPayV2InstitutionSubService : IWechatPayV2InstitutionSubService
{
    private readonly WechatPayV2SecureInvoker _invoker;

    /// <summary>创建机构子商户服务（由 DI 解析）。</summary>
    public WechatPayV2InstitutionSubService(WechatPayV2SecureInvoker invoker)
        => _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));

    /// <inheritdoc />
    public Task<V2InstitutionSubAddResponse> AddAsync(
        V2InstitutionSubAddRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2InstitutionSubAddRequest, V2InstitutionSubAddResponse>(
            "/secapi/mch/addInstitutionsub",
            WechatPayV2HttpClientNames.GlobalBaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2InstitutionSubResponse> ModifyAsync(
        V2InstitutionSubModifyRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2InstitutionSubModifyRequest, V2InstitutionSubResponse>(
            "/secapi/mch/modifyInstitutionsub",
            WechatPayV2HttpClientNames.GlobalBaseAddress, request, cancellationToken);

    /// <inheritdoc />
    public Task<V2InstitutionSubQueryResponse> QueryAsync(
        V2InstitutionSubQueryRequest request, CancellationToken cancellationToken = default)
        => _invoker.PostAsync<V2InstitutionSubQueryRequest, V2InstitutionSubQueryResponse>(
            "/secapi/mch/queryInstitutionsub",
            WechatPayV2HttpClientNames.GlobalBaseAddress, request, cancellationToken);
}

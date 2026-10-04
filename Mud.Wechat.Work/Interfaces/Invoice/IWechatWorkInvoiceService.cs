// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Invoice;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「电子发票」域公共 SDK（查询电子发票 / 更新发票状态 / 批量更新发票状态 / 批量查询电子发票，
/// 三类应用均可调用的端点收敛面）。
/// <para>
/// 落位与形态对齐 <see cref="IWechatWorkMediaService"/>：三类应用的端点集完全一致，全部收敛于本接口；
/// 应用类型子接口均为零差异端点空标记：自建应用见 <see cref="IWechatWorkInternalInvoiceService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyInvoiceService"/>，服务商代开发见 <see cref="IWechatWorkProviderInvoiceService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 官方权限口径：报销更新类接口仅认证的企业微信账号有接口权限；批量查询电子发票须企业激活人数超过 200。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInvoiceService
{
    /// <summary>
    /// 查询电子发票
    /// <para>报销方在获得用户选择的电子发票标识参数（card_id + encrypt_code）后，
    /// 查询电子发票的结构化信息，并可通过返回的 pdf_url 获取发票 PDF 文件。</para>
    /// <para>官方权限：仅认证的企业微信账号有接口权限。</para>
    /// </summary>
    /// <param name="request">发票标识请求体（<see cref="GetInvoiceInfoRequest"/>：card_id / encrypt_code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发票结构化信息（有效期 / openid / 类型 / 收款方 / <see cref="InvoiceUserInfo"/>，含报销状态与 PDF 地址）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90284"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90420"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99451"/></para>
    /// </remarks>
    [Post("/cgi-bin/card/invoice/reimburse/getinvoiceinfo")]
    Task<GetInvoiceInfoResponse> GetInvoiceInfoAsync(
        [Body] GetInvoiceInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新发票状态
    /// <para>对单张电子发票执行锁定、解锁和报销操作：锁定后发票仍保留在用户卡包内但无法重复提交报销；
    /// 解锁后发票恢复可提交状态；报销（核销）完成后发票将从用户卡包中移除。</para>
    /// <para>官方权限：仅认证的企业微信账号有接口权限。
    /// 官方约束：报销（INVOICE_REIMBURSE_CLOSURE）为<b>不可逆操作</b>，请开发者慎重调用。</para>
    /// </summary>
    /// <param name="request">发票状态请求体（<see cref="UpdateInvoiceStatusRequest"/>：card_id / encrypt_code / reimburse_status）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90285"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90421"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99452"/></para>
    /// </remarks>
    [Post("/cgi-bin/card/invoice/reimburse/updateinvoicestatus")]
    Task<WechatWorkResponse> UpdateInvoiceStatusAsync(
        [Body] UpdateInvoiceStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新发票状态
    /// <para>对同一用户（openid）名下的多张电子发票批量执行锁定、解锁和报销操作；
    /// 发票列表必须全部属于同一个 openid（openid 可通过 userid 与 openid 互换接口获取）。</para>
    /// <para>官方权限：仅认证的企业微信账号有接口权限。
    /// 官方约束：本接口为<b>事务性操作</b>——如果其中一张发票更新失败，
    /// 列表中的其它发票状态更新也会无法执行，恢复到接口调用前的状态；
    /// 报销为不可逆操作，请开发者慎重调用。</para>
    /// </summary>
    /// <param name="request">批量状态请求体（<see cref="UpdateInvoiceStatusBatchRequest"/>：openid / reimburse_status / invoice_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>更新结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90286"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90422"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99453"/></para>
    /// </remarks>
    [Post("/cgi-bin/card/invoice/reimburse/updatestatusbatch")]
    Task<WechatWorkResponse> UpdateInvoiceStatusBatchAsync(
        [Body] UpdateInvoiceStatusBatchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量查询电子发票
    /// <para>报销方在获得用户选择的电子发票标识参数后，批量查询电子发票的结构化信息。</para>
    /// <para>官方权限：仅认证的企业微信账号<b>并且企业激活人数超过 200</b> 才有接口权限。</para>
    /// </summary>
    /// <param name="request">批量查询请求体（<see cref="GetInvoiceInfoBatchRequest"/>：item_list，每项含 card_id / encrypt_code）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发票结构化信息列表（<see cref="InvoiceInfoItem"/>，与请求顺序对应）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90287"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90423"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99454"/></para>
    /// </remarks>
    [Post("/cgi-bin/card/invoice/reimburse/getinvoiceinfobatch")]
    Task<GetInvoiceInfoBatchResponse> GetInvoiceInfoBatchAsync(
        [Body] GetInvoiceInfoBatchRequest request,
        CancellationToken cancellationToken = default);
}

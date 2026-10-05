// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.PayTool;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「收银台」模块发票管理域第三方应用 SDK（获取发票列表 / 标记开票状态）。
/// <para>官方仅在第三方应用开发文档树开放本族端点，全部 2 个端点声明于本接口；
/// 企业自建应用与服务商代开发官方不开放，不设对应子接口。</para>
/// </summary>
/// <remarks>
/// <para>消费服务商级 provider_access_token（路由键 <see cref="WechatTokenTypes.ProviderAccessToken"/>）。</para>
/// <para>官方权限口径：服务商需有在收银台完成商户号注册；「标记开票状态」的操作人还需有
/// 「收银台-发票管理」的权限。</para>
/// <para>官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制。</para>
/// <para>MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（provider_access_token），无法改用 Header。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "PayTool",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkPayToolInvoiceService))]
[Token(TokenType = WechatTokenTypes.ProviderAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "provider_access_token")]
public interface IWechatWorkThirdPartyPayToolInvoiceService : IWechatWorkPayToolInvoiceService
{
    /// <summary>
    /// 获取发票列表
    /// <para>服务商可以使用该接口查询客户企业已提交的开票申请。</para>
    /// <para>官方权限：服务商需有在收银台完成商户号注册。</para>
    /// <para>官方约束：start_time 与 end_time <b>不能单独指定，必须同时指定</b>；
    /// limit 为可选整型，最大值 100、默认值 50；cursor 由上一次调用返回，首次调用可不填。</para>
    /// <para>官方契约陷阱：本端点参数表<b>不含</b> nonce_str / ts / sig（与收款工具族不同），调用时不要附加签名三要素。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetPayToolInvoiceListRequest"/>：start_time 开始时间 /
    /// end_time 结束时间 / cursor 分页游标 / limit 返回的最大记录数）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否有更多（has_more）/ 分页游标（next_cursor）/ 发票列表（invoice_list）。
    /// <para>列表项含 order_id / custom_corpid / apply_time / invoice_type / paid_price / invoice_status /
    /// invoice_title / tax_number / send_way / contact_name / contact_tel / contact_addr / contact_postcode /
    /// receive_email / company_addr / company_tel / bank_name / bank_account_number / invoice_note。</para></returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99436">path 99436 获取发票列表</see></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/paytool/get_invoice_list")]
    Task<GetPayToolInvoiceListResponse> GetInvoiceListAsync(
        [Body] GetPayToolInvoiceListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 标记开票状态
    /// <para>服务商可以使用该接口标记某个应用订单发票的开票状态。</para>
    /// <para>官方权限：服务商需有在收银台完成商户号注册。</para>
    /// <para>官方约束：操作人需要有「收银台-发票管理」的权限；invoice_status 取
    /// <c>1</c>-已开具纸质发票并邮寄给客户 / <c>2</c>-已开具电子发票并发送至客户邮箱 /
    /// <c>3</c>-取消开具发票（取消后企业可再次申请）；
    /// <b>若订单对应开票状态为已开票，此次标记将不生效</b>。</para>
    /// <para>官方约束：invoice_note 为官方必填，不超过 200 字节，客户侧可见。</para>
    /// <para>官方契约陷阱：本端点参数表不含 nonce_str / ts / sig（与收款工具族不同）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="MarkPayToolInvoiceStatusRequest"/>：order_id 订单号 /
    /// oper_userid 操作人 userid / invoice_status 开票状态 / invoice_note 开票备注）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 errcode / errmsg（官方本端点无业务负载）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99437">path 99437 标记开票状态</see></para>
    /// <para>官方页面未给出本端点的独立频率限制，走官方全局访问频率限制。</para>
    /// </remarks>
    [Post("/cgi-bin/paytool/mark_invoice_status")]
    Task<WechatWorkResponse> MarkInvoiceStatusAsync(
        [Body] MarkPayToolInvoiceStatusRequest request,
        CancellationToken cancellationToken = default);
}
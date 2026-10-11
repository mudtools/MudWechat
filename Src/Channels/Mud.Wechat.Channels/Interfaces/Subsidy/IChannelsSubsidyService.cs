// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Subsidy;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「国补管理-发票」域 SDK（3 端点：开票信息查询 /
/// 发票信息上传 / 发票文件上传，路由前缀 <c>/channels/ec/subsidy/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方调用限制（三端点一致）</b>：①<b>不支持云调用、不支持第三方平台代调用</b>（官方文档原文，
/// 故本域无权限集 id）；②订单必须是<b>使用了国补的订单 id</b>；③只能在订单<b>确认收货之后</b>调用；
/// ④<b>同一笔订单最多可以上传 5 次</b>发票信息。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Subsidy", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsSubsidyService
{
    /// <summary>
    /// 获取国补订单开票信息（<c>query_invoicing_info</c>，官方路由 <c>/channels/ec/subsidy/query_invoicing_info</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id</c> 必填，必须是使用了国补的订单 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>开票信息（<c>data</c>：发票抬头 / 税号 / 金额组 / 69 码 / sn 码 / imei / 商品型号品牌名称等）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/subsidy/invoicing/api_query_invoicing_info"/></para>
    /// <para>
    /// 官方业务限制：①订单必须是使用了国补的订单 id，且只能在订单确认收货之后调用；
    /// ②返回的 <c>goods_name</c>/<c>goods_model</c>/<c>brand_name</c> 从广东银联接口获得，
    /// 仅作为开票信息「规格型号」的参考。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：官方示例 JSON 将开票总金额写作 <c>invoice_tatal_amount</c>
    /// （官方拼写瑕疵）——按参数表字段名 <c>invoice_total_amount</c> 建模。
    /// </para>
    /// <para>官方错误码：<c>987100102</c>（订单非国补订单）/ <c>987100116</c>（订单查找失败）/ <c>987100126</c>（订单未确认收货）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/subsidy/query_invoicing_info")]
    Task<ChannelsSubsidyQueryInvoicingResponse> QueryInvoicingInfoAsync(
        [Body] ChannelsSubsidyQueryInvoicingRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传国补订单发票信息（<c>upload_invoice_info</c>，官方路由 <c>/channels/ec/subsidy/upload_invoice_info</c>）。
    /// </summary>
    /// <param name="request">上传请求（除 <c>invoice_code</c>/<c>invoice_check_code</c> 外均必填，字段表见 <see cref="ChannelsSubsidyUploadInvoiceInfoRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/subsidy/invoicing/api_upload_invoice_info"/></para>
    /// <para>
    /// 官方业务限制：①订单必须是使用了国补的订单 id，且只能在订单确认收货之后调用；
    /// ②<b>同一笔订单最多可以上传 5 次</b>；③<c>invoice_media_id</c> 需先调用「上传国补订单发票文件」获取；
    /// ④<c>invoice_code</c> 发票类型为 90 可选填；<c>invoice_check_code</c> 类型 10 或 04 必填、01/09/90 选填；
    /// ⑤<c>invoice_date</c> 官方说明为「开票时间，秒级时间戳，要求小于当前时间」但参数表类型列标注 string、
    /// 示例为数字（矛盾照录，按说明与示例的数字时间戳建模）；⑥更多要求参考官方「国家补贴发票要求说明」。
    /// </para>
    /// <para>官方错误码（节选）：<c>987100117</c>（上传发票信息达到最大次数）/ <c>987100121</c>（无效的 media_id）/ <c>987100129</c>（字段包含 CSV 分隔符）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/subsidy/upload_invoice_info")]
    Task<ChannelsResponse> UploadInvoiceInfoAsync(
        [Body] ChannelsSubsidyUploadInvoiceInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传国补订单发票文件（<c>upload_invoice_file</c>，官方路由 <c>/channels/ec/subsidy/upload_invoice_file</c>）。
    /// </summary>
    /// <param name="request">上传请求（<c>file_link</c> 必填，文件链接需保证公网可访问）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发票文件 media_id（<c>data.media_id</c>，供上传发票信息接口使用）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/subsidy/invoicing/api_upload_invoice_file"/></para>
    /// <para>
    /// 官方业务限制：①为防止发票文件失效导致审核失败，请使用本接口上传发票文件；
    /// ②仅支持 .jpg/.png 格式，文件大小 ≤ 1MB。
    /// </para>
    /// <para>官方错误码：<c>987100118</c>（链接无效）/ <c>987100119</c>（暂不支持的文件格式）/ <c>987100120</c>（文件过大，最大支持 1M）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/subsidy/upload_invoice_file")]
    Task<ChannelsSubsidyUploadInvoiceFileResponse> UploadInvoiceFileAsync(
        [Body] ChannelsSubsidyUploadInvoiceFileRequest request,
        CancellationToken cancellationToken = default);
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.MarketingBank;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「银行营销任务」域 SDK（2 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径；同时有【普通商户】与【普通服务商】两页的端点，两页字段表经逐页指纹比对<b>完全同形</b>
/// ⇒ <b>一面一方法 + 双页文档引用</b>，只有字段表存在差异的端点才按「同路由多方法」分建。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>分页口径不统一</b>：本域既有 <c>offset</c>/<c>limit</c>（整型页码）也有 <c>page_size</c> + <c>offset</c> 形态，
/// 一律<b>照页</b>，不得互相套用；应答的 <c>total_count</c> 官方多为选填（<c>offset=0</c> 或结果为空时才回）。</para>
/// <para><b>PATCH 与 PUT</b>：批次/活动的改预算与改状态走 <b>PATCH</b>，终止走 <b>POST .../terminate</b>
/// ⇒ 「写操作动词」在本域不统一，照页不得合并。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "MarketingBank", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayMarketingBankService
{
    /// <summary>
    /// 银行营销券码包上传任务。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466703"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012466728"/>（普通服务商）。
    /// </summary>
    /// <param name="packageId">【号码包id】 号码包唯一标识符。可在微信支付商户平台创建号码包后获得。（官方 path <c>package_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>银行营销券码包上传任务，见 <see cref="MarketingBankPackageTaskCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/marketing/bank/packages/{package_id}/tasks</c>；path <c>package_id</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【清算机构】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Post("/v3/marketing/bank/packages/{packageId}/tasks")]
    Task<MarketingBankPackageTaskCreateResponse> CreateBankPackageTaskAsync(
        [Path] string packageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 银行营销券码包上传任务列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012466699"/>（普通商户）、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012466722"/>（普通服务商）。
    /// </summary>
    /// <param name="packageId">【号码包id】 号码包唯一标识符。可在微信支付商户平台创建号码包后获得。（官方 path <c>package_id</c>，<b>必填</b>，类型 <c>string(16)</c>）。</param>
    /// <param name="filename">【文件名】 任务上传的文件名（官方 query <c>filename</c>，选填，类型 <c>string(128)</c>）。</param>
    /// <param name="offset">【分页页码】 分页页码，页码从0开始（官方 query <c>offset</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，单次最多查询20条记录（官方 query <c>limit</c>，选填，类型 <c>integer</c>）。</param>
    /// <param name="status">【任务状态】 上传任务的状态：PROCESSING - 处理中，FINISHED - 已完成。可选取值：`PROCESSING` 上传任务处理中，请等待 / `FINISHED` 上传任务已处理完成，任务上传的文件中的用户已导入到号码包中（官方 query <c>status</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>银行营销券码包上传任务列表，见 <see cref="MarketingBankPackageTaskQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/marketing/bank/packages/{package_id}/tasks</c>；path <c>package_id</c>，query <c>filename</c> / <c>offset</c> / <c>limit</c> / <c>status</c>。</para>
    /// <para><b>支持商户</b>【普通商户】【普通服务商】　【从业机构（银行）】　【从业机构（支付机构）】　【清算机构】　【渠道商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字 ⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>业务错误码</b>：400 INVALID_REQUEST 号码包ID未填或格式不正确 / 400 INVALID_REQUEST 文件内容为空 / 400 INVALID_REQUEST 文件大小超出限制，请分成多个文件上传 / 400 INVALID_REQUEST meta和file中的文件名不一致 / 400 INVALID_REQUEST 基于文件内容计算的sha256与meta传入的sha256不相等 / 400 INVALID_REQUEST 文件的换行符不支持\\r\\n，请使用\\n / 400 INVALID_REQUEST 文件不允许存在空行 / 400 INVALID_REQUEST 不支持的银行类型，请检查并重试 / 400 INVALID_REQUEST 同一号码包不可重复上传同名文件 / 400 PARAM_ERROR HTTP header缺少微信支付平台证书序列号(Wechatpay-Serial) / 404 NOT_FOUND 未找到指定号码包 / 500 SYSTEM_ERROR 系统错误。</para>
    /// </remarks>
    [Get("/v3/marketing/bank/packages/{packageId}/tasks")]
    Task<MarketingBankPackageTaskQueryResponse> QueryBankPackageTasksAsync(
        [Path] string packageId,
        [Query("filename")] string? filename = null,
        [Query("offset")] int? offset = null,
        [Query("limit")] int? limit = null,
        [Query("status")] string? status = null,
        CancellationToken cancellationToken = default);
}

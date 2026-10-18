// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>交易保障</b>域 1 支端点：接口调用耗时与返回信息主动上报。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 商户侧，2026-10-18 逐页核验）：交易保障
/// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935222"/>。</para>
/// <para><b>不需证书</b>：官方「是否需要证书：不需要」⇒ 走声明式管道。
/// <b>域名是 <c>api.mch</c></b>：官方接口地址栏为 <c>https://api.mch.weixin.qq.com/payitil/report</c>。</para>
/// <para><b>官方页不写请求方式</b>：全页无「请求方式」字样（只有接口地址与证书两栏）⇒
/// 本仓按 APIv2 全站惯例落 <c>POST</c>，该选择由契约守卫锁定，不声称出自该页。</para>
/// <para><b>定位是旁路遥测</b>：官方应用场景原文说明它是「主动上报接口」，
/// 微信侧用于优化网络部署与服务监控 ⇒ 上报失败<b>不得</b>影响交易主链路，
/// 也不该把它的应答当业务结果消费。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PayItilV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2PayItilService
{
    /// <summary>
    /// 上报交易信息（APIv2，交易保障）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011935222"/>。
    /// </summary>
    /// <param name="request">上报请求体，见 <see cref="V2ReportTransactionRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果（三行、无签名），见 <see cref="V2ReportTransactionResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<c>/payitil/report</c>；请求表 16 行、返回表 3 行。</para>
    /// <para><b><c>execute_time_</c> 带尾下划线</b>：官方该格原文「注意：该参数最后带有下划线“_”，
    /// 参数设计如此，非文档问题」⇒ 写成 <c>execute_time</c> 会让该字段整支<b>脱离签名串</b>，
    /// 表现为恒 <c>SIGN_ERROR</c> 而不是「缺参数」，是本支最难排查的误用点。</para>
    /// <para><b>必填面比直觉宽</b>：<c>return_code</c>／<c>return_msg</c>／<c>result_code</c>
    /// 与 <c>user_ip</c> 都是<b>入参</b>（报的就是上一次调用的结果），不是应答字段。</para>
    /// <para><b>刷卡支付单独整体上报</b>：官方要求 <c>interface_url</c> 在刷卡场景写
    /// <c>https://api.mch.weixin.qq.com/pay/micropay/total</c>，其余接口按「调用一次、上报一次」。</para>
    /// <para><b>签名算法默认 MD5</b>：官方「目前支持HMAC-SHA256和MD5，默认为MD5」⇒
    /// <c>SignType</c> 不给默认值，换算法须显式赋值。</para>
    /// <para><b>应答无签、无错误码</b>：返回表没有 <c>sign</c> 行，本页「错误码」章节正文即「无」⇒
    /// 失败只能读 <c>return_code</c>=FAIL + <c>return_msg</c> 文本，SDK 不猜结构化错误。</para>
    /// </remarks>
    [Post("/payitil/report")]
    Task<V2ReportTransactionResponse> ReportTransactionAsync(
        V2ReportTransactionRequest request, CancellationToken cancellationToken = default);
}

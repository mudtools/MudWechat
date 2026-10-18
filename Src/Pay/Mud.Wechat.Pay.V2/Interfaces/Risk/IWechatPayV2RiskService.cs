// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>风控</b>域：违规记录查询（机构 / 银行服务商侧）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-18 核验）：违规记录查询
/// <see href="https://pay.weixin.qq.com/doc/v2/institution/4011985446"/>。
/// 该页属<b>合作伙伴（机构）文档树</b>，应用场景原文为「方便银行服务商查询特约商户违规记录」。</para>
/// <para><b>需商户证书</b>：本页「是否需要证书」为<b>「是」</b>，错误码表亦给出
/// <c>CERT_ERROR</c>｜证书错误｜api 证书验证失败」 ⇒ 本接口<b>手写</b>（证书按商户绑定，
/// 与 <see cref="IWechatPayV2DepositSecureService"/> 同一理由），由 <c>WechatPayV2SecureInvoker</c> 承载。</para>
/// <para><b>域名是 <c>api.mch</c></b>：官方本页只给 <c>https://api.mch.weixin.qq.com/risk/getviolation</c>，
/// 全文无 <c>api2.mch</c> ⇒ 不复用 <see cref="IWechatPayV2SecureService"/> 的证书域名。</para>
/// <para><b>请求面没有 <c>appid</c></b>：官方表就 9 行，主体标识只有 <c>mch_id</c> ——
/// 这是 APIv2 线少见的例外，勿按「通用形状」补。</para>
/// <para><b>不出敏感材料</b>（PAY-B7 同线口径）：异常消息不含密钥、<c>sign</c> 或证书材料；
/// 应答的 <c>violation_records</c> 含公司名称与违规子商户号，SDK 不落日志。</para>
/// </remarks>
public interface IWechatPayV2RiskService
{
    /// <summary>
    /// 查询商户违规记录（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/institution/4011985446"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2ViolationRecordQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>记录原文（JSON 或 CSV，由请求的 <c>violation_records_format</c> 决定），见 <see cref="V2ViolationRecordQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/risk/getviolation</c>；XML 信封；
    /// <c>sign_type</c> <b>必填</b>（官方示例取 HMAC-SHA256）。</para>
    /// <para><b>窗口与分页</b>：<c>end_time - begin_time</c> <b>不超过 30 天</b>，超出官方判失败而非截断；
    /// <c>page_size</c> 最大 <b>50</b>；两个时间字段格式为 <c>yyyy-MM-dd HH:mm:ss</c>。</para>
    /// <para><b>记录载体是字符串</b>：官方不把记录做成 XML 子树，而是把 JSON 或 CSV 整段塞进
    /// <c>violation_records</c> 一个字段 ⇒ 本 SDK 原样交回 <see cref="string"/>，
    /// <b>不猜结构、不做二次解析</b>（CSV 形态官方自己的示例都含未转义逗号，解析口径交给商户）。</para>
    /// <para><b>门控只有一级</b>：应答的 <c>result_code</c> 与 <c>violation_records</c> 同处
    /// 由 <c>return_code</c> 把关的表内，没有第三级成功块；本支另有 <c>result_msg</c> 字段。</para>
    /// <para><b>失败码</b>：<c>SYSTEM_ERROR</c> / <c>AUTH_ERROR</c> 未开通权限 / <c>PARAM_ERROR</c> /
    /// <c>SIGN_ERROR</c> / <c>CERT_ERROR</c> / <c>INPUT_FORMAT_ERROR</c>
    /// （注意本族是<b>下划线形态</b>的 <c>SIGN_ERROR</c>，与商户侧各页的 <c>SIGNERROR</c> 不同）。</para>
    /// </remarks>
    Task<V2ViolationRecordQueryResponse> GetViolationRecordAsync(
        V2ViolationRecordQueryRequest request, CancellationToken cancellationToken = default);
}

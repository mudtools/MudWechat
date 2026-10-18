// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 服务商付款到银行卡族（<b>2 支端点，全部需商户 API 证书</b>）：
/// 付款到银行卡 / 查询付款到银行卡。
/// </summary>
/// <remarks>
/// <para>
/// <b>形态与理由</b>：与 <see cref="IWechatPayV2TransferService"/> 同因（证书按商户绑定 ⇒ 逐商户独立通道），
/// 官方域名为 <c>api.mch</c>；本族挂在 <c>/mmpaysptrans/*</c> 路径下，与零钱族（<c>/mmpaymkttransfers/*</c>）不同前缀。
/// </para>
/// <para>
/// <b>加密口径易踩错</b>：银行卡号与户名<b>不是</b> APIv2 常规的 AES-GCM/<c>req_info</c>，而是
/// <b>RSA OAEP</b>（Java 侧 <c>RSA/ECB/OAEPPadding</c>，即 <c>RSA/ECB/OAEPWITHSHA-1ANDMGF1PADDING</c>），
/// 公钥由 <c>/risk/getpublickey</c> 取；本 SDK <b>不代做加密</b>，加密交宿主（密钥与合规责任留在宿主侧）。
/// </para>
/// <para>
/// <b>个人金融信息</b>（PAY-B7 同线红线，本族是最敏感的一族）：<c>enc_bank_no</c> / <c>enc_true_name</c>
/// 与其明文形态、以及查询返回的 <c>bank_no_md5</c> / <c>true_name_md5</c> 全部属个人金融信息 ⇒
/// SDK 不落日志、不进异常消息；宿主须自行确保已取得收款人授权。
/// </para>
/// </remarks>
public interface IWechatPayV2PartnerTransferService
{
    /// <summary>
    /// 付款到银行卡（APIv2 服务商，需商户证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011939840"/>。
    /// </summary>
    /// <param name="request">付款请求体（卡号与户名须已按 RSA OAEP 加密），见 <see cref="V2PayBankRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果，含 <c>partner_trade_no</c> 与官方分账金额字段，见 <see cref="V2PayBankResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaysptrans/pay_bank</c>（域名 <c>api.mch</c>）；
    /// XML 报文 + 报文内 <c>sign</c> + <b>双向证书</b>。</para>
    /// <para><b>受理语义</b>：本支返回的是<b>受理</b>结果，到账与否须经
    /// <see cref="QueryBankAsync"/> 按 <c>status</c>（<c>PROCESSING</c> / <c>SUCCESS</c> / <c>FAILED</c> / <c>BANK_FAIL</c>）确认，
    /// 不得把受理成功当作付款成功。</para>
    /// <para><b>幂等</b>：<c>partner_trade_no</c> 为唯一键 ⇒ 超时或 <c>SYSTEMERROR</c> 后<b>沿用原单号</b>重试。</para>
    /// <para><b>业务限制</b>（官方本页）：频率 <b>30 次/秒</b>，超过 <b>600 次/分钟</b> 官方直接返回
    /// <c>FREQUENCY_LIMITED</c>；金额面 <b>单商户单日 10 万元</b>、<b>单笔 2 万元</b>、
    /// <b>同一张银行卡单日累计 2 万元</b>。需先开通「付款到银行卡」权限并预存「商家转账」额度。</para>
    /// <para><b>不支持公钥证书</b>：本族只认 <c>apiclient_cert.p12</c> 双向证书形态。</para>
    /// </remarks>
    Task<V2PayBankResponse> PayBankAsync(
        V2PayBankRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询付款到银行卡（APIv2 服务商，需商户证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011941066"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2QueryBankRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>付款状态与官方原因字段，见 <see cref="V2QueryBankResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaysptrans/query_bank</c>；XML + <b>双向证书</b>。</para>
    /// <para><b>查询入参形态</b>：官方要求传银行卡号与户名的 <b>MD5</b>（<c>bank_no_md5</c> / <c>true_name_md5</c>）
    /// 而非密文 ——与付款支的 RSA OAEP 是<b>两套</b>口径，不要复用同一份加密产物。</para>
    /// <para><b>轮询纪律</b>：<c>status</c> 处于 <c>PROCESSING</c> 期间官方仍未定局 ⇒
    /// 应以合理间隔轮询本支而非高频打满；终态只有 <c>SUCCESS</c> / <c>FAILED</c> / <c>BANK_FAIL</c>。</para>
    /// </remarks>
    Task<V2QueryBankResponse> QueryBankAsync(
        V2QueryBankRequest request, CancellationToken cancellationToken = default);
}

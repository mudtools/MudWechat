// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>防欺诈</b>域 1 支端点：获取防欺诈（付款到银行卡用的）RSA 公钥。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 商户侧，2026-10-18 逐页核验）：获取防欺诈平台证书
/// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011941097"/>。</para>
/// <para><b>需商户证书 ⇒ 手写</b>：本页「是否需要证书」为「是」，而证书按商户绑定，
/// 声明式管道无从拿到 ⇒ 落 <c>WechatPayV2SecureInvoker</c>，
/// 与 <see cref="IWechatPayV2RiskService"/> 同一理由、同一口径。</para>
/// <para><b>域名是本线第四个：<c>fraud.mch</c></b>：本页接口地址栏逐字印
/// <c>https://fraud.mch.weixin.qq.com/risk/getpublickey</c>，<b>既非 <c>api.mch</c> 亦非 <c>api2.mch</c></b>
/// ⇒ 取 <c>WechatPayV2HttpClientNames.FraudBaseAddress</c>。
/// 注意这与同前缀的违规记录查询<b>不同域</b>（那一支官方给的是 <c>api.mch</c>）——
/// 「同 <c>/risk/</c> 前缀 ⇒ 同域名」这个直觉在 APIv2 不成立。</para>
/// <para><b>它是「付款到银行卡」的前置取数接口</b>：<c>/mmpaysptrans/pay_bank</c> 要求
/// <c>enc_bank_no</c>／<c>enc_true_name</c> 用本公钥做 RSA OAEP 加密；那两支出参属<b>个人金融信息</b>，
/// 本 SDK 不做加密也不落日志，加密环节留给宿主。</para>
/// </remarks>
public interface IWechatPayV2FraudService
{
    /// <summary>
    /// 获取防欺诈公钥（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011941097"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="V2GetPublicKeyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>公钥原文（PKCS#1 PEM），见 <see cref="V2GetPublicKeyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/risk/getpublickey</c>；XML 信封；
    /// 请求面<b>只有 4 行且没有 <c>appid</c></b>（<c>mch_id</c>／<c>nonce_str</c>／<c>sign_type</c>／<c>sign</c>）。</para>
    /// <para><b>算法双档、无默认</b>：官方「支持HMAC-SHA256和MD5」，示例取 MD5、页面不写默认值 ⇒
    /// <c>SignType</c> 不给默认值，由调用方显式选定（与「仅 HMAC-SHA256」的族相反）。</para>
    /// <para><b>应答无签 ⇒ 不可验签</b>：返回表 7 行里没有 <c>sign</c>（<c>return_code</c>/
    /// <c>return_msg</c>/<c>result_code</c>/<c>err_code</c>/<c>err_code_des</c>/<c>mch_id</c>/<c>pub_key</c>），
    /// 收发器的应答验签在此天然放行，不是漏验。</para>
    /// <para><b>公钥是 PKCS#1</b>：官方示例头尾为 <c>-----BEGIN RSA PUBLIC KEY-----</c>，
    /// <b>不是</b> PKCS#8 的 <c>-----BEGIN PUBLIC KEY-----</c>；<c>netstandard2.0</c> 无 PKCS#1 导入 API ⇒
    /// 本包只承担「取回原文」，格式选择交给宿主，不替宿主猜。</para>
    /// <para><b>无轮换口径</b>：本页未说明有效期、长度上限或轮换策略 ⇒ SDK 不实现任何缓存/定时刷新逻辑。
    /// 公钥本身非机密，可进配置；但<b>不得</b>把 <c>sign</c> 值或密钥材料写日志（PAY-B7 同线红线）。</para>
    /// </remarks>
    Task<V2GetPublicKeyResponse> GetPublicKeyAsync(
        V2GetPublicKeyRequest request, CancellationToken cancellationToken = default);
}

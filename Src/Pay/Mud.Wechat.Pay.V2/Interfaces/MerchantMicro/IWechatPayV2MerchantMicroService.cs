// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>小微商户资料修改</b>域 2 支端点（结算银行卡 / 联系信息）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 合作伙伴侧，2026-10-18 逐页核验）：
/// 修改小微商户银行资料 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115349"/>、
/// 修改小微商户联系人信息 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115350"/>。
/// 两页的「是否需要证书」栏均为<b>「是，详见证书使用」</b>。</para>
/// <para><b>域名是 <c>api.mch</c> 而非 <c>api2.mch</c></b>：两页「接口说明」栏给的是
/// <c>https://api.mch.weixin.qq.com/applyment/micro/*</c> ⇒ 本接口走
/// <see cref="WechatPayV2HttpClientNames.BaseAddress"/>，与 <see cref="IWechatPayV2SecureService"/> 的证书域名不同
/// （<c>/secapi/</c> 前缀不等于证书域名，本线已两次验证该口径）。</para>
/// <para><b>为什么手写</b>：客户端证书按<b>商户</b>绑定，与「一条命名客户端一个连接池」的生成式模型冲突
/// （理由同 <see cref="IWechatPayV2SecureService"/>），故由 <c>WechatPayV2SecureInvoker</c>
/// 走逐商户证书通道；报文规则与声明式管道完全同源。</para>
/// <para><b>两支的签名面同一</b>：<c>version</c>（官方默认 <c>1.0</c>）+ <c>nonce_str</c> +
/// <c>sign_type</c> 必填，官方原文「签名类型，仅支持HMAC-SHA256。该字段需参与签名sign的计算。」
/// ⇒ 清空 <c>sign_type</c> 会把报文降成 MD5 而被官方拒；两支的应答表逐行相同，故共用一个响应类型。</para>
/// <para><b>敏感字段属个人金融信息</b>：银行号 <c>account_number</c>、手机号 <c>mobile_phone</c>、
/// 邮箱 <c>email</c> 官方明示「需要进行加密处理」⇒ 请求面承载<b>密文</b>，
/// 无论明文密文一律不得进日志、遥测或异常消息（PAY-B7 同线红线）。</para>
/// </remarks>
public interface IWechatPayV2MerchantMicroService
{
    /// <summary>
    /// 修改小微商户结算银行卡资料（APIv2，需证书，仅 HMAC-SHA256）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4014115349"/>。
    /// </summary>
    /// <param name="request">修改请求体，见 <see cref="V2MicroArchivesModifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改结果，见 <see cref="V2MicroModifyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/applyment/micro/modifyarchives</c>；XML；
    /// 请求面 11 行（<c>version</c>/<c>mch_id</c>/<c>nonce_str</c>/<c>sign_type</c>/<c>sign</c>/
    /// <c>sub_mch_id</c>/<c>account_number</c>/<c>bank_name</c>/<c>account_bank</c>/
    /// <c>bank_address_code</c>/<c>cert_sn</c>）。</para>
    /// <para><b>加密口径</b>：<c>account_number</c> 须用<b>微信支付平台证书公钥</b>做 RSAES-OAEP
    /// 后 Base64 上传（SDK 侧加密入口 <c>WechatPayV2SensitiveFieldEncrypter</c>）；
    /// <c>bank_name</c>／<c>account_bank</c>／<c>bank_address_code</c> 官方<b>未</b>标敏感，按明文上传。</para>
    /// <para><b><c>cert_sn</c> 与密文必须同源</b>：官方描述「用于对敏感信息进行加密的平台证书序列号」，
    /// 取法为获取平台证书接口应答的 <c>serial_no</c>（不是商户证书序列号）⇒
    /// 用 A 证书加密却填 B 序列号会直接解密失败。</para>
    /// <para><b>17 家直连银行免填 <c>bank_name</c></b>：官方原文「17家直连银行无需填写，其他银行务必填写」，
    /// 取值须命中「开户银行全称（含支行）对照表」；<c>bank_address_code</c> 至少精确到市。</para>
    /// <para><b>校验型修改</b>：官方失败样本的 <c>err_code_des</c> 是「银行卡存在，但与姓名证件号其中一项或全部不匹配」
    /// ⇒ 卡号与姓名／证件号三元组必须一致，不是「写什么收什么」。</para>
    /// </remarks>
    Task<V2MicroModifyResponse> ModifyArchivesAsync(
        V2MicroArchivesModifyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改小微商户联系人信息（APIv2，需证书，仅 HMAC-SHA256）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4014115350"/>。
    /// </summary>
    /// <param name="request">修改请求体，见 <see cref="V2MicroContactInfoModifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>修改结果，见 <see cref="V2MicroModifyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/applyment/micro/modifycontactinfo</c>；XML；
    /// 请求面在信封之外只有 <c>sub_mch_id</c>/<c>mobile_phone</c>/<c>email</c>/<c>merchant_name</c>/<c>cert_sn</c>。</para>
    /// <para><b>三个业务字段全部选填</b>（官方必填栏为「否」）⇒ 本 SDK 不做「至少填一个」的拦截，
    /// 空业务字段的修改语义由官方判定；<c>cert_sn</c> 仍为必填。</para>
    /// <para><b><c>mobile_phone</c> 与 <c>email</c> 是敏感字段</b>：官方要求加密处理，
    /// 请求面承载密文；<c>merchant_name</c> 官方未标敏感，明文上传。</para>
    /// <para><b>官方自身的长度矛盾</b>：<c>merchant_name</c> 类型栏 String(20)、描述栏「请输入 1-64 个字符」
    /// ⇒ 本 SDK 不做长度拦截、只照抄字段名，越界由官方回 <c>PARAM_ERROR</c>。</para>
    /// </remarks>
    Task<V2MicroModifyResponse> ModifyContactInfoAsync(
        V2MicroContactInfoModifyRequest request, CancellationToken cancellationToken = default);
}

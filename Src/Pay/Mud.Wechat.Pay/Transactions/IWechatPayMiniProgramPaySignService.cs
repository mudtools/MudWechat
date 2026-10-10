// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Transactions;

namespace Mud.Wechat.Pay.Transactions;

/// <summary>
/// 小程序调起支付参数签名（<b>本地签名，无 HTTP</b>）—— 官方 P1-a 清单中的「小程序调起支付」端点。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791898"/>。
/// 小程序端 <c>wx.requestPayment</c> 需要 <c>timeStamp</c> / <c>nonceStr</c> / <c>package</c> /
/// <c>signType</c> / <c>paySign</c> 五个参数，其中 <c>paySign</c> 必须由<b>服务端</b>用商户私钥计算
/// （私钥绝不外泄到端上）。本接口即为该计算的服务端入口。
/// </para>
/// <para>
/// <b>为什么它不是 <c>[HttpClientApi]</c> 接口</b>：官方把它列在下单产品的 TOC 里，但它是<b>本地密码学运算</b>
/// —— 没有请求路由、没有应答体。与 <c>IWechatPaySimulatedSignatureService</c> 之类的「伪端点」不同，
/// 这里连 HTTP 形态都不存在，故不做成生成客户端（生成管线要求有路由与 DTO 应答）。
/// </para>
/// <para>
/// <b>商户归属</b>：按当前环境商户（<c>IWechatPayMerchantContext</c>）取私钥 —— 与传输层签名<b>同一套</b>
/// 商户解析规则；单商户宿主零样板，多商户宿主用 <c>UseMerchant(key)</c> 作用域。
/// </para>
/// <para>
/// <b>PAY-B7</b>：本接口只产出签名值，不产出也不记录任何私钥材料；<c>paySign</c> 可入日志（它是一次性凭证）。
/// </para>
/// </remarks>
public interface IWechatPayMiniProgramPaySignService
{
    /// <summary>
    /// 计算小程序调起支付参数。
    /// </summary>
    /// <param name="appId">
    /// 调起支付小程序的 AppID（<b>必须</b>与下单时传入的 <c>appid</c> 完全一致 ——
    /// 微信支付会校验下单与调起支付所用 appid 的一致性）。
    /// </param>
    /// <param name="prepayId">下单接口（<c>POST /v3/pay/transactions/jsapi</c>）返回的 <c>prepay_id</c>（<b>2 小时</b>有效）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>可直接下发给小程序端的五参数（<b>字段名为官方驼峰写法</b>）。</returns>
    /// <exception cref="ArgumentException"><paramref name="appId"/> 或 <paramref name="prepayId"/> 为空白。</exception>
    /// <exception cref="InvalidOperationException">商户未确定（未注册 / 多商户且未开作用域）。</exception>
    Task<WechatPayMiniProgramPaySign> CreatePaySignAsync(
        string appId,
        string prepayId,
        CancellationToken cancellationToken = default);
}

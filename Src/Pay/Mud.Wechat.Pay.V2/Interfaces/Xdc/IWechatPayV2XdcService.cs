// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>XDC 仿真（沙箱）</b>域 1 支端点：获取验签密钥。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（2026-10-18 逐页核验）：支付验收指引
/// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011984810"/>。
/// <b>注意出处形态</b>：官方<b>没有</b>本端点的独立 API 参考页，它只是该指引页里
/// 「二、仿真测试系统 → 沙箱系统」小节下的一支（原文「测试仿真测试系统的API验签密钥需从API获取」），
/// 字段表以该小节为唯一权威，且<b>该小节不给请求/应答示例报文</b>。</para>
/// <para><b>不需证书、POST、挂在 <c>api.mch</c></b>：该小节印的完整地址为
/// <c>https://api.mch.weixin.qq.com/xdc/apiv2getsignkey/sign/getsignkey</c>、请求方式 POST、
/// 是否需要证书为否 ⇒ 走声明式管道。</para>
/// <para><b>本支不落证书、也不落沙箱转发</b>：同一页说明仿真系统的接法是「正式 API 的 URL 增加一层
/// <c>xdc/apiv2sandbox</c>」，异常用例靠 Http Header <c>Wechatpay-Negative-Test</c> 识别 ——
/// 那属于「把生产调用改道到沙箱」的能力，<b>不在</b>本契约面内；本仓只落这一支取数接口，
/// 由宿主自己决定拿到密钥后怎么用它签名。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "XdcV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2XdcService
{
    /// <summary>
    /// 获取仿真（沙箱）验签密钥（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011984810"/>。
    /// </summary>
    /// <param name="request">请求体，见 <see cref="V2GetSandboxSignKeyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>沙箱验签密钥，见 <see cref="V2GetSandboxSignKeyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<c>/xdc/apiv2getsignkey/sign/getsignkey</c>；请求面<b>只有三行</b>
    /// （<c>mch_id</c>／<c>nonce_str</c>／<c>sign</c>），<b>没有 <c>appid</c>，也没有 <c>sign_type</c></b>
    /// ⇒ 无 <c>sign_type</c> 即由签名器落 MD5 缺省档，
    /// 官方也把 <c>sign</c> 的长度写成 String(32)（MD5 摘要长度）。</para>
    /// <para><b>应答无签、无 <c>result_code</c></b>：返回表为 <c>return_code</c>／<c>return_msg</c>
    /// 加成功段的 <c>mch_id</c>／<c>sandbox_signkey</c> ⇒ 本支<b>不可验签</b>；
    /// 失败只在 <c>return_msg</c> 文本里（官方列「签名失败」「参数格式校验错误」），本页无错误码表。</para>
    /// <para><b>用哪份密钥签这一支</b>：官方页未写明；从取数顺序看此时沙箱密钥尚未取得，
    /// 只能是商户 APIv2 主密钥 ⇒ 本仓沿用收发器的默认签名密钥，不做特殊分支。</para>
    /// <para><b>凭据不回显</b>（PAY-B7 同线红线）：<c>sandbox_signkey</c> 是仿真系统的验签密钥，
    /// 与生产 API 密钥同敏感级 ⇒ 不得进日志、遥测或异常消息；SDK 也不缓存、不自动注入收发器。</para>
    /// </remarks>
    [Post("/xdc/apiv2getsignkey/sign/getsignkey")]
    Task<V2GetSandboxSignKeyResponse> GetSandboxSignKeyAsync(
        V2GetSandboxSignKeyRequest request, CancellationToken cancellationToken = default);
}

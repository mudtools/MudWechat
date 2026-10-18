// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 仿真（沙箱）验签密钥获取（<c>/xdc/apiv2getsignkey/sign/getsignkey</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>出处形态特殊</b>：官方<b>没有</b>该端点的独立 API 参考页，它只是「支付验收指引」
/// （更新 2025.03.21）里「二、仿真测试系统 → 沙箱系统」小节下的一支
/// 「测试仿真测试系统的API验签密钥需从API获取」⇒ 字段表以该小节为唯一权威，本页<b>无请求/应答示例报文</b>。</para>
/// <para><b>请求面只有三行</b>：<c>mch_id</c>／<c>nonce_str</c>／<c>sign</c>，
/// <b>没有 <c>appid</c>，也没有 <c>sign_type</c></b>⇒ 与通用形状差异大，照抄不得补齐。
/// 签名器按字段表判定的口径下，无 <c>sign_type</c> 即落 MD5 缺省档，
/// 官方把 <c>sign</c> 的长度也写成 String(32)（MD5 摘要长度，而非其余接口的 String(64)）。</para>
/// <para><b>用哪份密钥签这一支</b>：官方页未写明；从取数顺序看，此时沙箱密钥尚未取得，
/// 故只能是商户的 APIv2 主密钥（生产 API 密钥）。本 SDK 沿用收发器的默认签名密钥，不做特殊分支。</para>
/// <para><b>域名是 <c>api.mch</c></b>：官方给出的完整地址为
/// <c>https://api.mch.weixin.qq.com/xdc/apiv2getsignkey/sign/getsignkey</c>、请求方式 POST、
/// 「是否需要证书：不需要」⇒ 走 <c>WechatPayV2HttpClientNames.BaseAddress</c>。</para>
/// <para><b>与沙箱路径前缀是两件事</b>：同一页说明仿真系统的接入方式是「正式 API 的调用 URL 增加一层
/// <c>xdc/apiv2sandbox</c>」（如 <c>pay/micropay</c> → <c>xdc/apiv2sandbox/pay/micropay</c>），
/// 异常用例靠 Http Header <c>Wechatpay-Negative-Test: {用例名}</c> 识别；
/// 而本支自己的前缀是 <c>xdc/apiv2getsignkey/sign/</c>，<b>不是</b> <c>xdc/apiv2sandbox/</c> 家族，
/// 也不提供沙箱业务调用 ⇒ 本仓只落这一支取数接口，不实现沙箱转发。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2GetSandboxSignKeyRequest
{
    /// <summary>商户号（<c>mch_id</c>，必填 string(32)）：微信支付分配的微信商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串（<c>nonce_str</c>，必填 string(32)）：不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名（<c>sign</c>，必填 string(32)）：由签名器补写，调用方留空。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 仿真（沙箱）验签密钥应答。
/// </summary>
/// <remarks>
/// <para><b>应答面没有 <c>sign</c>，也没有 <c>result_code</c></b>：官方返回表为
/// <c>return_code</c>／<c>return_msg</c> 加「以下字段在 return_code 为 SUCCESS 的时有返回」段里的
/// <c>mch_id</c>／<c>sandbox_signkey</c>⇒ 本支<b>不可验签</b>，
/// 失败也只体现在 <c>return_code</c>=FAIL + <c>return_msg</c> 文本（官方列「签名失败」「参数格式校验错误」），
/// 本页<b>无错误码表</b>。</para>
/// <para><b><c>sandbox_signkey</c> 是凭据，不是业务数据</b>：它是仿真系统后续所有调用的验签密钥，
/// 与生产 API 密钥同敏感级 ⇒ <b>不得</b>写日志、进异常消息或落配置明文；本 SDK 只把它原样交回宿主，
/// 不缓存、不注入到任何收发器（沙箱调用本身不在本仓契约面内）。</para>
/// <para><b>官方标注的类型长度是 String(32)</b>，示例值 <c>013467007045764</c> 与「设备号」撞值，
/// 属文档示例复用，不作长度校验依据。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2GetSandboxSignKeyResponse
{
    /// <summary>通信标识（<c>return_code</c>）：SUCCESS/FAIL。</summary>
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息（<c>return_msg</c>）：FAIL 时为错误原因（签名失败 / 参数格式校验错误）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>商户号（<c>mch_id</c>，回显请求）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>
    /// 沙箱验签密钥（<c>sandbox_signkey</c>）：<b>凭据</b>，不得进日志、遥测或异常消息。
    /// </summary>
    [JsonPropertyName("sandbox_signkey")] public string? SandboxSignKey { get; set; }
}

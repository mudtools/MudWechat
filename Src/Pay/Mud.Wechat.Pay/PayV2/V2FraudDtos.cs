// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 防欺诈公钥获取（<c>/risk/getpublickey</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>请求面只有 4 个字段，且没有 <c>appid</c></b>（官方表就 <c>mch_id</c>／
/// <c>nonce_str</c>／<c>sign_type</c>／<c>sign</c> 四行）⇒ 与同前缀的违规记录查询一样是
/// 「不带 appid」的例外形状，照抄不得按通用形状补齐。</para>
/// <para><b><c>sign_type</c> 必填、双算法皆可</b>：官方「支持HMAC-SHA256和MD5」，示例值取 <c>MD5</c>，
/// 页面<b>不写默认值</b> ⇒ 本类<b>不给</b>默认值（与「仅 HMAC-SHA256」的族相反）：
/// 留空即由调用方显式选算法，签名器按字段表判定，缺省即 MD5。</para>
/// <para><b>域名是 <c>fraud.mch</c></b>：官方本页接口说明栏的完整地址为
/// <c>https://fraud.mch.weixin.qq.com/risk/getpublickey</c>，既非 <c>api.mch</c> 亦非 <c>api2.mch</c>
/// ⇒ 走 <c>WechatPayV2HttpClientNames.FraudBaseAddress</c>（第 4 个编译期常量，见该类的闭集说明）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2GetPublicKeyRequest
{
    /// <summary>微信支付分配的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串，长度小于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型：必填，官方支持 <c>HMAC-SHA256</c> 与 <c>MD5</c>，无默认值 ⇒ 由调用方显式选定。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>签名（由证书收发器按字段表计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 防欺诈公钥应答。
/// </summary>
/// <remarks>
/// <para><b>应答面没有 <c>sign</c></b>：官方返回表就 7 个字段
/// （<c>return_code</c>／<c>return_msg</c>／<c>result_code</c>／<c>err_code</c>／
/// <c>err_code_des</c>／<c>mch_id</c>／<c>pub_key</c>）⇒ 本支<b>不可验签</b>，
/// 收发器的应答验签在此天然放行（不是漏验，是官方无签可验）。</para>
/// <para><b><c>pub_key</c> 是 PKCS#1 PEM</b>：官方示例的头尾为
/// <c>-----BEGIN RSA PUBLIC KEY-----</c> / <c>-----END RSA PUBLIC KEY-----</c>，
/// <b>不是</b> <c>-----BEGIN PUBLIC KEY-----</c>（PKCS#8）⇒ 宿主侧解析时选错格式会直接抛，
/// 且本包不替宿主猜：原样交回文本。<see cref="System.Security.Cryptography.RSA"/> 在
/// <c>netstandard2.0</c> 无 PKCS#1 导入 API，若要在 SDK 内消费该公钥需另加解析层，
/// 当前只承担「取回」这一支契约。</para>
/// <para><b>公钥不是机密</b>：<c>pub_key</c> 是公钥，可以进配置与日志口径不同于密钥；
/// 但本页官方<b>未说明</b>长度上限、有效期与轮换策略 ⇒ SDK 不实现任何「定期刷新/缓存过期」逻辑，
/// 由宿主按自己的节奏取用。</para>
/// <para><b>本支的用途</b>：付款到银行卡（<c>/mmpaysptrans/pay_bank</c>）要求
/// <c>enc_bank_no</c>／<c>enc_true_name</c> 用该公钥做 RSA OAEP 加密 ⇒ 它是「付款到银行卡」族
/// 的前置取数接口；那两支出参属<b>个人金融信息</b>，本 SDK 不做加密也不落日志。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2GetPublicKeyResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>微信支付分配的商户号（回显请求）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>
    /// 防欺诈公钥原文：<b>PKCS#1 PEM</b>（<c>-----BEGIN RSA PUBLIC KEY-----</c>）。公钥，非机密。
    /// </summary>
    [JsonPropertyName("pub_key")] public string? PublicKey { get; set; }
}

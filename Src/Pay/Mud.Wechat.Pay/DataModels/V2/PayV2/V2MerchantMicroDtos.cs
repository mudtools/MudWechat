// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 小微商户<b>修改结算银行卡</b>（<c>/applyment/micro/modifyarchives</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>两支小微端点共用一张签名面</b>：<c>version</c>（官方默认 <c>1.0</c>）+
/// <c>nonce_str</c> + <c>sign_type</c> <b>必填且仅支持 HMAC-SHA256</b>，官方原文
/// 「该字段需参与签名 sign 的计算」⇒ <see cref="SignType"/> 的默认值属契约面。</para>
/// <para><b>敏感字段必须加密后上传</b>：官方把 <c>account_number</c> 标为
/// 「该字段属于敏感字段，需要进行加密处理」⇒ 本字段承载的是<b>密文</b>（平台证书公钥
/// RSAES-OAEP 加密后的 Base64），不是银行卡号明文。加密入口见
/// <c>WechatPayV2SensitiveFieldEncrypter</c>。</para>
/// <para><b><c>cert_sn</c> 是加密用的那一张平台证书的序列号</b>（不是商户证书序列号）：
/// 官方取法为「获取平台证书接口应答的 <c>serial_no</c>」⇒ 密文与 <c>cert_sn</c> 必须同源，
/// 用 A 证书加密却填 B 序列号会直接解密失败。</para>
/// <para><b>不出密文</b>（PAY-B7 同线口径）：<c>account_number</c> 无论明文还是密文都不得进日志、
/// 不得拼进异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2MicroArchivesModifyRequest
{
    /// <summary>协议版本，官方默认值 <c>1.0</c>（默认值属契约面）。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; } = "1.0";

    /// <summary>服务商的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型，官方「仅支持 HMAC-SHA256」且该字段参与 <c>sign</c> 计算；默认值即契约面。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>签名（由证书收发器计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>小微商户的商户号。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>
    /// 商户结算银行卡号<b>密文</b>（官方敏感字段，须用平台证书公钥加密后 Base64 上传）。
    /// <b>禁止写入日志与异常消息。</b>
    /// </summary>
    [JsonPropertyName("account_number")] public string? AccountNumber { get; set; }

    /// <summary>
    /// 开户银行全称（含支行），如「深圳农村商业银行XXX支行」。
    /// 官方：17 家直连银行无需填写，其他银行务必填写；取值见「开户银行全称（含支行）对照表」。
    /// </summary>
    [JsonPropertyName("bank_name")] public string? BankName { get; set; }

    /// <summary>开户银行，取值见「开户银行对照表」。</summary>
    [JsonPropertyName("account_bank")] public string? AccountBank { get; set; }

    /// <summary>开户银行省市编码，<b>至少精确到市</b>，取值见「微信支付提供省市对照表」。官方必填。</summary>
    [JsonPropertyName("bank_address_code")] public string? BankAddressCode { get; set; }

    /// <summary>
    /// 用于加密敏感字段的<b>微信支付平台证书</b>序列号（取获取平台证书接口应答的 <c>serial_no</c>）。
    /// 须与 <see cref="AccountNumber"/> 密文所用的证书同源。
    /// </summary>
    [JsonPropertyName("cert_sn")] public string? CertSn { get; set; }
}

/// <summary>
/// APIv2 小微商户<b>修改联系信息</b>（<c>/applyment/micro/modifycontactinfo</c>）请求体。
/// </summary>
/// <remarks>
/// <para><b>签名面与 <see cref="V2MicroArchivesModifyRequest"/> 同一</b>（<c>version</c>／
/// <c>nonce_str</c>／<c>sign_type</c> 必填 HMAC-SHA256 且参与签名）。</para>
/// <para><b>本支的两个敏感字段</b>：<c>mobile_phone</c> 与 <c>email</c> 官方均标
/// 「该字段属于敏感信息，需要进行加密处理」⇒ 承载<b>密文</b>；
/// <c>merchant_name</c> 官方<b>未</b>标敏感，按明文上传。</para>
/// <para><b>官方类型与描述自相矛盾</b>：<c>merchant_name</c> 类型栏标 String(20)，
/// 描述栏却写「请输入 1-64 个字符」⇒ 长度校验以官方字段表的 20 为准更保守，本 SDK 不做长度拦截、
/// 只照抄字段名，越界由官方回 <c>PARAM_ERROR</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2MicroContactInfoModifyRequest
{
    /// <summary>协议版本，官方默认值 <c>1.0</c>。</summary>
    [JsonPropertyName("version")] public string? Version { get; set; } = "1.0";

    /// <summary>服务商的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>随机字符串，不长于 32 位。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>签名类型，官方「仅支持 HMAC-SHA256」且该字段参与 <c>sign</c> 计算；默认值即契约面。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; } = "HMAC-SHA256";

    /// <summary>签名（由证书收发器计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>小微商户的商户号。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }

    /// <summary>
    /// 小微商户手机号<b>密文</b>（官方敏感字段，须加密后 Base64 上传；用于接收重要管理信息与操作验证码）。
    /// 官方对明文的格式要求：前后无空格／制表符／换行符，且为 11 位手机号或 5-20 位的数字 + <c>-</c> + <c>+</c>。
    /// <b>禁止写入日志与异常消息。</b>
    /// </summary>
    [JsonPropertyName("mobile_phone")] public string? MobilePhone { get; set; }

    /// <summary>小微商户邮箱<b>密文</b>（官方敏感字段，须加密后 Base64 上传）。<b>禁止写入日志与异常消息。</b></summary>
    [JsonPropertyName("email")] public string? Email { get; set; }

    /// <summary>商户名称，在支付完成页向买家展示；须与微信经营类目相关（非敏感字段，明文上传）。</summary>
    [JsonPropertyName("merchant_name")] public string? MerchantName { get; set; }

    /// <summary>加密所用<b>微信支付平台证书</b>序列号，须与密文同源（官方必填）。</summary>
    [JsonPropertyName("cert_sn")] public string? CertSn { get; set; }
}

/// <summary>
/// APIv2 小微商户两支修改端点的应答（官方两张表逐行相同，故共用一个类型）。
/// </summary>
/// <remarks>
/// <para><b>成功块回显主体标识</b>：<c>mch_id</c> 与 <c>sub_mch_id</c> 在
/// <c>result_code = SUCCESS</c> 时返回，应答同时带 <c>nonce_str</c> 与 <c>sign</c>
/// （⇒ 本 SDK 的证书收发器会做应答验签）。</para>
/// <para><b>官方本页的失败样本很有信息量</b>：<c>err_code_des</c> 示例是
/// 「银行卡存在，但与姓名证件号其中一项或全部不匹配」⇒ 本支是<b>校验型</b>修改，
/// 卡号与姓名／证件号的三元组必须一致，不是「写什么收什么」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2MicroModifyResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息，官方列举的失败原因：只支持 POST 方法、xml 格式错误、协议转换时发生系统错误、未检测到必填参数 XXX、请求参数签名错误、返回结果时签名失败。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>应答随机字符串（成功块返回）。</summary>
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }

    /// <summary>微信返回的签名（须验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    /// <summary>服务商的商户号（<c>result_code = SUCCESS</c> 时返回）。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>小微商户的商户号（<c>result_code = SUCCESS</c> 时返回）。</summary>
    [JsonPropertyName("sub_mch_id")] public string? SubMchId { get; set; }
}

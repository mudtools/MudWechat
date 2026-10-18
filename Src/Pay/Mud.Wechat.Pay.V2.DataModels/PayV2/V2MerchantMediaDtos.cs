// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 商户图片上传（<c>/secapi/mch/uploadmedia</c>）请求体 —— <b>只装参与签名的表单分节</b>，
/// 图片本体作为方法参数传入（见 <c>IWechatPayV2MerchantMediaService.UploadMediaAsync</c>）。
/// </summary>
/// <remarks>
/// <para><b>本支是 APIv2 线唯一的 <c>multipart/form-data</c> 端点</b>（其余全部是 XML 体）：
/// 官方原文「通过 POST 表单来调用该接口，表单媒体文件 id 为 media，需有 filename、filelength、
/// content-type 等信息」⇒ 文件分节<b>不参与签名计算</b>，故 <see cref="MediaHash"/> 必须与
/// 实际上传的字节内容一致。</para>
/// <para><b><c>media_hash</c> 是小写 MD5</b>：官方原文「根据媒体文件内容进行 MD5 计算后的值，注意小写」
/// —— 与 <c>sign</c> 的大写十六进制口径<b>相反</b>，写成大写即 <c>PARAM_ERROR｜图片文件错误</c>。</para>
/// <para><b>格式与体积</b>：仅 <c>jpeg</c>／<c>jpg</c>／<c>bmp</c>／<c>png</c>，图片<b>不超过 2M</b>。</para>
/// <para><b><c>sign_type</c> 在本支是选填</b>：官方「HMAC-SHA256加密方式，其他或者不填为MD5方式」
/// ⇒ 缺省即 MD5，与同域 <see cref="V2RecommendConfigAddRequest"/>（必填且仅 HMAC-SHA256）不同。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2UploadMediaRequest
{
    /// <summary>微信支付分配的商户号。</summary>
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>媒体文件内容的 MD5 值，<b>小写十六进制</b>（官方特别标注「注意小写」）。</summary>
    [JsonPropertyName("media_hash")] public string? MediaHash { get; set; }

    /// <summary>签名方式：填 <c>HMAC-SHA256</c> 走 HMAC，留空或其它值官方按 MD5 处理。</summary>
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }

    /// <summary>签名（由证书收发器按字段表计算，调用方无需赋值）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 商户图片上传应答：<b>请求是 multipart，应答仍是 XML</b>。
/// </summary>
/// <remarks>
/// <para><b>返回的 <c>media_id</c> 是后续进件的凭证</b>：机构子商户的
/// <c>registration_certificate_copy</c> 与 <c>store_photos</c> 都取该值（列表形态由调用方拼 JSON 数组）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2UploadMediaResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }

    /// <summary>返回信息，非空时为错误原因（不支持 GET、XML 格式错误、验签错误等）。</summary>
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }

    /// <summary>业务结果：SUCCESS / FAIL。</summary>
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>微信返回的媒体文件标识 Id，用于进件字段引用。</summary>
    [JsonPropertyName("media_id")] public string? MediaId { get; set; }

    /// <summary>微信返回的签名（官方必填，须验签）。</summary>
    [JsonPropertyName("sign")] public string? Sign { get; set; }

    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>结果信息描述（官方本页字段名与同族另三支同为 <c>err_code_des</c>）。</summary>
    [JsonPropertyName("err_code_des")] public string? ErrCodeDes { get; set; }
}

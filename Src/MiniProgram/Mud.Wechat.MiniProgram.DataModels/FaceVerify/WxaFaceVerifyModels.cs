// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.MiniProgram.DataModels.FaceVerify;

/// <summary>获取用户人脸核身会话唯一标识请求体（<c>POST /cityservice/face/identify/getverifyid</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>face/api_getverifyid.html</c>。</para>
/// <para>
/// 业务方后台根据「用户实名信息（姓名 + 证件号）」调用本接口获取会话唯一标识
/// <c>verify_id</c>，再交给小程序前端拉起人脸核身；核身通过后经
/// <c>queryverifyinfo</c> 查询真实结果。
/// </para>
/// <para><b>敏感信息警示（MP-X7）</b>：<c>cert_info</c> 含<b>证件姓名与证件号码</b>等个人敏感信息，
/// 禁止写入日志、遥测或异常消息；调用完成即应丢弃。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "FaceVerify")]
public class WxaFaceGetVerifyIdRequest
{
    /// <summary>业务方系统内部流水号（<c>out_seq_no</c>，必填；业务方幂等标识）。</summary>
    [JsonPropertyName("out_seq_no")]
    public string? OutSeqNo { get; set; }

    /// <summary>用户的 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户身份信息（<c>cert_info</c>，必填），见 <see cref="WxaFaceCertInfo"/>。</summary>
    [JsonPropertyName("cert_info")]
    public WxaFaceCertInfo? CertInfo { get; set; }
}

/// <summary>人脸核身用户身份信息（<c>cert_info</c>）。</summary>
/// <remarks><b>个人敏感信息</b>：证件姓名/号码禁止写入日志、遥测或异常消息。</remarks>
[HttpJsonSerializable(SerializerClassName = "FaceVerify")]
public class WxaFaceCertInfo
{
    /// <summary>证件类型（<c>cert_type</c>，如 <c>1</c> 身份证；取值以官方页面为准）。</summary>
    [JsonPropertyName("cert_type")]
    public string? CertType { get; set; }

    /// <summary>证件姓名（<c>cert_name</c>；敏感信息，禁止落日志）。</summary>
    [JsonPropertyName("cert_name")]
    public string? CertName { get; set; }

    /// <summary>证件号码（<c>cert_no</c>；敏感信息，禁止落日志）。</summary>
    [JsonPropertyName("cert_no")]
    public string? CertNumber { get; set; }
}

/// <summary>获取用户人脸核身会话唯一标识应答（<c>verify_id</c> / <c>expires_in</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "FaceVerify")]
public class WxaFaceGetVerifyIdResponse : WxaResponse
{
    /// <summary>人脸核身会话唯一标识（<c>verify_id</c>；交小程序前端拉起核身，并提供给 <c>queryverifyinfo</c>）。</summary>
    [JsonPropertyName("verify_id")]
    public string? VerifyId { get; set; }

    /// <summary>会话唯一标识有效期（<c>expires_in</c>，单位：秒）。</summary>
    [JsonPropertyName("expires_in")]
    public long? ExpiresIn { get; set; }
}

/// <summary>查询用户人脸核身真实验证结果请求体（<c>POST /cityservice/face/identify/queryverifyinfo</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>face/api_queryverifyinfo.html</c>。</para>
/// <para>
/// 业务方后台根据会话唯一标识 <c>verify_id</c> 查询人脸核身<b>真实</b>验证结果；
/// <c>cert_hash</c> 为证件信息的 SHA-256 摘要（经 <see cref="SetCertHash"/> 计算），
/// 用于核对核身对象与发起对象一致。<b>核身通过须同时满足 <c>errcode == 0</c> 且 <c>verify_ret == 10000</c></b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "FaceVerify")]
public class WxaFaceQueryVerifyInfoRequest
{
    /// <summary>业务方系统内部流水号（<c>out_seq_no</c>，必填；须与 <c>getverifyid</c> 一致）。</summary>
    [JsonPropertyName("out_seq_no")]
    public string? OutSeqNo { get; set; }

    /// <summary>人脸核身会话唯一标识（<c>verify_id</c>，必填）。</summary>
    [JsonPropertyName("verify_id")]
    public string? VerifyId { get; set; }

    /// <summary>用户的 OpenId（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>证件信息摘要（<c>cert_hash</c>，必填；经 <see cref="SetCertHash"/> 计算）。</summary>
    [JsonPropertyName("cert_hash")]
    public string? CertHash { get; set; }

    /// <summary>
    /// 按官方规则计算证件信息摘要（<c>cert_hash</c>）：将证件类型/姓名/号码分别 UTF-8 Base64 编码后
    /// 拼接为 <c>cert_type=…&amp;cert_name=…&amp;cert_no=…</c>，再取 SHA-256 小写十六进制。
    /// </summary>
    /// <param name="certType">证件类型，见 <see cref="WxaFaceCertInfo.CertType"/>。</param>
    /// <param name="certName">证件姓名，见 <see cref="WxaFaceCertInfo.CertName"/>。</param>
    /// <param name="certNumber">证件号码，见 <see cref="WxaFaceCertInfo.CertNumber"/>。</param>
    /// <remarks>本方法只做内存内哈希，不记录任何明文；三个参数均不可为空（空白串即抛 <see cref="ArgumentException"/>）。</remarks>
    /// <exception cref="ArgumentNullException"><paramref name="certType"/> / <paramref name="certName"/> / <paramref name="certNumber"/> 为 <see langword="null"/>。</exception>
    public void SetCertHash(string certType, string certName, string certNumber)
    {
        if (certType == null) throw new ArgumentNullException(nameof(certType));
        if (certName == null) throw new ArgumentNullException(nameof(certName));
        if (certNumber == null) throw new ArgumentNullException(nameof(certNumber));

        string encodedCertType = Convert.ToBase64String(Encoding.UTF8.GetBytes(certType));
        string encodedCertName = Convert.ToBase64String(Encoding.UTF8.GetBytes(certName));
        string encodedCertNumber = Convert.ToBase64String(Encoding.UTF8.GetBytes(certNumber));
        string certInfoStr = $"cert_type={encodedCertType}&cert_name={encodedCertName}&cert_no={encodedCertNumber}";

        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(certInfoStr));
        CertHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();
    }
}

/// <summary>查询用户人脸核身真实验证结果应答（<c>verify_ret</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "FaceVerify")]
public class WxaFaceQueryVerifyInfoResponse : WxaResponse
{
    /// <summary>人脸核身验证结果码（<c>verify_ret</c>）：<c>10000</c> 核身通过（且须 <c>errcode == 0</c>）。</summary>
    [JsonPropertyName("verify_ret")]
    public long? VerifyRet { get; set; }
}
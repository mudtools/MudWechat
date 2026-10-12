// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.MiniProgram.DataModels.FaceVerify;

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

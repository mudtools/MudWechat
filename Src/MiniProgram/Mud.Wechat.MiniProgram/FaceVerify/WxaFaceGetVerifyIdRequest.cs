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

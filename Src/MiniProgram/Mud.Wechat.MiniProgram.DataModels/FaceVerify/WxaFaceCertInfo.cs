// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.MiniProgram.DataModels.FaceVerify;

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

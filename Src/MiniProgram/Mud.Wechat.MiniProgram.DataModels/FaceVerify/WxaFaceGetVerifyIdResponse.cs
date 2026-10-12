// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.MiniProgram.DataModels.FaceVerify;

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

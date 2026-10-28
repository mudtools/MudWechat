// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.MiniProgram.DataModels.FaceVerify;

/// <summary>查询用户人脸核身真实验证结果应答（<c>verify_ret</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "FaceVerify")]
public class WxaFaceQueryVerifyInfoResponse : WxaResponse
{
    /// <summary>人脸核身验证结果码（<c>verify_ret</c>）：<c>10000</c> 核身通过（且须 <c>errcode == 0</c>）。</summary>
    [JsonPropertyName("verify_ret")]
    public long? VerifyRet { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 解密券码（<c>POST /card/code/decrypt</c>）请求体。
/// </summary>
/// <remarks>
/// <b>入参是 <c>encrypt_code</c> 而非 <c>code</c></b>：微信卡券在「扫码」链路上给用户端的是加密码，
/// 核销/查询前必须先解密 ⇒ 本端点是 <see cref="MpCardCodeConsumeRequest"/> 与
/// <see cref="MpCardCodeGetRequest"/> 的前置步骤（SDK 只给端点、<b>不编排</b>两步）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCodeDecryptRequest
{
    /// <summary>获取或设置加密券码（官方 <c>encrypt_code</c>，必填）。</summary>
    [JsonPropertyName("encrypt_code")]
    public string EncryptCode { get; set; } = string.Empty;
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 设置公钥请求体（<c>/cgi-bin/chatdata/set_public_key</c>）。
/// <para>
/// 授权完成后需调用本接口上传公钥（RSA-2048）；<b>设置公钥之后，消息才开始存档</b>。
/// 公钥经 openssl 生成（<c>openssl genrsa -out private_key.pem 2048</c> →
/// <c>openssl rsa -in private_key.pem -pubout -out public_key.pem</c>），
/// 换行符转义为 <c>\n</c> 后填入 <see cref="PublicKey"/>。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class SetDataZonePublicKeyRequest
{
    /// <summary>
    /// 获取或设置开发者为该企业生成的公钥（官方必填；PEM 格式 RSA-2048，换行转义为 <c>\n</c>）。
    /// <para>
    /// 用于加密每条消息的密钥：通过「获取会话记录」等接口得到 <c>encrypt_secret_key</c> 后，
    /// 以私钥执行 <c>secret_key = RSA_Decypt(base64_decode(encrypt_secret_key))</c> 解密，
    /// 再传入会话展示组件展示。
    /// </para>
    /// </summary>
    [JsonPropertyName("public_key")]
    public string? PublicKey { get; set; }

    /// <summary>
    /// 获取或设置公钥对应的版本号（官方必填）；重复调用本接口更换公钥时，要求比旧公钥版本号大。
    /// </summary>
    [JsonPropertyName("public_key_ver")]
    public long? PublicKeyVer { get; set; }
}

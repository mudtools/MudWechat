// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「获取实名信息」响应（字段均为<strong>加密后</strong>数据与算法标识，解密由调用方执行）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetRealNameResponse : MpResponse
{
    /// <summary>获取或设置密钥加密算法（官方 <c>key_encrypt_type</c>）。</summary>
    [JsonPropertyName("key_encrypt_type")]
    public string? KeyEncryptType { get; set; }

    /// <summary>获取或设置加密后的密钥（官方 <c>key</c>）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>获取或设置数据加密算法（官方 <c>data_encrypt_type</c>）。</summary>
    [JsonPropertyName("data_encrypt_type")]
    public string? DataEncryptType { get; set; }

    /// <summary>获取或设置加密后的实名数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }
}

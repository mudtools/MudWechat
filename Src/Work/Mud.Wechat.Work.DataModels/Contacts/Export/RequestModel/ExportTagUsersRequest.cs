// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Contacts.Export;

/// <summary>
/// 导出标签成员请求体（<c>/cgi-bin/export/taguser</c>）。
/// </summary>
/// <remarks>
/// <para>要求对标签有读取权限。</para>
/// <para><see cref="EncodingAesKey"/> 为敏感凭据，调用方不得记录到日志；
/// 导出的数据文件以 AES-256-CBC 加密（AESKey = Base64_Decode(encoding_aeskey + "=")），
/// 解密由调用方自行完成。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Export")]
public class ExportTagUsersRequest
{
    /// <summary>
    /// 获取或设置需要导出的标签 ID。
    /// </summary>
    [JsonPropertyName("tagid")]
    public int TagId { get; set; }

    /// <summary>
    /// 获取或设置 Base64 编码后的加密密钥（长度固定为 43；用于解密导出的数据文件；敏感，不得记录到日志）。
    /// </summary>
    [JsonPropertyName("encoding_aeskey")]
    public string EncodingAesKey { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置每块数据的人员数和部门数之和（支持范围 [10^4, 10^6]，官方默认值为 10^6；不填时不出网）。
    /// </summary>
    [JsonPropertyName("block_size")]
    public int? BlockSize { get; set; }
}

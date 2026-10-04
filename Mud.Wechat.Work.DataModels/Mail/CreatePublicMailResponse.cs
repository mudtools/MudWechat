// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 创建公共邮箱响应体（<c>/cgi-bin/exmail/publicmail/create</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class CreatePublicMailResponse : WechatWorkResponse
{
    /// <summary>获取或设置公共邮箱 ID（官方 id）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>获取或设置客户端专用密码 ID（官方 auth_code_id；仅当设置创建密码时返回，可通过删除客户端专用密码接口处理）。</summary>
    [JsonPropertyName("auth_code_id")]
    public long? AuthCodeId { get; set; }

    /// <summary>
    /// 获取或设置客户端专用密码（官方 auth_code；仅当设置创建密码时返回）。
    /// </summary>
    /// <remarks><para>官方注明：该密码仅会返回一次，请妥善存储。</para></remarks>
    [JsonPropertyName("auth_code")]
    public string? AuthCode { get; set; }
}

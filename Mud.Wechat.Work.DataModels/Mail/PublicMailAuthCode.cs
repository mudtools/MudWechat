// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 公共邮箱客户端专用密码信息（官方 auth_code_list 元素结构；
/// 客户端专用密码仅会在创建时候返回，本列表不返回密码本身）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class PublicMailAuthCode
{
    /// <summary>获取或设置客户端专用密码 ID（官方 auth_code_id）。</summary>
    [JsonPropertyName("auth_code_id")]
    public long? AuthCodeId { get; set; }

    /// <summary>获取或设置客户端专用密码创建时间戳（官方 create_time）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置最后使用时间，未使用过则返回 0（官方 last_use_time）。</summary>
    [JsonPropertyName("last_use_time")]
    public long? LastUseTime { get; set; }

    /// <summary>获取或设置客户端专用密码备注（官方 remark）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

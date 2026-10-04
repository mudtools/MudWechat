// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 创建/更新公共邮箱时的客户端专用密码创建备注信息（官方 auth_code_info 结构：remark）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class PublicMailAuthCodeInfo
{
    /// <summary>
    /// 获取或设置创建客户端专用密码的备注（官方 remark，仅当 create_auth_code=1 时有效）。
    /// <para>未设置则默认为「办公PC」，最长不超过 128 个字节，必须是 utf8 编码。</para>
    /// </summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

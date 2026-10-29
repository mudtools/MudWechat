// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 门店员工信息。
/// </summary>
/// <remarks>
/// <para><b><c>role</c> 官方未发布完整枚举</b>：仅示例取值 1 管理员 / 2 员工 ⇒ SDK 只建模 <c>int</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessRetailStaff
{
    /// <summary>员工 openid（敏感）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>角色（示例 1 管理员 / 2 员工，官方未发布完整枚举）。</summary>
    [JsonPropertyName("role")]
    public int? Role { get; set; }

    /// <summary>成为员工的时间（Unix 秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }
}

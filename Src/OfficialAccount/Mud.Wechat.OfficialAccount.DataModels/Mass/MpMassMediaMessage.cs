// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>
/// 单 media_id 载体的群发消息分支（官方 mpnews / voice / mpvideo（sendall 页）/
/// music、image（preview 页）的请求体形态均为单个 <c>media_id</c> ⇒ 共用 DTO，由守卫双向锁定）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassMediaMessage
{
    /// <summary>获取或设置用于群发的素材 media_id（官方 <c>media_id</c>）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

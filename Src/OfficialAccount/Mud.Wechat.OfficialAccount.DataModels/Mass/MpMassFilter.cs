// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>群发接收者过滤条件（官方 <c>filter</c> 对象，sendall 专用）。</summary>
/// <remarks>
/// 官方契约（逐页核验 2026-10-07）：<c>is_to_all</c> 为 true 时群发给所有用户（可不填 tag_id）；
/// false 时按 <c>tag_id</c> 发送给指定群组。官方示例中 tag_id 数字与字符串形态并存（照录）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassFilter
{
    /// <summary>获取或设置是否向全部用户发送（官方 <c>is_to_all</c>：true 全部 / false 按 tag_id）。</summary>
    [JsonPropertyName("is_to_all")]
    public bool IsToAll { get; set; }

    /// <summary>获取或设置群发到的标签 id（官方 <c>tag_id</c>；is_to_all = true 时可不填）。</summary>
    [JsonPropertyName("tag_id")]
    public int? TagId { get; set; }
}

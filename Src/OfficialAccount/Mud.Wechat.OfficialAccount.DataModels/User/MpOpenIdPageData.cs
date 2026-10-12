// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.User;

/// <summary>openid 分页数据容器（<c>user/get</c> 与 <c>tags/members/getblacklist</c> 共用同一形态）。</summary>
/// <remarks>两个端点的官方 <c>data</c> 对象字段表逐字段一致（只有 <c>openid</c> 数组），故共用容器类型。</remarks>
[HttpJsonSerializable(SerializerClassName = "User")]
public class MpOpenIdPageData
{
    /// <summary>openid 列表。</summary>
    [JsonPropertyName("openid")]
    public List<string>? OpenId { get; set; }
}

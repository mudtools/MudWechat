// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 获取关联的商家列表（<c>get_ec_list</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方表例矛盾照录（2026-10-17 逐页核验）</b>：返回参数表把 <c>list</c> 标为 <c>objarray</c>，
/// 但 6.1 / 6.2 两段应答示例均把 <c>list</c> 写成<b>单个对象</b>而非数组——SDK 按参数表建模为列表；
/// 若联调实测只认对象形态，须同批改形与守卫，不得两处各写一份（UNVERIFIED 不写成断言）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsGetEcListResponse : ChannelsResponse
{
    /// <summary>获取或设置商家列表（官方 <c>list</c>，见remarks 表例矛盾）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsEcRelationItem>? List { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }
}

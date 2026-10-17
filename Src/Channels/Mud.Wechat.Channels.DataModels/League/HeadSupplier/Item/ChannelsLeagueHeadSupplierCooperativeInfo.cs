// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>机构商品合作信息（官方 <c>item.cooperative_info</c> 对象）。</summary>
/// <remarks>官方变更日志 2026-02-09：更新 <c>link</c> 字段描述——仅「商家指定达人佣金」类型（<c>commission_type = 0</c>）且机构商品状态上架中才返回。</remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierCooperativeInfo
{
    /// <summary>获取或设置商品合作状态（官方 <c>cooperative_status</c>：1 合作中）。</summary>
    [JsonPropertyName("cooperative_status")]
    public int? CooperativeStatus { get; set; }

    /// <summary>获取或设置商品是否在商品列表页对外隐藏（官方 <c>is_hidden</c>）。</summary>
    [JsonPropertyName("is_hidden")]
    public bool? IsHidden { get; set; }

    /// <summary>获取或设置带货链接（官方 <c>link</c>）。</summary>
    [JsonPropertyName("link")]
    public string? Link { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取机构配置达人佣金计划列表请求（官方 <c>headsupplier/subitem/list/get</c> 请求体）。
/// 官方把 <c>next_key</c> 与 <c>need_qr_code</c> 都标「必填 = 是」，实为「第一页传空」与「不确定则不填」⇒ 建模为可空。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetSubItemListRequest
{
    /// <summary>获取或设置机构合作计划 id（官方 <c>cooperative_item_id</c>，必填）。</summary>
    [JsonPropertyName("cooperative_item_id")]
    public long CooperativeItemId { get; set; }

    /// <summary>获取或设置单页计划数（官方 <c>page_size</c>，必填，最大 20）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页参数（官方 <c>next_key</c>，第一页为空）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置是否需要推广二维码（官方 <c>need_qr_code</c>，2026-03-09 新增；不确定时不填，填了会增加耗时）。</summary>
    [JsonPropertyName("need_qr_code")]
    public bool? NeedQrCode { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>批量新增联盟机构推广（<c>item/headsupplier/batchadd</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsBatchAddHeadSupplierItemRequest
{
    /// <summary>
    /// 获取或设置机构开放唯一凭证（官方 <c>headsupplier_appid</c>，必填）——即账号 ID，
    /// 机构可在「微信小店·联盟带货机构管理平台 - 设置 - 开放信息」中获得
    /// （需要已完成机构开通，且账号没有异常状态）。
    /// </summary>
    [JsonPropertyName("headsupplier_appid")]
    public string HeadSupplierAppId { get; set; } = string.Empty;

    /// <summary>获取或设置商品列表（官方 <c>list</c>，必填；<c>product_id</c> 不可重复且数量不超过 20）。</summary>
    [JsonPropertyName("list")]
    public List<ChannelsLeagueHeadSupplierProduct> List { get; set; } = new();

    /// <summary>获取或设置推广开始时间戳（官方 <c>begin_time</c>，选填）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置推广结束时间戳（官方 <c>end_time</c>，选填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置是否永久推广（官方 <c>is_forerver</c>，选填；官方历史拼写照录）。</summary>
    [JsonPropertyName("is_forerver")]
    public bool? IsForerver { get; set; }
}

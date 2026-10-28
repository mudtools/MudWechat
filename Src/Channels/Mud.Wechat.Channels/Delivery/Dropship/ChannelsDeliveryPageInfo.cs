// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>分页信息（官方 <c>page_info</c> 请求对象与 <c>next_page</c> 响应对象）。</summary>
/// <remarks>
/// <b>共用 DTO 裁决</b>：<c>dropship/list</c>、<c>dropship/search</c>、<c>supplyorder/list</c>、
/// <c>supplyorder/search</c> 四端点的请求 <c>page_info</c> 与响应 <c>next_page</c> 字段集完全一致
/// （<c>offset</c>/<c>limit</c>/<c>next_key</c>）⇒ 共用本 DTO；
/// <c>next_key</c> 代替 <c>offset</c> 分页性能更优（首页传空，后续用响应 <c>next_page</c> 回填）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsDeliveryPageInfo
{
    /// <summary>获取或设置游标（官方 <c>offset</c>，选填，从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置分页大小（官方 <c>limit</c>，必填，&lt;= 100）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// 获取或设置分页标识（官方 <c>next_key</c>，选填）——代替 <c>offset</c>，分页性能更优
    /// （首页传空，后续用响应 <c>next_page</c> 的 <c>next_key</c> 回填）。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Funds;

/// <summary>
/// 实时消耗条目（官方 <c>realtime_cost/get</c> 的 <c>data.list[]</c> 元素，2026-10-17 逐表核验，2 字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsRealtimeCostInfo
{
    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>；原文「当获取客户纬度报表时，该值为 0，无意义」——
    /// <c>level=ADVERTISER</c> 时返回 <c>0</c> 是官方形态，不是数据缺失）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>实时消耗（官方 <c>cost</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("cost")]
    public long? Cost { get; set; }
}

/// <summary><c>realtime_cost/get</c> 的 <c>data</c> 载荷（列表 + 分页）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsRealtimeCostListData
{
    /// <summary>实时消耗列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsRealtimeCostInfo>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary><c>GET /v3.0/realtime_cost/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class AdsRealtimeCostGetResponse : AdsResponse<AdsRealtimeCostListData>
{
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 「拉取礼品卡订单列表」请求（官方 <c>giftcard/order/batchget</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardGiftOrderBatchGetRequest
{
    /// <summary>获取或设置查询起始时间（官方 <c>begin_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置查询结束时间（官方 <c>end_time</c>，秒级 Unix 时间戳，可选）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置排序方式（官方 <c>sort_type</c>；ASC / DESC）。</summary>
    [JsonPropertyName("sort_type")]
    public string SortType { get; set; } = "ASC";

    /// <summary>获取或设置起始偏移（官方 <c>offset</c>，必填）。</summary>
    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    /// <summary>获取或设置拉取数量（官方 <c>count</c>，必填；官方默认 10）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; } = 10;
}

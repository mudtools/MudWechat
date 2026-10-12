// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

/// <summary>
/// 「批量查询子商户」请求（官方 <c>submerchant/batchget</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardSubMerchantBatchGetRequest
{
    /// <summary>获取或设置起始 ID（官方 <c>begin_id</c>；首个查询传 0 或不传）。</summary>
    [JsonPropertyName("begin_id")]
    public string BeginId { get; set; } = "0";

    /// <summary>获取或设置拉取数量（官方 <c>limit</c>；官方默认 10，最大 50）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; } = 10;

    /// <summary>获取或设置审核状态过滤（官方 <c>status</c>，可选；不传返回全部）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

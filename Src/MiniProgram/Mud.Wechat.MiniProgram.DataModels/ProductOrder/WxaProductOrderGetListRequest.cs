// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>
/// 获取订单列表请求体（<c>POST /product/order/get_list</c>）。
/// </summary>
/// <remarks>
/// <para><b>时间窗口二选一</b>：官方原文「<c>start_create_time</c>&amp;<c>end_create_time</c> 和</para>
/// <para><c>start_update_time</c>&amp;<c>end_update_time</c> 至少填一对」；格式 <c>yyyy-MM-dd HH:mm:ss</c>、不可早于 1970-01-01。</para>
/// <para><b>翻页上限差异</b>：本页 <c>page_size</c> 不超过 10,000，搜索页不超过 1,000 ⇒ 两页不得共用同一上限口径。</para>
/// <para><b>扫描过多</b>：错误码 <c>-31042</c> / <c>-31043</c> 提示缩短时间范围，宿主应分段拉取。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductOrder")]
public class WxaProductOrderGetListRequest
{
    /// <summary>创建时间起始，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("start_create_time")]
    public string? StartCreateTime { get; set; }

    /// <summary>创建时间结束，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("end_create_time")]
    public string? EndCreateTime { get; set; }

    /// <summary>更新时间起始，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("start_update_time")]
    public string? StartUpdateTime { get; set; }

    /// <summary>更新时间结束，<c>yyyy-MM-dd HH:mm:ss</c>。</summary>
    [JsonPropertyName("end_update_time")]
    public string? EndUpdateTime { get; set; }

    /// <summary>订单状态（必填），见 <see cref="WxaProductOrderStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>第几页，最小 1（必填）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>每页数量，不超过 10,000（必填）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

    /// <summary>订单来源：1 小商店 / 2 CPS 带货（选填，默认 1）。</summary>
    [JsonPropertyName("source")]
    public long? Source { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCoupon;

/// <summary>
/// 更新限时抢购任务状态请求体（<c>POST /product/limiteddiscount/update_status</c>）。
/// </summary>
/// <remarks>
/// <para><b>只支持终结操作</b>：官方仅接受 1 结束 / 9 删除，修改其它值返回 110002。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ProductCoupon")]
public class WxaProductLimitedDiscountUpdateStatusRequest
{
    /// <summary>抢购任务 ID（必填）。</summary>
    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }

    /// <summary>目标状态，见 <see cref="WxaProductLimitedDiscountStatuses"/>。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }
}

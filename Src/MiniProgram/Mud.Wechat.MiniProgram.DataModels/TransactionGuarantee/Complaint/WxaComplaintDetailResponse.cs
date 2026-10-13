// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>查询投诉单详情应答（<c>GET /wxaapi/minishop/complaintOrderDetail</c>，官方「查询投诉单详情」）。</summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/transaction-guarantee/complaint/api_getorderdetail.html"/>。
/// </para>
/// <para>
/// <b>官方字段名照录 <c>item</c>（单数）</b>：官方参数表把「投诉进度」建模为<b>单个对象</b> <c>item</c>，
/// 而非进度数组；本类型不得改成列表（若要完整进度轨迹，官方侧无该返回形态）。
/// </para>
/// <para><b>退货单信息可选</b>：<see cref="ReturnBill"/> 仅在进入退货流程时出现，其余场景缺省。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaComplaintDetailResponse : WxaResponse
{
    /// <summary>投诉单（<c>complaintOrder</c>），见 <see cref="WxaComplaintOrder"/>。</summary>
    [JsonPropertyName("complaintOrder")]
    public WxaComplaintOrder? ComplaintOrder { get; set; }

    /// <summary>投诉进度（<c>item</c>，官方为单数对象），见 <see cref="WxaComplaintProgressItem"/>。</summary>
    [JsonPropertyName("item")]
    public WxaComplaintProgressItem? Item { get; set; }

    /// <summary>退货单（<c>returnBill</c>），见 <see cref="WxaComplaintReturnBill"/>。</summary>
    [JsonPropertyName("returnBill")]
    public WxaComplaintReturnBill? ReturnBill { get; set; }
}

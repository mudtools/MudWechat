// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>同城配送余额明细（<c>balance_detail</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressIntracityBalanceDetail
{
    /// <summary>余额（<c>balance</c>；单位：分）。</summary>
    [JsonPropertyName("balance")]
    public int Balance { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>运力名称（<c>service_trans_name</c>）。</summary>
    [JsonPropertyName("service_trans_name")]
    public string? ServiceTransName { get; set; }

    /// <summary>充值订单列表（<c>order_list</c>），见 <see cref="WxaExpressIntracityChargeOrder"/>。</summary>
    [JsonPropertyName("order_list")]
    public List<WxaExpressIntracityChargeOrder>? OrderList { get; set; }
}

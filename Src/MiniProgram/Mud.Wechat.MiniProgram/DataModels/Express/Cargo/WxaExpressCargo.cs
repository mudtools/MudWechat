// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>包裹信息（<c>cargo</c>，将传递给快递公司）。</summary>
/// <remarks>官方字段以商家寄件 <c>order/add</c> 的「Body.cargo Object Payload」为准。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressCargo
{
    /// <summary>包裹数量（<c>count</c>，必填）。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>包裹长度（<c>space_x</c>，单位：厘米）。</summary>
    [JsonPropertyName("space_x")]
    public double? Length { get; set; }

    /// <summary>包裹宽度（<c>space_y</c>，单位：厘米）。</summary>
    [JsonPropertyName("space_y")]
    public double? Width { get; set; }

    /// <summary>包裹高度（<c>space_z</c>，单位：厘米）。</summary>
    [JsonPropertyName("space_z")]
    public double? Height { get; set; }

    /// <summary>包裹重量（<c>weight</c>，单位：千克，精确到小数点后 2 位）。</summary>
    [JsonPropertyName("weight")]
    public double? Weight { get; set; }

    /// <summary>包裹中商品详情列表（<c>detail_list</c>）。</summary>
    [JsonPropertyName("detail_list")]
    public List<WxaExpressCargoGood>? GoodsList { get; set; }
}

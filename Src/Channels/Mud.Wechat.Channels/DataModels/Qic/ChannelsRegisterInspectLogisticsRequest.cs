// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Qic;

/// <summary>自寄快递送检（<c>registerlogistics</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；最多可修改三次自寄快递信息。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsRegisterInspectLogisticsRequest
{
    /// <summary>获取或设置订单号列表（官方 <c>order_id_list</c>，必填）。</summary>
    [JsonPropertyName("order_id_list")]
    public List<string> OrderIdList { get; set; } = new();

    /// <summary>获取或设置快递物流信息（官方 <c>logistics_info</c>，必填）。</summary>
    [JsonPropertyName("logistics_info")]
    public ChannelsInspectLogisticsInfo LogisticsInfo { get; set; } = new();
}

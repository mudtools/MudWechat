// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>面单模板信息选项（官方 <c>template_info.options</c>，总数必须为 8 个，顺序按数组序）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTemplateOption
{
    /// <summary>获取或设置信息类型（官方 <c>option_id</c>，填 0-7）：0 商品总数量；1 商品名称+规格+编码+数量；2 商品名称+规格+数量；3 商品名称+数量；4 店铺名称；5 订单号；6 买家留言；7 卖家备注。</summary>
    [JsonPropertyName("option_id")]
    public int OptionId { get; set; }

    /// <summary>获取或设置打印字号（官方 <c>font_size</c>，0 默认大小，1 加大）。</summary>
    [JsonPropertyName("font_size")]
    public int FontSize { get; set; }

    /// <summary>获取或设置字体是否加粗（官方 <c>is_bold</c>）。</summary>
    [JsonPropertyName("is_bold")]
    public bool? IsBold { get; set; }

    /// <summary>获取或设置是否选用此信息（官方 <c>is_open</c>）。</summary>
    [JsonPropertyName("is_open")]
    public bool? IsOpen { get; set; }
}

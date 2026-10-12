// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 运费信息（官方 <c>express_info</c> 对象：<c>template_id</c> / <c>weight</c> / <c>express_type</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductExpressInfo
{
    /// <summary>
    /// 获取或设置运费模板 ID（官方 <c>template_id</c>，先从「获取运费模板列表」拿到；
    /// <c>deliver_method</c> 为 1 或 3 时不用填写）。官方类型为 string，官方示例为数字 —— 建模为 string 兼容两种形态。
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>获取或设置商品重量（官方 <c>weight</c>，单位：克；运费模版计价方式为「按重量」时必填）。</summary>
    [JsonPropertyName("weight")]
    public long? Weight { get; set; }

    /// <summary>获取或设置是否开启「偏远地区中转集运」（官方 <c>express_type</c>，见 <see cref="ChannelsProductExpressTypes"/>）。</summary>
    [JsonPropertyName("express_type")]
    public int? ExpressType { get; set; }
}

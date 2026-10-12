// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>更新门店内容（<c>content</c>；仅更新传入字段）。</summary>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressUpdateStoreContent
{
    /// <summary>门店名称（<c>store_name</c>，可选）。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>运力偏好（<c>order_pattern</c>，可选）。</summary>
    [JsonPropertyName("order_pattern")]
    public int? OrderPattern { get; set; }

    /// <summary>优先使用的运力 ID（<c>service_trans_prefer</c>，可选；<c>order_pattern=2</c> 时必填）。</summary>
    [JsonPropertyName("service_trans_prefer")]
    public string? ServiceTransIdPrefer { get; set; }

    /// <summary>门店地址信息（<c>address_info</c>，可选），见 <see cref="WxaExpressIntracityAddress"/>。</summary>
    [JsonPropertyName("address_info")]
    public WxaExpressIntracityAddress? AddressInfo { get; set; }
}

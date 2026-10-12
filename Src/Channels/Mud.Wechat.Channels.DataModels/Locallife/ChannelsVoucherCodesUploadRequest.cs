// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Locallife;

/// <summary>上传团购优惠 code（<c>uploadvouchercode</c>）请求体。</summary>
/// <remarks>官方契约：仅「商家预存 code 模式」（<c>code_source_type=2</c>）需要；
/// 一次最多 200 个 code，单个 code 不超过 48 位，仅支持数字和字母。</remarks>
[HttpJsonSerializable(SerializerClassName = "Locallife")]
public class ChannelsVoucherCodesUploadRequest
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，必填；通过本地生活 api 或页面创建的商品 id）。</summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>获取或设置 sku id（官方 <c>sku_id</c>，必填；创建商品时返回的 sku_id）。</summary>
    [JsonPropertyName("sku_id")]
    public string SkuId { get; set; } = string.Empty;

    /// <summary>获取或设置商户上传的券 code 列表（官方 <c>codes</c>，必填；一次最多 200 个）。</summary>
    [JsonPropertyName("codes")]
    public List<string> Codes { get; set; } = new();
}

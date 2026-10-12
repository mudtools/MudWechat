// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Qic;

/// <summary>
/// 质检码详情（官方 <c>data</c> 对象）。
/// </summary>
/// <remarks><b>官方本对象字段为 camelCase 命名</b>（如 backupDeliveryId、boxDTOList），与小店其余端点 snake_case 不同，字段名照抄官方。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectCodeDetail
{
    /// <summary>获取或设置备用发货快递编码（官方 <c>backupDeliveryId</c>）。</summary>
    [JsonPropertyName("backupDeliveryId")]
    public string? BackupDeliveryId { get; set; }

    /// <summary>获取或设置备用发货快递名称（官方 <c>backupDeliveryName</c>，官方说明原文「用发货快递名称」照录）。</summary>
    [JsonPropertyName("backupDeliveryName")]
    public string? BackupDeliveryName { get; set; }

    /// <summary>获取或设置包装盒列表（官方 <c>boxDTOList</c>）。</summary>
    [JsonPropertyName("boxDTOList")]
    public List<ChannelsInspectBox>? BoxDtoList { get; set; }

    /// <summary>获取或设置视频号店铺 appid（官方 <c>channelAppId</c>）。</summary>
    [JsonPropertyName("channelAppId")]
    public string? ChannelAppId { get; set; }

    /// <summary>获取或设置发货快递公司编码（官方 <c>deliveryId</c>，同视频号小店电子面单支持快递公司编码）。</summary>
    [JsonPropertyName("deliveryId")]
    public string? DeliveryId { get; set; }

    /// <summary>获取或设置发货快递公司名称（官方 <c>deliveryName</c>）。</summary>
    [JsonPropertyName("deliveryName")]
    public string? DeliveryName { get; set; }

    /// <summary>获取或设置商品镶嵌材质（官方 <c>embedGoodsMaterial</c>）。</summary>
    [JsonPropertyName("embedGoodsMaterial")]
    public string? EmbedGoodsMaterial { get; set; }

    /// <summary>获取或设置商品描述（官方 <c>goodsDesc</c>）。</summary>
    [JsonPropertyName("goodsDesc")]
    public string? GoodsDesc { get; set; }

    /// <summary>获取或设置是否合并发货（官方 <c>expressMerge</c>，true：合并，false：不合并）。</summary>
    [JsonPropertyName("expressMerge")]
    public bool? ExpressMerge { get; set; }

    /// <summary>获取或设置商品主材质（官方 <c>goodsMainMaterial</c>）。</summary>
    [JsonPropertyName("goodsMainMaterial")]
    public string? GoodsMainMaterial { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>goodsName</c>）。</summary>
    [JsonPropertyName("goodsName")]
    public string? GoodsName { get; set; }

    /// <summary>获取或设置商品件数（官方 <c>goodsNum</c>）。</summary>
    [JsonPropertyName("goodsNum")]
    public int? GoodsNum { get; set; }

    /// <summary>获取或设置商品配件材质（官方 <c>goodsPartsMaterial</c>）。</summary>
    [JsonPropertyName("goodsPartsMaterial")]
    public string? GoodsPartsMaterial { get; set; }

    /// <summary>获取或设置质检仓 id（官方 <c>inspectBaseId</c>）。</summary>
    [JsonPropertyName("inspectBaseId")]
    public string? InspectBaseId { get; set; }

    /// <summary>获取或设置质检仓名称（官方 <c>inspectBaseName</c>）。</summary>
    [JsonPropertyName("inspectBaseName")]
    public string? InspectBaseName { get; set; }

    /// <summary>获取或设置质检码（官方 <c>inspectCode</c>）。</summary>
    [JsonPropertyName("inspectCode")]
    public string? InspectCode { get; set; }

    /// <summary>获取或设置质检机构 id（官方 <c>inspectOrgId</c>）。</summary>
    [JsonPropertyName("inspectOrgId")]
    public string? InspectOrgId { get; set; }

    /// <summary>获取或设置质检机构名称（官方 <c>inspectOrgName</c>）。</summary>
    [JsonPropertyName("inspectOrgName")]
    public string? InspectOrgName { get; set; }

    /// <summary>获取或设置质检机构名称简称（官方 <c>inspectOrgShortName</c>）。</summary>
    [JsonPropertyName("inspectOrgShortName")]
    public string? InspectOrgShortName { get; set; }

    /// <summary>获取或设置商家名称（官方 <c>merchantName</c>）。</summary>
    [JsonPropertyName("merchantName")]
    public string? MerchantName { get; set; }

    /// <summary>获取或设置订单号（官方 <c>orderId</c>）。</summary>
    [JsonPropertyName("orderId")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置是否加急订单（官方 <c>urgentOrder</c>，true-加急，false-非加急）。</summary>
    [JsonPropertyName("urgentOrder")]
    public bool? UrgentOrder { get; set; }

    /// <summary>
    /// 获取或设置打印模板数据和模板 html 地址（官方 <c>printInfo</c>，base64 编码）——
    /// 官方质检面单打印新方案：只需把本字段传入打印控件即可进行打印。
    /// </summary>
    [JsonPropertyName("printInfo")]
    public string? PrintInfo { get; set; }

    /// <summary>获取或设置是否需要打标（官方 <c>needLabel</c>，官方变更日志 2026-03-24 新增；true：需要打标）。</summary>
    [JsonPropertyName("needLabel")]
    public bool? NeedLabel { get; set; }
}

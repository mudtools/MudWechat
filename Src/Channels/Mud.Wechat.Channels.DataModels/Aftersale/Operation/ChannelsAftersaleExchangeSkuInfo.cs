// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「售后操作类」DTO（aftersale/acceptapply、aftersale/rejectapply、
// aftersale/acceptexchangereship、aftersale/rejectexchangereship、aftersale/merchantreship、
// aftersale/merchantupdatereshipexpress、aftersale/merchantupdateaftersale、aftersale/genaftersaleorder、
// aftersale/refundpricediff、aftersale/getexchangeableskulist、aftersale/applyvirtualtelnum、
// aftersale/handlefastexchangereceipt、aftersale/uploadrefundcertificate）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>换货信息（官方 <c>genaftersaleorder.exchange_sku_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Aftersale")]
public class ChannelsAftersaleExchangeSkuInfo
{
    /// <summary>
    /// 获取或设置要换成的新 SKU ID（官方 <c>new_sku_id</c>，必须来自 <c>getexchangeableskulist</c> 返回的可换 SKU 列表）。
    /// </summary>
    [JsonPropertyName("new_sku_id")]
    public string NewSkuId { get; set; } = string.Empty;
}

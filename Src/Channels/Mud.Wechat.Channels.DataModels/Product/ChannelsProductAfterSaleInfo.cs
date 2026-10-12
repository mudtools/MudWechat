// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 售后 / 退货地址（官方 <c>after_sale_info</c> 对象：<c>after_sale_address_id</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAfterSaleInfo
{
    /// <summary>
    /// 获取或设置售后 / 退货地址 id（官方 <c>after_sale_address_id</c>），
    /// 使用地址管理相关接口（物流发货域 <c>merchant/address/*</c>）进行添加或获取。
    /// </summary>
    [JsonPropertyName("after_sale_address_id")]
    public long? AfterSaleAddressId { get; set; }
}

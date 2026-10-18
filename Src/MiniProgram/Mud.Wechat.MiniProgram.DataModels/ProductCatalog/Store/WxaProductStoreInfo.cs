// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

/// <summary>
/// 店铺信息（<c>data</c> 对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ProductCatalog")]
public class WxaProductStoreInfo
{
    /// <summary>store_name（官方类型 <c>string</c>）：商店名称。</summary>
    [JsonPropertyName("store_name")]
    public string? StoreName { get; set; }

    /// <summary>logo（官方类型 <c>string</c>）：商店头像。</summary>
    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    /// <summary>type（官方类型 <c>number</c>）：<c>1</c>=企业店、<c>2</c>=个人店。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }
}

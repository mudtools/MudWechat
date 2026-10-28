// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.HomePage;

/// <summary>主页展示商品信息（官方 <c>products</c> 数组元素，<c>getstorewindowproductlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "HomePage")]
public class ChannelsStoreWindowProduct
{
    /// <summary>获取或设置商品 ID（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>获取或设置是否隐藏（官方 <c>is_set_hide</c>；1=隐藏，0=取消隐藏。隐藏的商品只在首页不可见，并不代表下架）。</summary>
    [JsonPropertyName("is_set_hide")]
    public int? IsSetHide { get; set; }

    /// <summary>获取或设置是否置顶（官方 <c>is_set_top</c>；1=置顶，0=取消置顶）。</summary>
    [JsonPropertyName("is_set_top")]
    public int? IsSetTop { get; set; }
}

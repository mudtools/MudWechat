// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>获取推客的某个商品的推广分佣比例请求（官方 <c>promoter/get_sharer_product_commission_info</c> 请求体）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterGetSharerProductCommissionInfoRequest
{
    /// <summary>获取或设置推客在微信电商平台注册的身份标识（官方 <c>sharer_appid</c>，必填）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppId { get; set; }

    /// <summary>获取或设置要查询的商品 id（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }
}

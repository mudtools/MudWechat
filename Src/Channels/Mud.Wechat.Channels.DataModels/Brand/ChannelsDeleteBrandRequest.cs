// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>删除品牌资质（<c>deletebrandlogic</c>）请求体。</summary>
/// <remarks>官方业务限制：删除品牌资质后，对应品牌关联的商品将会被下架。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsDeleteBrandRequest
{
    /// <summary>获取或设置品牌库中的品牌编号（官方 <c>brand_id</c>，必填，官方示例 "10000000"）。</summary>
    [JsonPropertyName("brand_id")]
    public string BrandId { get; set; } = string.Empty;
}

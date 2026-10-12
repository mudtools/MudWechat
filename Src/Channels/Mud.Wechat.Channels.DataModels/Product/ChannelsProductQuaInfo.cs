// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品资质（官方 <c>product_qua_infos</c> 数组元素：<c>qua_id</c> + <c>qua_url</c>）。
/// </summary>
/// <remarks>取代已废弃的 <c>qualifications</c> 字段；不同类目下必填的资质要求不同（参考「获取类目信息」的 <c>product_qua_list[]</c>）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductQuaInfo
{
    /// <summary>获取或设置商品资质 id（官方 <c>qua_id</c>，对应 <c>product_qua_list[].qua_id</c>）。</summary>
    [JsonPropertyName("qua_id")]
    public string? QuaId { get; set; }

    /// <summary>获取或设置商品资质图片列表（官方 <c>qua_url</c>，单个资质 id 下最多 10 张）。</summary>
    [JsonPropertyName("qua_url")]
    public List<string>? QuaUrl { get; set; }
}

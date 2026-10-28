// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品详情文本（官方 <c>desc_info</c> 对象：<c>imgs</c> + <c>desc</c>）。
/// </summary>
/// <remarks>
/// 官方契约见「添加商品 / 更新商品」的 <c>Body.desc_info Object Payload</c> 小节：
/// <c>imgs</c> 最少 1 张、最多 50 张（食品饮料和生鲜类目商品最少 3 张），不得有重复图片；
/// 图片 url 必须经「上传图片」接口（<c>resp_type=1</c>）返回（前缀 <c>mmecimage.cn/p/</c>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductDescInfo
{
    /// <summary>获取或设置商品详情图片列表（官方 <c>imgs</c>，必填）。</summary>
    [JsonPropertyName("imgs")]
    public List<string> Imgs { get; set; } = new();

    /// <summary>获取或设置商品详情文本（官方 <c>desc</c>，选填）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }
}

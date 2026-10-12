// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品类目节点（官方 <c>cats</c> / <c>cats_v2</c> 数组元素：<c>cat_id</c>）。
/// </summary>
/// <remarks>
/// <para>旧类目树 <c>cats</c> 大小恒等于 3（一、二、三级类目）；新类目树 <c>cats_v2</c> 为多级类目，</para>
/// <para>两者均为「数组下标 0 = 一级类目」。商品上架后不可修改一级类目。</para>
/// <para>官方类型为 number，但官方代码示例同时出现数字（<c>6000</c>）与字符串（<c>"10000113"</c>）两种形态，</para>
/// <para>故建模为 string（反序列化时 number→string 由 System.Text.Json 原生支持）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductCategory
{
    /// <summary>获取或设置类目 ID（官方 <c>cat_id</c>，必填；经「获取类目接口」拿到可用的 cat_id）。</summary>
    [JsonPropertyName("cat_id")]
    public string CatId { get; set; } = string.Empty;
}

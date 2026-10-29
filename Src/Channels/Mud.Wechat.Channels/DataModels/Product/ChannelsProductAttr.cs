// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Product;

/// <summary>
/// 商品参数（官方 <c>attrs</c> 数组元素：<c>attr_key</c> + <c>attr_value</c>）。
/// </summary>
/// <remarks>
/// 部分类目有必填参数（参考「获取类目信息」的 <c>attr.product_attr_list[].is_required</c>）。
/// <c>attr_value</c> 按类目属性 type 有不同的格式约定（select_many 用 ; 分隔 / integer_unit
/// 为「数值 单位」/ integer 为字符串数字），SDK 不做二次加工，原样透传。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Product")]
public class ChannelsProductAttr
{
    /// <summary>获取或设置属性键 key（官方 <c>attr_key</c>，属性自定义用）。</summary>
    [JsonPropertyName("attr_key")]
    public string AttrKey { get; set; } = string.Empty;

    /// <summary>获取或设置属性值 value（官方 <c>attr_value</c>，属性自定义用）。</summary>
    [JsonPropertyName("attr_value")]
    public string AttrValue { get; set; } = string.Empty;
}

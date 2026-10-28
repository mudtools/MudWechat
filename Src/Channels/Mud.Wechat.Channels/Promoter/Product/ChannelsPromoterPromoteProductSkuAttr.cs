// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>sku 属性（官方 <c>skus[].sku_attrs</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterPromoteProductSkuAttr
{
    /// <summary>获取或设置属性键（官方 <c>attr_key</c>，属性自定义用）。</summary>
    [JsonPropertyName("attr_key")]
    public string? AttrKey { get; set; }

    /// <summary>获取或设置属性值（官方 <c>attr_value</c>，属性自定义用；类目返回 type 为 select_many 时用分号分隔多选项，为 integer_unit/decimal4_unit 时为「数值 单位」，为 integer/decimal4 时为字符串形式的数字）。</summary>
    [JsonPropertyName("attr_value")]
    public string? AttrValue { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 物流发货（Logistics）域「电子面单」类 DTO（logistics/ewaybill/biz/*）。命名空间恒为
// Mud.Wechat.Channels.DataModels.Logistics。

namespace Mud.Wechat.Channels.DataModels.Logistics;

/// <summary>面单标准模板项（官方 <c>config</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Logistics")]
public class ChannelsLogisticsEwaybillTemplateConfig
{
    /// <summary>获取或设置模板类型（官方 <c>type</c>，默认填 'single'）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置模板描述（官方 <c>desc</c>，默认填「一联单标准模板」）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>获取或设置面单宽度（官方 <c>width</c>，单位毫米）。</summary>
    [JsonPropertyName("width")]
    public int? Width { get; set; }

    /// <summary>获取或设置面单高度（官方 <c>height</c>，单位毫米）。</summary>
    [JsonPropertyName("height")]
    public int? Height { get; set; }

    /// <summary>获取或设置标准模板（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置自定义模板配置（官方 <c>custom_config</c>）。</summary>
    [JsonPropertyName("custom_config")]
    public ChannelsLogisticsEwaybillTemplateCustomConfig? CustomConfig { get; set; }
}

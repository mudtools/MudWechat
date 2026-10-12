// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>额外服务项（官方 <c>extra_service_list</c> 数组元素，对应添加商品 extra_service 参数）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryExtraService
{
    /// <summary>获取或设置参数名（官方 <c>extra_service_name</c>）。</summary>
    [JsonPropertyName("extra_service_name")]
    public string? ExtraServiceName { get; set; }

    /// <summary>获取或设置参数类型（官方 <c>extra_service_type</c>：<c>select_one</c>（单选）或 <c>integer</c>）。</summary>
    [JsonPropertyName("extra_service_type")]
    public string? ExtraServiceType { get; set; }

    /// <summary>获取或设置可选值（官方 <c>extra_service_value</c>）。</summary>
    [JsonPropertyName("extra_service_value")]
    public string? ExtraServiceValue { get; set; }
}

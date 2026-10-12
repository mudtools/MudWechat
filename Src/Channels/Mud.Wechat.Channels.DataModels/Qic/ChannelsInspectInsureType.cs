// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Qic;

/// <summary>保价类型（官方 <c>insure_type_list</c> 数组元素）。</summary>
/// <remarks>保价金额单位为<b>分</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Qic")]
public class ChannelsInspectInsureType
{
    /// <summary>获取或设置保价类型 id（官方 <c>id</c>，绑定送检信息 <c>delivery_insure_id</c> 的取值来源）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>获取或设置保价名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置上限类型（官方 <c>upper_limit_type</c>，见 <see cref="ChannelsInspectUpperLimitTypes"/>）。</summary>
    [JsonPropertyName("upper_limit_type")]
    public int? UpperLimitType { get; set; }

    /// <summary>获取或设置上限金额（官方 <c>upper_limit_amount</c>，单位：分）。</summary>
    [JsonPropertyName("upper_limit_amount")]
    public long? UpperLimitAmount { get; set; }
}

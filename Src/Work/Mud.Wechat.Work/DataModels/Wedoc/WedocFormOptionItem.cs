// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表问题选项项（官方 <c>option_item</c> 元素；单选/多选/下拉列表题的选项列表）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormOptionItem
{
    /// <summary>获取或设置选项 key（官方 <c>key</c>，必填），取值 1、2、3……。</summary>
    [JsonPropertyName("key")]
    public uint? Key { get; set; }

    /// <summary>获取或设置选项内容（官方 <c>value</c>，必填）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// 获取或设置选项状态（官方 <c>status</c>，必填）。
    /// 官方取值：<c>1</c> 正常、<c>2</c> 被删除。
    /// </summary>
    [JsonPropertyName("status")]
    public uint? Status { get; set; }
}

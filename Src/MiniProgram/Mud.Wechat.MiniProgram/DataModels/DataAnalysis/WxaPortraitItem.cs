// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>画像条目（<c>{id,name,value}</c>）。</summary>
/// <remarks>
/// <see cref="Id"/> 为可空：官方字段表对六类数组统一列 <c>id</c>，但示例中 <c>devices</c> 元素缺 <c>id</c>
/// （官方不一致，照录且<b>不本地补齐</b>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaPortraitItem
{
    /// <summary>属性值 id（<c>id</c>；官方示例中部分维度缺省）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>属性值名称（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>该属性值的访问 uv（<c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public long? Value { get; set; }
}

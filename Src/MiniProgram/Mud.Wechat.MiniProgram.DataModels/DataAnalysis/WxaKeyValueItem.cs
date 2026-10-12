// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.DataAnalysis;

/// <summary>
/// 键值条目（<c>{key,value}</c>）：留存数组元素与访问分布 <c>item_list</c> 元素<b>同形共用</b>。
/// </summary>
/// <remarks>
/// <b><c>key</c> 语义随宿主端点不同</b>：留存端点为留存天数（<c>0</c> 当天、<c>1</c> 一天后……
/// 取值 <c>0,1,2,3,4,5,6,7,14,30</c>）；访问分布端点为各 <c>index</c> 下的场景 id。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "DataAnalysis")]
public class WxaKeyValueItem
{
    /// <summary>标识（<c>key</c>）。</summary>
    [JsonPropertyName("key")]
    public long? Key { get; set; }

    /// <summary>数值（<c>value</c>）：留存端点为用户数，分布端点为该场景访问 pv。</summary>
    [JsonPropertyName("value")]
    public long? Value { get; set; }
}

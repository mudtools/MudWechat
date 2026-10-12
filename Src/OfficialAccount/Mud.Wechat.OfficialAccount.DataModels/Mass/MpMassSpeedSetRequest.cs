// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Mass;

/// <summary>设置群发速度（<c>message/mass/speed/set</c>）请求体。</summary>
/// <remarks>官方 speed 档位原文：「0 为 80w/分钟、1 为 60w/分钟、2 为 45w/分钟、3 为 30w/分钟、4 为 10w/分钟」。</remarks>
[HttpJsonSerializable(SerializerClassName = "Mass")]
public class MpMassSpeedSetRequest
{
    /// <summary>获取或设置群发速度级别（官方 <c>speed</c>：0~4，档位见类型 remarks；官方错误码 45083 表达越界）。</summary>
    [JsonPropertyName("speed")]
    public int Speed { get; set; }
}

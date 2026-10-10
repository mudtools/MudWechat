// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 发送的位置信息（<c>location_select</c> 报文的 <c>SendLocationInfo</c> 节点；官方 path 90240）。
/// </summary>
/// <remarks>
/// 经上游 G-ADR-17 的 <c>Object</c> 嵌套通道声明化。官方节点名含下划线（<c>Location_X</c>/<c>Location_Y</c>），
/// 坐标为小数 —— <c>Number&lt;T&gt;</c> 只解析整数，故坐标显式 <c>ParseReal</c>。节点缺失 ⇒ 返回 <c>null</c>。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackSendLocationInfo
{
    /// <summary>X 坐标（官方 <c>Location_X</c>）。</summary>
    [PayloadField("Location_X", Method = nameof(WechatPayloadConverter.ParseReal))]
    public double? LocationX { get; set; }

    /// <summary>Y 坐标（官方 <c>Location_Y</c>）。</summary>
    [PayloadField("Location_Y", Method = nameof(WechatPayloadConverter.ParseReal))]
    public double? LocationY { get; set; }

    /// <summary>精度 / 比例尺（官方 <c>Scale</c>，越精细数值越高）。</summary>
    [PayloadField("Scale")]
    public long? Scale { get; set; }

    /// <summary>地理位置的字符串信息（官方 <c>Label</c>）。</summary>
    [PayloadField("Label")]
    public string? Label { get; set; }

    /// <summary>POI 的名字，可能为空（官方 <c>Poiname</c>）。</summary>
    [PayloadField("Poiname")]
    public string? Poiname { get; set; }
}

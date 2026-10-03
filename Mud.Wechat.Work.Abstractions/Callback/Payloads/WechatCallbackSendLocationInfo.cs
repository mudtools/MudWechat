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
/// 由 <c>WechatPayloadConverter.ParseSendLocationInfo</c> 在转换器内手工组装
/// （官方节点名含下划线 <c>Location_X</c>/<c>Location_Y</c>，且坐标为小数 —— 生成器的
/// <c>Number&lt;T&gt;</c> 推断只解析整数）。节点缺失 ⇒ 返回 <c>null</c>。
/// </remarks>
public sealed class WechatCallbackSendLocationInfo
{
    /// <summary>X 坐标（官方 <c>Location_X</c>）。</summary>
    public double? LocationX { get; set; }

    /// <summary>Y 坐标（官方 <c>Location_Y</c>）。</summary>
    public double? LocationY { get; set; }

    /// <summary>精度 / 比例尺（官方 <c>Scale</c>，越精细数值越高）。</summary>
    public long? Scale { get; set; }

    /// <summary>地理位置的字符串信息（官方 <c>Label</c>）。</summary>
    public string? Label { get; set; }

    /// <summary>POI 的名字，可能为空（官方 <c>Poiname</c>）。</summary>
    public string? Poiname { get; set; }
}

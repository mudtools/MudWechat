// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 扫码信息（<c>scancode_push</c>/<c>scancode_waitmsg</c> 报文的 <c>ScanCodeInfo</c> 节点；官方 path 90240）。
/// </summary>
/// <remarks>
/// 由 <c>WechatPayloadConverter.ParseScanCodeInfo</c> 在转换器内手工组装（生成器仅支持「容器 → 单层同构项」，
/// 本节点是「容器 → 两个异构标量子节点」形态）。节点缺失 ⇒ 返回 <c>null</c>。
/// </remarks>
public sealed class WechatCallbackScanCodeInfo
{
    /// <summary>扫描类型（官方 <c>ScanType</c>，一般是 <c>qrcode</c>）。</summary>
    public string? ScanType { get; set; }

    /// <summary>扫描结果，即二维码对应的字符串信息（官方 <c>ScanResult</c>）。</summary>
    public string? ScanResult { get; set; }
}

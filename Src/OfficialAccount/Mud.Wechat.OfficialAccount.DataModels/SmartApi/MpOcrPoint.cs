// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// OCR / 图像处理共用的坐标点（官方四角点的 <c>x</c>/<c>y</c>）。
/// </summary>
/// <remarks>
/// 官方字段表原文：<c>x</c> = x 坐标，<c>y</c> = y 坐标；四角点（left_top / right_top /
/// right_bottom / left_bottom）字段结构相同 ⇒ 共用本 DTO。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrPoint
{
    /// <summary>获取或设置 x 坐标（官方 <c>x</c>）。</summary>
    [JsonPropertyName("x")]
    public double? X { get; set; }

    /// <summary>获取或设置 y 坐标（官方 <c>y</c>）。</summary>
    [JsonPropertyName("y")]
    public double? Y { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// OCR / 图像处理共用的矩形位置（官方四角点容器 <c>pos</c> / <c>cert_position</c>）。
/// </summary>
/// <remarks>
/// 官方字段表原文：<c>left_top</c> 左上角 / <c>right_top</c> 右上角 /
/// <c>right_bottom</c> 右下角 / <c>left_bottom</c> 左下角，四者字段结构相同。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrRect
{
    /// <summary>获取或设置左上角位置（官方 <c>left_top</c>）。</summary>
    [JsonPropertyName("left_top")]
    public MpOcrPoint? LeftTop { get; set; }

    /// <summary>获取或设置右上角位置（官方 <c>right_top</c>）。</summary>
    [JsonPropertyName("right_top")]
    public MpOcrPoint? RightTop { get; set; }

    /// <summary>获取或设置右下角位置（官方 <c>right_bottom</c>）。</summary>
    [JsonPropertyName("right_bottom")]
    public MpOcrPoint? RightBottom { get; set; }

    /// <summary>获取或设置左下角位置（官方 <c>left_bottom</c>）。</summary>
    [JsonPropertyName("left_bottom")]
    public MpOcrPoint? LeftBottom { get; set; }
}

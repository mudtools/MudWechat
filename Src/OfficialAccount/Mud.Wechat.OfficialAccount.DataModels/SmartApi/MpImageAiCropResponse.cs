// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 图片智能裁剪（<c>POST /cv/img/aicrop</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>results</c>（智能裁剪结果，条目含
/// <c>crop_left</c> / <c>crop_top</c> / <c>crop_right</c> / <c>crop_bottom</c>）/
/// <c>img_size</c>（图片大小）。
/// </para>
/// <para>
/// <b>结果顺序</b>：官方原文「<c>ratios</c> 参数为可选，如果为空，则算法自动裁剪最佳宽高比；
/// 如果提供多个宽高比，请以英文逗号「,」分隔，<b>最多支持 5 个宽高比</b>」⇒ <c>results</c>
/// 与请求的 <c>ratios</c> 顺序对应，最多 5 条。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c>（invalid image url）/ <c>101002</c>（invalid image data）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpImageAiCropResponse : MpResponse
{
    /// <summary>获取或设置智能裁剪结果列表（官方 <c>results</c>）。</summary>
    [JsonPropertyName("results")]
    public List<MpImageCropResult>? Results { get; set; }

    /// <summary>获取或设置图片尺寸（官方 <c>img_size</c>）。</summary>
    [JsonPropertyName("img_size")]
    public MpImageSize? ImageSize { get; set; }
}

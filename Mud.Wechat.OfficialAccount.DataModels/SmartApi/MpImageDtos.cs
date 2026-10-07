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

/// <summary>智能裁剪结果条目（官方 <c>results[]</c>，裁剪区域四边界）。</summary>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpImageCropResult
{
    /// <summary>获取或设置左上角 x（官方 <c>crop_left</c>）。</summary>
    [JsonPropertyName("crop_left")]
    public int? CropLeft { get; set; }

    /// <summary>获取或设置左上角 y（官方 <c>crop_top</c>）。</summary>
    [JsonPropertyName("crop_top")]
    public int? CropTop { get; set; }

    /// <summary>获取或设置右下角 x（官方 <c>crop_right</c>）。</summary>
    [JsonPropertyName("crop_right")]
    public int? CropRight { get; set; }

    /// <summary>获取或设置右下角 y（官方 <c>crop_bottom</c>）。</summary>
    [JsonPropertyName("crop_bottom")]
    public int? CropBottom { get; set; }
}

/// <summary>
/// 二维码/条码识别（<c>POST /cv/img/qrcode</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>code_results</c>（处理结果，条目含
/// <c>type_name</c> 码的类型 / <c>data</c> 码的信息 / <c>pos</c> 码的坐标）/ <c>img_size</c>。
/// </para>
/// <para>
/// <b>官方原文（位置坐标的条件返回，照录）</b>：「支持条码、二维码、DataMatrix 和 PDF417 的识别」；
/// 「<b>二维码、DataMatrix 会返回位置坐标，条码和 PDF417 暂不返回位置坐标</b>」⇒
/// <see cref="MpImageQrcodeResult.Position"/> 为可空（字段表未标条件返回，SDK 以可空承载该事实）。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101002</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpImageQrcodeResponse : MpResponse
{
    /// <summary>获取或设置处理结果列表（官方 <c>code_results</c>）。</summary>
    [JsonPropertyName("code_results")]
    public List<MpImageQrcodeResult>? CodeResults { get; set; }

    /// <summary>获取或设置图片尺寸（官方 <c>img_size</c>）。</summary>
    [JsonPropertyName("img_size")]
    public MpImageSize? ImageSize { get; set; }
}

/// <summary>二维码/条码识别结果条目（官方 <c>code_results[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpImageQrcodeResult
{
    /// <summary>获取或设置码的类型（官方 <c>type_name</c>）。</summary>
    [JsonPropertyName("type_name")]
    public string? TypeName { get; set; }

    /// <summary>获取或设置码的信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }

    /// <summary>获取或设置码的坐标（官方 <c>pos</c>；条码与 PDF417 暂不返回位置坐标）。</summary>
    [JsonPropertyName("pos")]
    public MpOcrRect? Position { get; set; }
}

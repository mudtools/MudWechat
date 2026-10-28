// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

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

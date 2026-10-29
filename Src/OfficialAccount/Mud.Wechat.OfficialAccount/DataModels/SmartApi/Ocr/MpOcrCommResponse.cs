// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 通用印刷体识别（<c>POST /cv/ocr/comm</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>items</c>（识别结果）/ <c>img_size</c>（图片大小）；
/// 条目字段表仅列 <c>pos</c>（位置信息）。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：返回示例中 <c>items[]</c> 含 <c>text</c>（识别文本），
/// 但「RES.ITEMS(ARRAY) OBJECT PAYLOAD」字段表只列了 <c>pos</c>；SDK 按超集建模保留
/// <see cref="MpOcrTextItem.Text"/>。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101002</c> / <c>101003</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrCommResponse : MpResponse
{
    /// <summary>获取或设置识别结果列表（官方 <c>items</c>）。</summary>
    [JsonPropertyName("items")]
    public List<MpOcrTextItem>? Items { get; set; }

    /// <summary>获取或设置图片尺寸（官方 <c>img_size</c>）。</summary>
    [JsonPropertyName("img_size")]
    public MpImageSize? ImageSize { get; set; }
}

// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 扫码信息（官方菜单事件 <c>ScanCodeInfo</c> 节点；来源：自定义菜单事件推送页 F18）。
/// </summary>
/// <remarks>
/// 单对象嵌套，经 <c>Object&lt;TSingle&gt;</c> 通道声明化（元素名 ↔ 属性名配对由编译器校验）。
/// 节点缺失 ⇒ <c>null</c>。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpScanCodeInfo
{
    /// <summary>扫描类型（官方 <c>ScanType</c>：<c>qrcode</c> 二维码 / <c>barcode</c> 一维码）。</summary>
    [PayloadField("ScanType")]
    public string? ScanType { get; set; }

    /// <summary>扫描结果（官方 <c>ScanResult</c>：扫码所得内容）。</summary>
    [PayloadField("ScanResult")]
    public string? ScanResult { get; set; }
}

/// <summary>
/// 发送的图片信息（官方菜单事件 <c>SendPicsInfo</c> 节点；F18）。
/// </summary>
/// <remarks>
/// 官方嵌套为 <c>SendPicsInfo/Count</c> 与 <c>SendPicsInfo/PicList/item/PicMd5Sum</c>（三层）：
/// <c>item</c> 无文本（文本在其子节点），故经 <c>ItemsObject</c> 声明化为强类型项
/// <see cref="MpSendPicItem"/>（走叶层投影豁免表：「容器 + item 项」包装形态不合并同名兄弟）。
/// 节点缺失 ⇒ <c>null</c>。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpSendPicsInfo
{
    /// <summary>发送的图片数量（官方 <c>Count</c>）。</summary>
    [PayloadField("Count")]
    public long? Count { get; set; }

    /// <summary>图片项列表（官方 <c>PicList/item</c>，项内 <c>PicMd5Sum</c> 可用于校验接收到的图片）。</summary>
    [PayloadField("PicList", Format = PayloadFieldFormat.ItemsObject, ItemName = "item")]
    public List<MpSendPicItem> PicList { get; set; } = new List<MpSendPicItem>();
}

/// <summary>图片项（官方 <c>SendPicsInfo/PicList/item</c>；F18）。</summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpSendPicItem
{
    /// <summary>图片的 MD5 值（官方 <c>PicMd5Sum</c>；处理器可据此校验下载到的图片）。</summary>
    [PayloadField("PicMd5Sum")]
    public string? PicMd5Sum { get; set; }
}

/// <summary>
/// 地理位置信息（官方菜单事件 <c>SendLocationInfo</c> 节点；F18）。
/// </summary>
/// <remarks>
/// 字段名与普通 <c>location</c> 消息一致（<c>Location_X</c>/<c>Location_Y</c>/<c>Scale</c>/<c>Label</c>），
/// 但多出 <c>Poiname</c>（POI 名称，注意官方拼写为 Poiname 非 PoiName —— 不得「顺手修正」）。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
public sealed partial class MpSendLocationInfo
{
    /// <summary>地理位置维度（官方 <c>Location_X</c>）。</summary>
    [PayloadField("Location_X")]
    public string? Latitude { get; set; }

    /// <summary>地理位置经度（官方 <c>Location_Y</c>）。</summary>
    [PayloadField("Location_Y")]
    public string? Longitude { get; set; }

    /// <summary>地图缩放大小（官方 <c>Scale</c>）。</summary>
    [PayloadField("Scale")]
    public string? Scale { get; set; }

    /// <summary>地理位置信息（官方 <c>Label</c>）。</summary>
    [PayloadField("Label")]
    public string? Label { get; set; }

    /// <summary>POI 名称（官方 <c>Poiname</c>；**官方拼写陷阱**：非 <c>PoiName</c>）。</summary>
    [PayloadField("Poiname")]
    public string? PoiName { get; set; }
}

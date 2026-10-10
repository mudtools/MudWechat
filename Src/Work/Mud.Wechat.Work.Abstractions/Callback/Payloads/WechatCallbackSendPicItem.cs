// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 发送的图片项（<c>pic_sysphoto</c>/<c>pic_photo_or_album</c>/<c>pic_weixin</c> 报文的
/// <c>SendPicsInfo/PicList/item</c> 节点；官方 path 90240）。
/// </summary>
/// <remarks>
/// 官方 <c>item</c> 节点无文本、值在其子节点 <c>PicMd5Sum</c> —— 既有的标量 <c>Items</c> 通道
/// 取 <c>child.Value</c> 不适用，故随 G-ADR-17 拆出本契约化项类型（经 <c>ItemsObject</c> 收集）。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackSendPicItem
{
    /// <summary>图片的 MD5 值（官方 <c>PicMd5Sum</c>，可用于校验接收到的图片）。</summary>
    [PayloadField("PicMd5Sum")]
    public string? PicMd5Sum { get; set; }
}

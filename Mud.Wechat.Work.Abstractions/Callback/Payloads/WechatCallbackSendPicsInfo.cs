// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 发送的图片信息（<c>pic_sysphoto</c>/<c>pic_photo_or_album</c>/<c>pic_weixin</c> 报文的
/// <c>SendPicsInfo</c> 节点；官方 path 90240）。
/// </summary>
/// <remarks>
/// 由 <c>WechatPayloadConverter.ParseSendPicsInfo</c> 在转换器内手工组装：
/// 官方嵌套为 <c>SendPicsInfo/Count</c> 与 <c>SendPicsInfo/PicList/item/PicMd5Sum</c>（三层），
/// 超出生成器「容器 → 单层同构项」的表达力。节点缺失 ⇒ 返回 <c>null</c>。
/// </remarks>
public sealed class WechatCallbackSendPicsInfo
{
    /// <summary>发送的图片数量（官方 <c>Count</c>）。</summary>
    public long? Count { get; set; }

    /// <summary>图片 MD5 值列表（官方 <c>PicList/item/PicMd5Sum</c>，可用于校验接收到的图片）。</summary>
    public List<string> PicMd5Sums { get; set; } = new List<string>();
}

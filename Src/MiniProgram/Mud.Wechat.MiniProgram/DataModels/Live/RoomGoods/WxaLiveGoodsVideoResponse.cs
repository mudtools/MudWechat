// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 商品讲解视频应答（<c>POST /wxaapi/broadcast/goods/getVideo</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_downloadgoodsvideo.html"/>。
/// </para>
/// <para>
/// <b>官方返回体表只有 <c>url</c> 与 <c>errcode</c>（无 <c>errmsg</c>）</b>，而官方本页文件名与接口英文名均为
/// <c>downloadgoodsvideo</c>/<c>downloadGoodsVideo</c>、路由却是 <c>goods/getVideo</c>
/// —— 页面标题（下载商品讲解视频）与路由动词（get）不一致，属官方原文形态，路由照录。
/// </para>
/// <para>官方错误码：<c>300044</c>（商品没有讲解视频）/ <c>300045</c>（讲解视频未生成）⇒ 两者为业务态而非参数错误。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveGoodsVideoResponse : WxaResponse
{
    /// <summary>讲解视频链接（<c>url</c>，官方原文「讲解链接」）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

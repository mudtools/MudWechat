// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 客服功能管理请求体（<c>POST /wxaapi/broadcast/room/updatekf</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_updatekf.html"/>。
/// </para>
/// <para>
/// <b>前置条件</b>：创建直播间若未配置客服会报 <c>300038</c>「未在小程序管理后台配置客服」⇒
/// 开启直播前须先在小程序管理后台配置客服，本端点只切换开关。
/// </para>
/// <para>调用额度：10000 次/天。官方错误码：<c>-1</c>（系统繁忙）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLiveKfSwitchRequest
{
    /// <summary>房间 ID（<c>roomId</c>，必填）。</summary>
    [JsonPropertyName("roomId")]
    public long? RoomId { get; set; }

    /// <summary>是否关闭客服（<c>closeKf</c>，必填）：0 开启、1 关闭。</summary>
    [JsonPropertyName("closeKf")]
    public long? CloseKf { get; set; }
}

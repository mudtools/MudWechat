// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Bots;

/// <summary>
/// 智能机器人长连接帧命令常量（官方 <c>cmd</c> 取值，101463，2026-10-11 官方文档逐段核验）。
/// </summary>
/// <remarks>
/// 帧外壳形态统一为 <c>{cmd, headers: {req_id}, body}</c>；服务端应答帧统一为
/// <c>{headers: {req_id}, errcode, errmsg}</c>（<c>init</c> / <c>finish</c> 的应答另带 <c>body</c> 载荷）。
/// <b>长连接帧无签名</b> ⇒ 无指纹闸，<c>msgid</c> 去重与业务幂等归宿主（信封暴露 <c>MsgId</c>）。
/// </remarks>
public static class WechatBotFrameCommands
{
    /// <summary>订阅帧（建连后由客户端发送；body 为 <c>{bot_id, secret}</c>。有频率保护，成功后禁止反复订阅）。</summary>
    public const string Subscribe = "aibot_subscribe";

    /// <summary>消息回调帧（服务端下发；body 即回调模式的 <c>AibotMessageCallback</c> 报文，明文）。</summary>
    public const string MsgCallback = "aibot_msg_callback";

    /// <summary>事件回调帧（服务端下发；body 即回调模式的 <c>AibotEventCallback</c> 报文，明文）。</summary>
    public const string EventCallback = "aibot_event_callback";

    /// <summary>欢迎语应答帧（仅 <c>enter_chat</c> 事件，官方要求 5 秒内回复）。</summary>
    public const string RespondWelcomeMsg = "aibot_respond_welcome_msg";

    /// <summary>普通/流式应答帧（<c>req_id</c> 须<b>透传</b>回调帧的值；同一次回调的流式回复须用相同 req_id）。</summary>
    public const string RespondMsg = "aibot_respond_msg";

    /// <summary>模板卡片更新应答帧（仅模板卡片点击事件，官方要求 5 秒内回复；body 为 <c>response_type=update_template_card</c>）。</summary>
    public const string RespondUpdateMsg = "aibot_respond_update_msg";

    /// <summary>主动推送帧（body 另带 <c>chatid</c> / <c>chat_type</c>；需用户在会话中先发过消息）。</summary>
    public const string SendMsg = "aibot_send_msg";

    /// <summary>素材上传初始化帧（应答 body 含 <c>upload_id</c>；会话 30 分钟有效）。</summary>
    public const string UploadMediaInit = "aibot_upload_media_init";

    /// <summary>素材上传分片帧（单分片 ≤512KB（base64 前）、≤100 片；分片可乱序、重复上传幂等忽略）。</summary>
    public const string UploadMediaChunk = "aibot_upload_media_chunk";

    /// <summary>素材上传结束帧（应答 body 含 <c>media_id</c>，3 天内有效）。</summary>
    public const string UploadMediaFinish = "aibot_upload_media_finish";

    /// <summary>心跳帧（官方建议每 30 秒一次；长时间未收到心跳服务端会主动断开。无 body）。</summary>
    public const string Ping = "ping";
}

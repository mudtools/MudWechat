// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

using System.Text.Json;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 长连接帧编解码（官方 101463 帧外壳 <c>{cmd, headers: {req_id}, body}</c> 的唯一读写咽喉点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两阶段反序列化</b>：<see cref="ParseFrame"/> 只解外壳（<see cref="AibotFrame.Body"/> 保留为
/// <see cref="JsonElement"/>），调用方按 <see cref="AibotFrame.Cmd"/> 取对应 <c>JsonTypeInfo</c> 解
/// <see cref="AibotFrame.Body"/> —— 禁用反射多态（AOT 红线）。应答帧走 <see cref="ParseAck"/>。
/// </para>
/// <para>
/// <b>长连接帧无签名</b>（官方不提供帧级签名）⇒ 不触碰 <c>IWechatCallbackReplayGuard</c>（回调模式专用）；
/// <c>msgid</c> 去重与业务幂等归宿主（信封暴露 <c>MsgId</c>，R10）。
/// </para>
/// </remarks>
internal static class WechatBotFrameCodec
{
    /// <summary>序列化出站帧（强类型 body 先转 <see cref="JsonElement"/> 再挂外壳，零反射）。</summary>
    /// <typeparam name="TBody">帧体类型（源生成上下文登记的闭合类型）。</typeparam>
    /// <param name="cmd">帧命令（<c>WechatBotFrameCommands</c> 常量）。</param>
    /// <param name="reqId">请求唯一标识（应答帧关联凭据；回调应答须<b>透传</b>回调帧的 req_id）。</param>
    /// <param name="body">帧体（<c>null</c> = 无 body，如 ping）。</param>
    /// <param name="bodyTypeInfo">帧体的源生成 <c>JsonTypeInfo</c>。</param>
    /// <returns>JSON 帧文本。</returns>
    internal static string EncodeFrame<TBody>(
        string cmd, string reqId, TBody? body, System.Text.Json.Serialization.Metadata.JsonTypeInfo<TBody> bodyTypeInfo)
        where TBody : class
    {
        var frame = new AibotFrame
        {
            Cmd = cmd,
            Headers = new AibotFrameHeaders { ReqId = reqId },
            Body = body == null ? null : JsonSerializer.SerializeToElement(body, bodyTypeInfo),
        };

        return JsonSerializer.Serialize(frame, AibotJsonContext.Default.AibotFrame);
    }

    /// <summary>解析入站帧外壳（第一阶段；body 由调用方按 cmd 二阶段解析）。</summary>
    /// <param name="json">WebSocket 帧文本。</param>
    /// <returns>帧外壳；<c>cmd</c> 缺失即抛（非官方帧形态）。</returns>
    internal static AibotFrame ParseFrame(string json)
    {
        var frame = TryParse(json, AibotJsonContext.Default.AibotFrame);
        if (frame?.Cmd == null || frame.Cmd.Length == 0)
        {
            throw new InvalidOperationException("智能机器人长连接收到非法帧：缺少 cmd（非官方 101463 帧形态）。");
        }

        return frame;
    }

    /// <summary>解析应答帧（<c>{headers, errcode, errmsg}</c>；init/finish 应答另带 body）。</summary>
    internal static AibotFrameAck ParseAck(string json)
    {
        var ack = TryParse(json, AibotJsonContext.Default.AibotFrameAck);
        if (ack == null)
        {
            throw new InvalidOperationException("智能机器人长连接收到非法应答帧（无法按官方 {headers, errcode, errmsg} 解析）。");
        }

        return ack;
    }

    /// <summary>按 <see cref="AibotFrame.Cmd"/> 二阶段解析帧体（返回 <c>null</c> = body 缺省）。</summary>
    internal static TBody? ParseBody<TBody>(JsonElement? body, System.Text.Json.Serialization.Metadata.JsonTypeInfo<TBody> typeInfo)
        where TBody : class
    {
        if (body is not { ValueKind: JsonValueKind.Object } element)
        {
            return null;
        }

        return element.Deserialize(typeInfo);
    }

    /// <summary>生成 <c>req_id</c>（进程内全局唯一；官方要求请求唯一标识）。</summary>
    internal static string NewReqId() => Guid.NewGuid().ToString("N");

    private static T? TryParse<T>(string json, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo)
        where T : class
    {
        try
        {
            return JsonSerializer.Deserialize(json, typeInfo);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("智能机器人长连接帧解析失败（非官方 101463 JSON 形态）。", ex);
        }
    }
}

#endif

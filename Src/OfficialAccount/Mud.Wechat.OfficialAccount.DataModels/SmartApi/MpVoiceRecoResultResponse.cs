// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 获取语音识别结果（<c>POST /cgi-bin/media/voice/queryrecoresultfortext</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：仅 <c>result</c>（识别结果）。
/// </para>
/// <para>
/// <b>异步轮询语义（官方注意事项原文，SDK 不编排）</b>：「<b>添加完文件之后 10s 内调用这个接口</b>」
/// ——上传语音（<c>addvoicetorecofortext</c>）后须在 10 秒内以同一 <c>voice_id</c> 查询；
/// 轮询次数与退避策略归宿主（「SDK 不做业务编排」红线）。
/// </para>
/// <para>官方错误码：<c>-1</c> / <c>40001</c>（本页错误码表仅此两行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpVoiceRecoResultResponse : MpResponse
{
    /// <summary>获取或设置识别结果（官方 <c>result</c>）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }
}

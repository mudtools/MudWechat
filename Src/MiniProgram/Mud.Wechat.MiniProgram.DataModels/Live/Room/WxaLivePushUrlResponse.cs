// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Live;

/// <summary>
/// 获取直播间推流地址应答（<c>GET /wxaapi/broadcast/room/getpushurl</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/studio-management/api_getpushurl.html"/>。
/// </para>
/// <para>
/// <b><see cref="PushAddr"/> 含密钥，禁止落日志</b>：官方示例的推流地址带 <c>txSecret</c> 与 <c>txTime</c>
/// 查询参数（形如 <c>rtmp://…?txSecret=…&amp;txTime=…</c>）⇒ 属推流凭据，与 <c>permanent_code</c> 同级别的敏感值。
/// </para>
/// <para>官方错误码：<c>43001</c>（require GET method，误用 POST 时返回）/ <c>300022</c> / <c>300023</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class WxaLivePushUrlResponse : WxaResponse
{
    /// <summary>推流地址（<c>pushAddr</c>，含 <c>txSecret</c> / <c>txTime</c> 凭据参数，禁止写入日志或遥测）。</summary>
    [JsonPropertyName("pushAddr")]
    public string? PushAddr { get; set; }
}

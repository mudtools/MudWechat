// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「查询下载订单任务」响应：任务状态与下载链接。
/// </summary>
/// <remarks>
/// <para><b><c>download_url</c> 是带签名的临时地址、<c>expire_at</c> 给出失效时刻</b> ⇒ 不得写进日志、遥测或异常消息（与短剧媒资 <c>cdn_url</c>、CDN 日志链接同级别处置），也不得持久化后复用。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_download_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayQueryDownloadOrderResponse : WxaResponse
{
    /// <summary>task_id（官方类型 <c>string</c>）：下载任务 ID，与请求参数对应</summary>
    [JsonPropertyName("task_id")]
    public string? TaskId { get; set; }

    /// <summary>status（官方类型 <c>number</c>）：任务状态：0=初始化 /1=运行中 /2=成功 /3=失败</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>download_url（官方类型 <c>string</c>）：下载文件 URL，仅 status=2(成功) 时有值</summary>
    [JsonPropertyName("download_url")]
    public string? DownloadUrl { get; set; }

    /// <summary>expire_at（官方类型 <c>number</c>）：URL 过期时间（Unix 秒级时间戳）</summary>
    [JsonPropertyName("expire_at")]
    public long? ExpireAt { get; set; }
}

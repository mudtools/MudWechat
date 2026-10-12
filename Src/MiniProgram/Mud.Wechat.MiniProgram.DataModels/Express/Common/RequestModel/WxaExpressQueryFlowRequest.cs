// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>查询门店运费流水请求体（<c>POST /cgi-bin/express/intracity/queryflow</c>）。</summary>
/// <remarks>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/express/express-intracity/api_queryflow"/>；<c>flow_type</c>：1 充值，2 扣款，3 退款，4 扣款退回。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressQueryFlowRequest
{
    /// <summary>微信门店编号（<c>wx_store_id</c>，必填）。</summary>
    [JsonPropertyName("wx_store_id")]
    public string? StoreId { get; set; }

    /// <summary>流水类型（<c>flow_type</c>，必填；1 充值，2 扣款，3 退款，4 扣款退回）。</summary>
    [JsonPropertyName("flow_type")]
    public int FlowType { get; set; }

    /// <summary>运力 ID（<c>service_trans_id</c>，可选）。</summary>
    [JsonPropertyName("service_trans_id")]
    public string? ServiceTransId { get; set; }

    /// <summary>开始时间（<c>begin_time</c>，可选；Unix 时间戳）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>结束时间（<c>end_time</c>，可选；Unix 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}

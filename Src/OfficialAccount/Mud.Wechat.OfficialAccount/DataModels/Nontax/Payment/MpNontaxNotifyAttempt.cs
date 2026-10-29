// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「查询订单」响应的单次通知尝试（官方 <c>notify_detail</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxNotifyAttempt
{
    /// <summary>获取或设置错误码（官方 <c>errcode</c>）。</summary>
    [JsonPropertyName("errcode")]
    public int ErrorCode { get; set; }

    /// <summary>获取或设置错误信息（官方 <c>errmsg</c>，可选）。</summary>
    [JsonPropertyName("errmsg")]
    public string? ErrorMessage { get; set; }

    /// <summary>获取或设置返回码（官方 <c>ret</c>）。</summary>
    [JsonPropertyName("ret")]
    public int Ret { get; set; }

    /// <summary>获取或设置返回信息（官方 <c>ret_errmsg</c>，可选）。</summary>
    [JsonPropertyName("ret_errmsg")]
    public string? RetErrorMessage { get; set; }

    /// <summary>获取或设置通知地址（官方 <c>url</c>，可选）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>获取或设置加密的通知报文（官方 <c>wxnontaxstr</c>）。</summary>
    [JsonPropertyName("wxnontaxstr")]
    public string? WxNontaxString { get; set; }

    /// <summary>获取或设置通知状态（官方 <c>status</c>）。</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>获取或设置耗时（官方 <c>cost_time</c>，毫秒）。</summary>
    [JsonPropertyName("cost_time")]
    public int CostTime { get; set; }

    /// <summary>获取或设置通知时间（官方 <c>notify_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("notify_time")]
    public long NotifyTime { get; set; }

    /// <summary>获取或设置第三方响应（官方 <c>third_resp</c>，可选）。</summary>
    [JsonPropertyName("third_resp")]
    public string? ThirdResp { get; set; }

    /// <summary>获取或设置第三方响应数据（官方 <c>third_resp_data</c>，可选）。</summary>
    [JsonPropertyName("third_resp_data")]
    public string? ThirdRespData { get; set; }
}

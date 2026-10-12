// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>
/// 即时配送取消 / 预取消配送单<b>共用</b>响应（<c>POST .../order/{cancel,precancel}</c>）。
/// </summary>
/// <remarks><c>deduct_fee</c> 语义按端点区分：<c>cancel</c> 为<b>扣除</b>的违约金，<c>precancel</c> 为<b>预计扣除</b>的违约金（单位：元，精确到分）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressLocalCancelOrderResponse : WxaExpressLocalResponse
{
    /// <summary>违约金（<c>deduct_fee</c>；单位：元，精确到分）。</summary>
    [JsonPropertyName("deduct_fee")]
    public decimal DeductFee { get; set; }

    /// <summary>说明（<c>desc</c>）。</summary>
    [JsonPropertyName("desc")]
    public string? Description { get; set; }
}

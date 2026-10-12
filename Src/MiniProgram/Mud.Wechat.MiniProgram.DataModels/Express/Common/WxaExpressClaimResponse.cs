// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>无忧退理赔响应（<c>POST /wxa/business/insurance_freight/claim</c>）。</summary>
/// <remarks><c>is_home_pick_up</c> 枚举：<c>0</c> 否，<c>1</c> 是。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressClaimResponse : WxaResponse
{
    /// <summary>理赔报案号（<c>report_no</c>；成功申请理赔时返回）。</summary>
    [JsonPropertyName("report_no")]
    public string? ReportNo { get; set; }

    /// <summary>是否上门取件（<c>is_home_pick_up</c>；<c>0</c> 否，<c>1</c> 是）。</summary>
    [JsonPropertyName("is_home_pick_up")]
    public int IsHomePickUp { get; set; }
}

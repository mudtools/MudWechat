// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 批量预录入门店信息回包（<c>POST /wxa/business/batchcreateretail</c>）。
/// </summary>
/// <remarks>
/// <para><b>部分成功</b>：<c>errcode=0</c> 仍可能含失败项，须结合 <c>num_failure</c> 与 <c>failure_record_list</c> 判定。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessBatchCreateRetailResponse : WxaResponse
{
    /// <summary>成功条数。</summary>
    [JsonPropertyName("num_success")]
    public int? NumSuccess { get; set; }

    /// <summary>失败条数。</summary>
    [JsonPropertyName("num_failure")]
    public int? NumFailure { get; set; }

    /// <summary>失败明细。</summary>
    [JsonPropertyName("failure_record_list")]
    public List<WxaBusinessBatchCreateRetailFailureRecord>? FailureRecordList { get; set; }
}

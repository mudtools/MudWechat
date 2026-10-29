// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.BusinessCapability;

/// <summary>
/// 批量预录入门店信息请求体（<c>POST /wxa/business/batchcreateretail</c>）。
/// </summary>
/// <remarks>
/// <para><b>批量上限 100 条</b>：官方限制 <c>retail_info_list</c> 单次不超过 100 项，超出整单拒绝。</para>
/// <para><b>部分成功语义</b>：回包以 <c>num_success</c> / <c>num_failure</c> 与逐条失败记录返回，
/// 调用方须按 <c>mobile_phone</c> 或 <c>registration_number</c> 复核后再重试失败项。</para>
/// <para>权限集 158；错误码 <c>9404000</c> / <c>9404001</c> 为 B2b 门店业务拒绝对。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BusinessCapability")]
public class WxaBusinessBatchCreateRetailRequest
{
    /// <summary>门店信息列表（必填，≤100 条）。</summary>
    [JsonPropertyName("retail_info_list")]
    public List<WxaBusinessRetailInfoCreate>? RetailInfoList { get; set; }
}

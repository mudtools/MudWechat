// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 查询申请单状态请求体（<c>/cgi-bin/miniapppay/get_applyment_status</c>）。
/// <para>官方业务限制：仅能查询该应用本身提交的申请单。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayApplymentStatusRequest
{
    /// <summary>
    /// 获取或设置业务申请编号（官方必填，长度 1~32 个字符，
    /// 即提交创建对外收款账户申请单时填写的 out_request_no）。
    /// </summary>
    [JsonPropertyName("out_request_no")]
    public string? OutRequestNo { get; set; }
}

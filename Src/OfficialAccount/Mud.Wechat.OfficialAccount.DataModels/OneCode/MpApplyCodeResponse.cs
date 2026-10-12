// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

/// <summary>
/// 申请二维码（<c>POST /intp/marketcode/applycode</c>）响应。
/// </summary>
/// <remarks>
/// <b>响应为平级字段（无 <c>data</c> 包裹）</b>——本域 6 端点均为平级形态（与门店域的 <c>data</c> 包裹不同）。
/// 返回<b>不含二维码本体</b>，仅申请单号；取码须另调 <c>applycodequery</c> 与 <c>applycodedownload</c>。
/// 官方错误码：<c>0</c> / <c>40001</c> / <c>40002</c>（后两项见类型级 remarks 的误挂说明）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpApplyCodeResponse : MpResponse
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }
}

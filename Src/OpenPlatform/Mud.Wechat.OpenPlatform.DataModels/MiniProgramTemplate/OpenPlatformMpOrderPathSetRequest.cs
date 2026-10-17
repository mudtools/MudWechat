// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;

/// <summary>
/// 「申请设置订单页 path 信息」请求（<c>applysetorderpathinfo</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/applySetOrderPathInfo.html"/>。
/// </summary>
/// <remarks>
/// <para>本端点为 <b>component_access_token</b> 面，与授权方 <c>getorderpathinfo</c>（<c>authorizer_access_token</c>）令牌归属不同；
/// 结果通过 <c>order_path_audit_result_notify</c> 异步推送回授权事件接收 URL，本接口仅同步返回 <c>errcode</c>/<c>errmsg</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramTemplate")]
public class OpenPlatformMpOrderPathSetRequest
{
    /// <summary>批量申请的信息（官方必填 <c>batch_req</c>，object）。</summary>
    [JsonPropertyName("batch_req")]
    public OpenPlatformMpOrderPathBatchReq? BatchReq { get; set; }
}

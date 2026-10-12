// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

// ---------------------------------------------------------------- 查询二维码申请单（POST /intp/marketcode/applycodequery）

/// <summary>
/// 查询二维码申请单（<c>POST /intp/marketcode/applycodequery</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方必填性矛盾（照录，SDK 以可空承载）</b>：官方字段表把 <c>application_id</c> 与
/// <c>isv_application_id</c> <b>都标为必填</b>，但官方注意事项原文为「需要通过 <c>application_id</c>
/// <b>或</b> <c>isv_application_id</c> 查询申请单状态」⇒ 二者实为<b>二选一</b>。SDK 不本地校验。
/// </para>
/// <para>
/// <b>类型口径说明</b>：官方字段表标 <c>application_id</c> 为 <c>number</c>，而官方<b>请求示例</b>写作字符串
/// <c>"581865877"</c>。按本 SDK 冲突处置规则，标量冲突的「取示例」仅适用于<b>返回示例（真机报文）</b>；
/// <b>请求示例</b>由文档作者手写且渲染常加引号，不构成口径依据 ⇒ 请求侧取<b>字段表</b>（数值）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpApplyCodeQueryRequest
{
    /// <summary>获取或设置申请单号（官方 <c>application_id</c>；与 <see cref="IsvApplicationId"/> 二选一）。</summary>
    [JsonPropertyName("application_id")]
    public long? ApplicationId { get; set; }

    /// <summary>获取或设置外部单号（官方 <c>isv_application_id</c>；与 <see cref="ApplicationId"/> 二选一）。</summary>
    [JsonPropertyName("isv_application_id")]
    public string? IsvApplicationId { get; set; }
}

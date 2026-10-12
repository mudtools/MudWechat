// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.KfSession;

/// <summary>
/// 获取未接入会话列表响应（<c>getwaitcase</c>，<c>GET /customservice/kfsession/getwaitcase</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>GET 且无请求体</b>（无业务 Query 参数）。
/// </para>
/// <para>
/// <b>返回上限（官方原文）</b>：<c>waitcaselist</c> <b>最多返回 100 条数据，按照来访顺序</b>
/// ⇒ 调用方不得假定能一次取全（官方未提供该接口的分页游标，超出部分需靠反复轮询与人工接入消化）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "KfSession")]
public class MpGetWaitCaseResponse : MpResponse
{
    /// <summary>未接入会话数量。</summary>
    [JsonPropertyName("count")]
    public int Count { get; set; }

    /// <summary>未接入会话列表（最多 100 条，按来访顺序）。</summary>
    [JsonPropertyName("waitcaselist")]
    public List<MpWaitCase>? WaitCaseList { get; set; }
}

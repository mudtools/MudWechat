// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 按 media_id 寻址永久素材的请求体（<c>material/get_material</c> 与 <c>material/del_material</c> 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决（N4：字段集一致才共用，由守卫双向锁定）</b>：两页请求体字段表<b>均为单个
/// <c>media_id</c>（必填）</b>，形态完全一致 ⇒ 共用同一 DTO（两份声明会漂移）。
/// </para>
/// <para>
/// 官方「注意事项」（删除页，逐页核验 2026-10-07）：「请谨慎操作本接口，可以删除官网素材管理模块中的
/// 图文/语音/视频等素材（需先通过获取素材列表获取 media_id）」；「临时素材无法通过本接口删除」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpMediaIdRequest
{
    /// <summary>获取或设置目标素材的 media_id（官方 <c>media_id</c>，必填）。</summary>
    [JsonPropertyName("media_id")]
    public string MediaId { get; set; } = string.Empty;
}
